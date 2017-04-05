using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public void Init(Stream stream)
    {
      _msgStream = new MessageStream();
      _segFactory = new ArrayPoolSegmentFactory();
      _msgStream.Init(stream, _segFactory);
    }

    public async Task StartReadLoop()
    {
      while (true)
      {
        using (var msg = await _msgStream.ReceiveAsync())
        {
          var rpcMsg = msg.GetRoot<Message>();
          this.Process(rpcMsg);
        }
      }
    }

    private async Task Process(Message msg)
    {
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
        reply.AddSegment(await _segFactory.CreateAsync(reply, senderMsg.TotalCapcity));
        reply.Allocate(0, 1).WritePointer(0, new Message(reply)
        {
          which = Message.Union.unimplemented,
          unimplemented = msg.CopyTo(reply),
        });
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
    private Stream _stream;

    public void Init(Stream stream, ArrayPoolSegmentFactory segmentFactory)
    {
      _stream = stream;
      _segFactory = segmentFactory;
    }

    public async Task<CapnpNet.Message> ReceiveAsync()
    {
      var msg = new CapnpNet.Message().Init(null);
      var intBuf = ArrayPool<byte>.Shared.Rent(16);
      var bytesRead = await _stream.ReadAsync(intBuf, 0, 4);
      if (bytesRead < intBuf.Length) throw new InvalidOperationException("Expected more data");

      var segmentCount = BitConverter.ToInt32(intBuf, 0) + 1;
      var readLen = segmentCount * 4 + (segmentCount % 2 == 0 ? 4 : 0);
      if (intBuf.Length < readLen)
      {
        ArrayPool<byte>.Shared.Return(intBuf);
        intBuf = ArrayPool<byte>.Shared.Rent(segmentCount * 4 + 4);
      }
      
      bytesRead = await _stream.ReadAsync(intBuf, 0, readLen);
      if (bytesRead < intBuf.Length) throw new InvalidOperationException("Expected more data");
      
      for (int i = 0; i < segmentCount; i++)
      {
        var len = BitConverter.ToInt32(intBuf, i * 4) * 8;
        var seg = await _segFactory.CreateAsync(msg, len);
        seg.Is(out ArraySegment<byte> arrSeg);
        bytesRead = await _stream.ReadAsync(arrSeg.Array, arrSeg.Offset, len);
        if (bytesRead < len) throw new InvalidOperationException("Expected more data");

        msg.AddSegment(seg);
      }

      ArrayPool<byte>.Shared.Return(intBuf);
      
      return msg;
    }

    public async Task WriteAsync(CapnpNet.Message msg)
    {
      await _semaphore.WaitAsync();
      try
      {
        await msg.SerializeAsync(_stream);
      }
      finally
      {
        _semaphore.Release();
      }

      msg.Dispose();
    }
    
    public void Dispose()
    {
      _stream.Dispose();
      _stream = null;
    }
  }
}
