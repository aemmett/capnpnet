using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CapnpNet
{
  [StructLayout(LayoutKind.Sequential)]
  public struct Data : IPureAbsPointer
  {
    private readonly FlatArray<byte> _bytes;

    public Segment Segment => this.Pointer.Segment;

    public AbsPointer Pointer => _bytes.Pointer;

    public int Length => (int)_bytes.Pointer.Tag.ElementCount;

    public byte this[int index]
    {
      get
      {
        Check.Range(index, this.Length);
        return this.Segment[this.Pointer.DataOffset * sizeof(ulong) + index | Byte.unit];
      }
    }

    public bool Is(out ArraySegment<byte> seg) => _bytes.Is(out seg);

    public Stream GetStream()
    {
      if (this.Is(out ArraySegment<byte> arraySegment))
      {
        return new MemoryStream(
          arraySegment.Array,
          arraySegment.Offset,
          arraySegment.Count);
      }
      #if UNSAFE
      else if (this.Segment.Is(out SafeBuffer safeBuffer))
      {
        return new UnmanagedMemoryStream(
          safeBuffer,
          this.Pointer.DataOffset * sizeof(ulong),
          this.Length);
      }
      else if (this.Segment.Is(out (IntPtr offset, IntPtr length) fixedMemory))
      {
        unsafe
        {
          return new UnmanagedMemoryStream(
            (byte*)fixedMemory.offset,
            fixedMemory.length.ToInt64());
        }
      }
      #endif

      throw new NotSupportedException();
    }
  }
}
