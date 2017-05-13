using Nito.AsyncEx;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CapnpNet.Rpc
{
  public sealed class ExportTable<T>
  {
    private static T _empty;

    private T[] _slots = new T[16]; // TODO: pool

    // TODO: find a priority queue implementation
    private readonly SortedList<uint, uint> _freeList = new SortedList<uint, uint>();

    private uint _count; // FIXME: mixed meaning
    
    public ref T TryGet(uint key, out bool valid)
    {
      valid = key < _count;
      if (!valid) return ref _empty;

      return ref _slots[key];
    }

    private ref T GetOrAdd(uint key)
    {
      if (key < _slots.Length) return ref _slots[key];

      Array.Resize(ref _slots, _slots.Length * 2);
      return ref _slots[key];
    }

    public T Remove(uint key, ref T entry)
    {
      ref T val = ref this.TryGet(key, out bool valid);
      if (!valid || !Unsafe.AreSame(ref val, ref entry))
      {
        throw new InvalidOperationException($"Entry not present at key ${key}");
      }

      T copy = val;
      val = default(T);
      _freeList.Add(key, key);
      _count--;
      return copy;
    }

    public ref T Next(out uint id)
    {
      if (_freeList.Count > 0)
      {
        id = _freeList.Keys[0];
        _freeList.RemoveAt(0);
      }
      else
      {
        id = _count;
        _count++;
      }

      return ref this.GetOrAdd(id);
    }
  }

  public sealed class ImportTable<T>
  {
    private struct Entry
    {
      public uint key;
      public T value;
      public bool active;
    }
    
    private Entry[] _entries;
    
    private T _empty;
    private uint _count;
    
    public ref T TryGet(uint key, out bool valid)
    {
      for (int i = 0; i < _entries.Length; i++)
      {
        ref var entry = ref _entries[i];
        if (entry.key == key && entry.active)
        {
          valid = true;
          return ref entry.value;
        }
      }

      valid = false;
      return ref _empty;
    }

    private ref T GetOrAdd(uint key)
    {
      int? emptyIndex = null;
      for (int i = 0; i < _entries.Length; i++)
      {
        ref var entry = ref _entries[i];
        if (entry.key == key && entry.active)
        {
          return ref entry.value;
        }
        else if (entry.active == false && emptyIndex == null)
        {
          emptyIndex = i;
        }
      }

      if (emptyIndex == null)
      {
        emptyIndex = _entries.Length;
        Array.Resize(ref _entries, _entries.Length * 2);
      }

      ref var newEntry = ref _entries[emptyIndex.Value];
      newEntry.key = key;
      newEntry.active = true;
      _count++;
      return ref newEntry.value;
    }

    public T Remove(uint key)
    {
      for (int i = 0; i < _entries.Length; i++)
      {
        ref var entry = ref _entries[i];
        if (entry.key == key && entry.active)
        {
          var copy = entry.value;
          entry.key = 0;
          entry.value = default(T);
          entry.active = false;
          _count--;
          return copy;
        }
      }

      throw new InvalidOperationException($"Entry with key {key} not found.");
    }
  }
  
  public class RpcConnection : IDisposable
  {
    // question/answer/export/import tables
    private MessageStream _msgStream;
    private ArrayPoolSegmentFactory _segFactory;
    private CancellationTokenSource _ctSource;
    private AsyncCountdownEvent _inFlightCounter;
    private ConcurrentQueue<System.Exception> _exceptions;

    private struct Question
    {
      public List<uint> paramExports;
      public bool isAwaitingReturn;
      public bool isTailCall;
    }
    private struct Answer
    {
      public bool active;
      // something about pipelines
      //public Task<RpcResponse> redirectedResults;
      //public RpcCallContext callContext;
      public List<uint> resultExports;
    }
    private struct Export
    {
      public uint refCount;
      public ICapability capability;
      public Task resolveOp;
    }
    private struct Import
    {
      //public ImportClient importClient;
      //public RpcClient appClient;
      //public TaskCompletionSource<ClientHook> promiseFulfiller;
    }

    private ExportTable<Export> _exports;
    private ExportTable<Question> _questions;
    private ImportTable<Answer> _answers;
    private ImportTable<Import> _imports;
    private ICapability _bootstrapCap;

    public void Init(Stream readStream, Stream writeStream, ICapability bootstrapCapability)
    {
      _msgStream = new MessageStream();
      _segFactory = new ArrayPoolSegmentFactory();
      _msgStream.Init(readStream, writeStream, _segFactory);
      _ctSource = new CancellationTokenSource();
      _exceptions = new ConcurrentQueue<System.Exception>();
      _exports = new ExportTable<Export>();
      _bootstrapCap = bootstrapCapability;
    }

    public async Task StartReadLoop()
    {
      _inFlightCounter = new AsyncCountdownEvent(1);

      var ct = _ctSource.Token;
      while (ct.IsCancellationRequested == false)
      {
        var msg = await _msgStream.ReceiveAsync();
        this.Consume(this.ProcessAsync(msg));
      }

      _inFlightCounter.Signal();
      await _inFlightCounter.WaitAsync();

      if (_exceptions.Count > 0)
      {
        throw new AggregateException(_exceptions);
      }
    }

    private void Consume(Task task)
    {
      if (task.IsCompleted)
      {
        if (task.IsFaulted)
        {
          _exceptions.Enqueue(task.Exception);
          _ctSource.Cancel();
        }
      }
      else
      {
        _inFlightCounter.AddCount();
        task.ContinueWith(t =>
        {
          this.Consume(t);
          _inFlightCounter.Signal();
        }, TaskContinuationOptions.ExecuteSynchronously);
      }
    }

    internal async Task ProcessAsync(CapnpNet.Message message)
    {
      try
      {
        var msg = message.GetRoot<Message>();
        if (msg.Is(out Message unimplemented))
        {
          // TODO: release resources
          throw new NotImplementedException();
        }
        else if (msg.Is(out Exception abort))
        {
          throw new InvalidOperationException($"Connection aborted by remote: {abort.type.ToString()}: {abort.reason.ToString()}");
        }
        else if (msg.Is(out Bootstrap bootstrap))
        {
          throw new NotImplementedException();
          //var reply = new CapnpNet.Message().Init(_segFactory);
          ////await reply.PreAllocateAsync(senderMsg.CalculateSize() + 3);
          //reply.Allocate(0, 1).WritePointer(0, new Return(reply)
          //{
          //  answerId = bootstrap.questionId,
          //  which = Return.Union.results,
          //  results = new Payload
          //  {
          //    content = 
          //  }
          //});
          //message.Dispose();
          //message = null;
          //await _msgStream.WriteAsync(reply);
          //reply.Dispose();
        }
        //else if (msg.Is(out Call call))
        //{

        //}
        //else if (msg.Is(out Return ret))
        //{

        //}
        //else if (msg.Is(out Finish finish))
        //{

        //}
        else
        {
          // level -1 RPC
          var senderMsg = msg.GetStruct().Segment.Message;
          // TODO: copy data, or just move segments?
          var reply = this.CreateMessage();
          await reply.PreAllocateAsync(senderMsg.CalculateSize() + 3);
          reply.Allocate(0, 1).WritePointer(0, new Message(reply)
          {
            which = Message.Union.unimplemented,
            unimplemented = msg.CopyTo(reply),
          });
          message.Dispose();
          message = null;
          await _msgStream.WriteAsync(reply);
          reply.Dispose();
        }
      }
      finally
      {
        if (message != null) message.Dispose();
      }
    }

    public void SendAndDispose(CapnpNet.Message msg)
    {
      async Task Run(CapnpNet.Message m)
      {
        // TODO: walk message, dereference capabilities from message, add them to our own export
        //       table, rewrite pointers
        var exportCache = new Dictionary<uint, uint>(); // why not just ICap->id?
        m.Root.ReplaceCaps((oldId, cap) =>
        {
          if (exportCache.TryGetValue(oldId, out uint newId)) return newId;
          else return (exportCache[oldId] = this.ExportCap(cap));
        });

        await _msgStream.WriteAsync(m);
        m.Dispose();
      }

      this.Consume(Run(msg));
    }

    private uint ExportCap(ICapability cap)
    {
      // TODO: do we try to de-duplicate exports?
      ref Export export = ref _exports.Next(out uint id);
      export.capability = cap;
      export.refCount = 1;
      return id;
    }
    
    public void Dispose()
    {
      _msgStream.Dispose();
      _msgStream = null;
      _segFactory = null;
    }

    internal CapnpNet.Message CreateMessage() => new CapnpNet.Message().Init(_segFactory);
  }
  
  public struct CallContext
  {
    private uint _questionId;
    private Struct _parameters;
    private RpcConnection _rpc;

    public Struct Parameters => _parameters;
    
    public void DisposeParameters()
    {
      _parameters.Segment.Message.Dispose();
      _parameters = default(Struct);
    }
    
    public CapnpNet.Message CreateReply(int? sizeHint = null)
    {
      var msg = _rpc.CreateMessage();
      
      msg.PreAllocate(3 + sizeHint ?? 5);
      
      msg.Allocate(0, 1).WritePointer(0, new Message(msg)
      {
        @return = new Return(msg)
        {
          answerId = _questionId
        }
      });

      return msg;
    }

    public async ValueTask<CapnpNet.Message> CreateReplyAsync(int? sizeHint = null)
    {
      var msg = _rpc.CreateMessage();
      
      await msg.PreAllocateAsync(3 + sizeHint ?? 5);
      
      msg.Allocate(0, 1).WritePointer(0, new Message(msg)
      {
        @return = new Return(msg)
        {
          answerId = _questionId
        }
      });

      return msg;
    }

    public Task TailCall(Pointer requestPayload)
    {
      throw new NotImplementedException();
    }
  }

  public struct ReplyContext
  {
    private RpcConnection _rpc;

    public ReplyContext(RpcConnection rpc, CapnpNet.Message msg)
    {
      _rpc = rpc;
      this.Message = msg;
    }

    public CapnpNet.Message Message { get; }
    
    public Payload PrepareReturn()
    {
      var ret = this.Message.GetRoot<Message>().@return;
      ret.which = Rpc.Return.Union.results;
      return (ret.results = new Payload(this.Message));
    }

    public void Return(Pointer returnPayload) // TODO: AnyPointer type? discriminated union of Struct, *List?
    {
      var ret = this.Message.GetRoot<Message>().@return;
      ret.which = Rpc.Return.Union.results;
      ret.results = new Payload(this.Message)
      {
        content = returnPayload
      };

      this.Send();
    }

    public void Throw(string reason)
    {
      var ret = this.Message.GetRoot<Message>().@return;
      ret.which = Rpc.Return.Union.exception;
      ret.exception = new Exception(this.Message)
      {
        type = Exception.Type.failed,
        reason = new Text(this.Message, reason)
      };

      this.Send();
    }

    public void Send() => _rpc.SendAndDispose(this.Message);
  }

  public sealed class MessageStream : IDisposable
  {
    private ArrayPoolSegmentFactory _segFactory;

    private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private Stream _readStream;
    private Stream _writeStream;

    public void Init(Stream readStream, Stream writeStream, ArrayPoolSegmentFactory segmentFactory)
    {
      _readStream = readStream;
      _writeStream = writeStream;
      _segFactory = segmentFactory;
    }

    public async Task<CapnpNet.Message> ReceiveAsync()
    {
      var msg = new CapnpNet.Message().Init(null);
      var intBuf = ArrayPool<byte>.Shared.Rent(16);
      var bytesRead = await _readStream.ReadAsync(intBuf, 0, 4);
      if (bytesRead < intBuf.Length) throw new InvalidOperationException("Expected more data");

      var segmentCount = BitConverter.ToInt32(intBuf, 0) + 1;
      var readLen = segmentCount * 4 + (segmentCount % 2 == 0 ? 4 : 0);
      if (intBuf.Length < readLen)
      {
        ArrayPool<byte>.Shared.Return(intBuf);
        intBuf = ArrayPool<byte>.Shared.Rent(segmentCount * 4 + 4);
      }
      
      bytesRead = await _readStream.ReadAsync(intBuf, 0, readLen);
      if (bytesRead < intBuf.Length) throw new InvalidOperationException("Expected more data");
      
      for (int i = 0; i < segmentCount; i++)
      {
        var len = BitConverter.ToInt32(intBuf, i * 4) * 8;
        var seg = await _segFactory.CreateAsync(msg, len);
        seg.Is(out ArraySegment<byte> arrSeg);
        bytesRead = await _readStream.ReadAsync(arrSeg.Array, arrSeg.Offset, len);
        if (bytesRead < len) throw new InvalidOperationException("Expected more data");
      }

      ArrayPool<byte>.Shared.Return(intBuf);
      
      return msg;
    }

    public async Task WriteAsync(CapnpNet.Message msg)
    {
      await _semaphore.WaitAsync();
      try
      {
        await msg.SerializeAsync(_writeStream);
      }
      finally
      {
        _semaphore.Release();
      }

      msg.Dispose();
    }
    
    public void Dispose()
    {
      _readStream.Dispose();
      _writeStream.Dispose();
      _readStream = null;
      _writeStream = null;
    }
  }
}
