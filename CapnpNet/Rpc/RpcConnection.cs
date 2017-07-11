using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
    private SortedList<uint, uint> _freeList;

    private uint _nextId;
    
    public ref T TryGet(uint key, out bool valid)
    {
      valid = key < _nextId && _freeList?.ContainsKey(key) == false;
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
      if (_freeList == null) _freeList = new SortedList<uint, uint>();

      _freeList.Add(key, key);
      return copy;
    }

    public ref T Next(out uint id)
    {
      if (_freeList?.Count > 0)
      {
        id = _freeList.Keys[0];
        _freeList.RemoveAt(0);
      }
      else
      {
        id = _nextId;
        _nextId++;
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
    private Dictionary<ICapability, uint> _capToExportId;

    public void Init(Stream readStream, Stream writeStream, ICapability bootstrapCapability)
    {
      _msgStream = new MessageStream();
      _segFactory = new ArrayPoolSegmentFactory();
      _msgStream.Init(readStream, writeStream, _segFactory);
      _ctSource = new CancellationTokenSource();
      _exceptions = new ConcurrentQueue<System.Exception>();
      _exports = new ExportTable<Export>();
      _bootstrapCap = bootstrapCapability;
      _capToExportId = new Dictionary<ICapability, uint>();
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
          var reply = new CapnpNet.Message().Init(_segFactory);
          //await reply.PreAllocateAsync(senderMsg.CalculateSize() + 3);
          reply.Allocate(0, 1).WritePointer(0, new Return(reply)
          {
            answerId = bootstrap.questionId,
            which = Return.Union.results,
            results = new Payload
            {
              content = new OtherPointer
              {
                Type = PointerType.Other,
                OtherPointerType = OtherPointerType.Capability,
                CapabilityId = 0
              },
              capTable = new CompositeList<CapDescriptor>(reply, 1, out AllocationContext allocContext)
              {
                new CapDescriptor(ref allocContext)
                {
                  which = CapDescriptor.Union.senderHosted,
                  senderHosted = this.ExportCap(_bootstrapCap)
                }
              }
            }
          });
          message.Dispose();
          message = null;
          await _msgStream.WriteAsync(reply);
          reply.Dispose();
        }
        else if (msg.Is(out Call call))
        {
          HandleCall();

          void HandleCall()
          {
            bool valid;
            ref Answer ans = ref _answers.TryGet(call.questionId, out valid);
            if (!valid)
            {
              // TODO: return exception message
            }

            if (call.target.which == MessageTarget.Union.importedCap)
            {
              ref Export export = ref _exports.TryGet(call.target.importedCap, out valid);
              if (!valid)
              {
                // TODO: return exception message
              }

              // TODO: imports

              export.capability.DispatchCall(
                call.interfaceId,
                call.methodId,
                new CallContext(call.questionId, call.@params.content, this));
            }
            else if (call.target.Is(out PromisedAnswer promisedAnswer))
            {
              throw new NotImplementedException();
            }
            else
            {
              throw new NotSupportedException("Unknown call target");
            }
          }
        }
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

    internal CompositeList<CapDescriptor> CreateCapDescList(CapnpNet.Message msg)
    {
      var caps = msg.LocalCaps;
      var count = 0;
      for (uint i = 0; i < caps.Count; i++)
      {
        count += caps[i].RefCount > 0 ? 1 : 0;
      }

      var list = new CompositeList<CapDescriptor>(msg, count, out var allocContext);
      
      for (uint i = 0; i < caps.Count; i++)
      {
        var cap = caps[i];
        var desc = new CapDescriptor(ref allocContext);

        if (cap.RefCount > 0)
        {
          desc.which = CapDescriptor.Union.senderHosted;
          desc.senderHosted = this.ExportCap(cap.Capability);
        }
        else
        {
          desc.which = CapDescriptor.Union.none;
        };

        list.Add(desc);
      }

      return list;
    }

    public void SendAndDispose(CapnpNet.Message msg)
    {
      async Task Run(CapnpNet.Message m)
      {
        await _msgStream.WriteAsync(m);
        m.Dispose();
      }

      this.Consume(Run(msg));
    }

    private uint ExportCap(ICapability cap)
    {
      if (_capToExportId.TryGetValue(cap, out uint id) == false)
      {
        ref Export export = ref _exports.Next(out id);
        export.capability = cap;
        export.refCount = 1;
        _capToExportId[cap] = id;
      }
      else
      {
        ref Export export = ref _exports.TryGet(id, out bool valid);
        Debug.Assert(valid);
        Debug.Assert(export.capability == cap);
        export.refCount++;
      }

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
    private readonly uint _questionId;
    private Struct _parameters;
    private readonly RpcConnection _rpc;

    public CallContext(uint questionId, Struct parameters, RpcConnection rpc)
    {
      _questionId = questionId;
      _parameters = parameters;
      _rpc = rpc;
    }

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
      
      await msg.PreAllocateAsync(3 + sizeHint ?? 5); // TODO: tune
      
      msg.Allocate(0, 1).WritePointer(0, new Message(msg)
      {
        @return = new Return(msg)
        {
          answerId = _questionId,
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
        content = returnPayload,
        capTable = _rpc.CreateCapDescList(this.Message)
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
}
