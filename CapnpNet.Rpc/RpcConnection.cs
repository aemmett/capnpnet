using Nito.AsyncEx;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CapnpNet.Rpc
{
  public class RpcConnection : IDisposable
  {
    // question/answer/export/import tables
    private MessageStream _msgStream;
    private ArrayPoolSegmentFactory _segFactory;
    private CancellationTokenSource _ctSource;
    private AsyncCountdownEvent _inFlightCounter;
    private ConcurrentQueue<System.Exception> _exceptions;

    public void Init(Stream readStream, Stream writeStream)
    {
      _msgStream = new MessageStream();
      _segFactory = new ArrayPoolSegmentFactory();
      _msgStream.Init(readStream, writeStream, _segFactory);
      _ctSource = new CancellationTokenSource();
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
        //if (msg.Is(out Message unimplemented))
        //{

        //}
        //else if (msg.Is(out Exception abort))
        //{

        //}
        //else if (msg.Is(out Bootstrap bootstrap))
        //{

        //}
        //else if (msg.Is(out Call call))
        //{

        //}
        //else if (msg.Is(out Return ret))
        //{

        //}
        //else if (msg.Is(out Finish finish))
        //{

        //}
        //else
        {
          // level -1 RPC
          var senderMsg = msg.GetStruct().Segment.Message;
          // TODO: copy data, or just move segments?
          var reply = new CapnpNet.Message().Init(_segFactory);
          await reply.PreAllocateAsync(senderMsg.CalculateSize() + 3);
          reply.Allocate(0, 1).WritePointer(0, new Message(reply)
          {
            which = Message.Union.unimplemented,
            unimplemented = msg.CopyTo(reply),
          });
          message.Dispose();
          message = null;
          await _msgStream.WriteAsync(reply);
        }
      }
      finally
      {
        if (message != null) message.Dispose();
      }
    }
    
    public void Dispose()
    {
      _msgStream.Dispose();
      _msgStream = null;
      _segFactory = null;
    }
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
