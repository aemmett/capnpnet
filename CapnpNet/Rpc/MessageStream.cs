using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpNet.Rpc
{
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
      var msg = new CapnpNet.Message().Init(null, false);
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
