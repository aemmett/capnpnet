using System;
using System.Runtime.CompilerServices;

namespace CapnpNet
{
  public static class FrameDecoder
  {
    public static Message From(byte[] array)
    {
      if (array.Length <= 8) throw new ArgumentException("array too small", nameof(array));

      var msg = new Message();

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
  }
}