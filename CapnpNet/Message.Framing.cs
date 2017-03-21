using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpNet
{
  [StructLayout(LayoutKind.Explicit)]
  public struct Pack64B<T> where T : struct
  {
    [FieldOffset(0)]
    private byte _b0;
    [FieldOffset(63)]
    private byte _b63;

    public T this[int index]
    {
      get
      {
        Check.Range(index, 64 / Unsafe.SizeOf<T>());
        return Unsafe.Add(ref Unsafe.As<byte, T>(ref _b0), index);
      }
      set
      {
        Check.Range(index, 64 / Unsafe.SizeOf<T>());
        Unsafe.Add(ref Unsafe.As<byte, T>(ref _b0), index) = value;
      }
    }
  }

  public partial class Message
  {
    // TODO: packing
    public static async Task<Message> DecodeAsync(Stream s, CancellationToken ct = default(CancellationToken))
    {
      var msg = new Message().Init(null);
      var intBuf = new byte[4]; // pool scrach buffers?
      var bytesRead = await s.ReadAsync(intBuf, 0, intBuf.Length, ct);
      if (bytesRead < intBuf.Length) throw new InvalidOperationException("Expected more data");

      var segmentCount = BitConverter.ToInt32(intBuf, 0) + 1;
      intBuf = segmentCount != 1 ? new byte[segmentCount * 4] : intBuf;
      bytesRead = await s.ReadAsync(intBuf, 0, intBuf.Length, ct);
      if (bytesRead < intBuf.Length) throw new InvalidOperationException("Expected more data");

      for (int i = 0; i < segmentCount; i++)
      {
        var len = BitConverter.ToInt32(intBuf, i * 4) * 8;
        var array = new byte[len];
        bytesRead = await s.ReadAsync(array, 0, len);
        if (bytesRead < len) throw new InvalidOperationException("Expected more data");

        msg.AddSegment(array);
      }

      return msg;
    }

    public static Message Decode(byte[] array)
    {
      if (array.Length <= 8) throw new ArgumentException("array too small", nameof(array));

      var msg = new Message().Init(null);

      ref int ptr = ref Unsafe.As<byte, int>(ref array[0]);
      int segmentCount = ptr + 1;
      // make sure we don't overrrun arr
      if (array.Length < (segmentCount + 2) * 4) throw new ArgumentException("array too small", nameof(array));

      var startOffset = (segmentCount + 2) / 2 * 8;
      for (int i = 0; i < segmentCount; i++)
      {
        var len = Unsafe.Add(ref ptr, i + 1) * 8;
        msg.AddSegment(new ArraySegment<byte>(array, startOffset, len));
        startOffset += len;
      }

      return msg;
    }

    // TODO: get/rewrite framing header into byte[]

    public Task SerializeAsync(Stream dest)
    {
      var intBuf = new byte[(_lastSegment.SegmentIndex + 3) / 2 * 8];
      ref int intPtr = ref Unsafe.As<byte, int>(ref intBuf[0]);
      intPtr = _lastSegment.SegmentIndex;
      foreach (var seg in this.Segments)
      {
        Unsafe.Add(ref intPtr, seg.SegmentIndex + 1) = seg.AllocationIndex;
      }

      return SerializeAsync(dest, intBuf);
    }

    private async Task SerializeAsync(Stream dest, byte[] intBuf)
    {
      await dest.WriteAsync(intBuf, 0, intBuf.Length);
      foreach (var seg in this.Segments)
      {
        if (seg.Is(out ArraySegment<byte> arr))
        {
          await dest.WriteAsync(arr.Array, arr.Offset, seg.AllocationIndex * sizeof(ulong));
        }
        else if (seg.IsFixedMemory)
        {
          UnmanagedMemoryStream readStream;
          unsafe
          {
            readStream = new UnmanagedMemoryStream(
              (byte*)Unsafe.AsPointer(ref seg[0 | Byte.unit]),
              seg.AllocationIndex * sizeof(ulong));
          }

          using (readStream)
          {
            await readStream.CopyToAsync(dest);
          }
        }
        else
        {
          // TODO: ask for pin
          throw new NotImplementedException();
        }
      }
    }
  }
}