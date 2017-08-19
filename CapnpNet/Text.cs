using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace CapnpNet
{
  [StructLayout(LayoutKind.Sequential)]
  public struct Text : IPureAbsPointer
  {
    private readonly FlatArray<byte> _bytes;
    
    public Text(Message msg, int bytesWithNulTerminator, out AllocationContext allocContext)
    {
      _bytes = new FlatArray<byte>(msg, bytesWithNulTerminator, out allocContext);
    }
    
    public Text(Message msg, string str)
    {
      var byteCount = Encoding.UTF8.GetByteCount(str) + 1;
      _bytes = new FlatArray<byte>(msg, byteCount, out AllocationContext allocContext);

      var absPointer = _bytes.Pointer;
      var segment = absPointer.Segment;
      if (segment.Is(out ArraySegment<byte> arrSeg))
      {
        var writeOffset = arrSeg.Offset + absPointer.DataOffset * sizeof(ulong);
        Encoding.UTF8.GetBytes(str, 0, str.Length, arrSeg.Array, writeOffset);
        arrSeg.Array[writeOffset + byteCount - 1] = 0; // null terminator
      }
      else if (segment.IsFixedMemory)
      {
        unsafe
        {
          fixed (char* chars = str)
          {
            var ptr = (byte*)Unsafe.AsPointer(ref absPointer.Data);
            Encoding.UTF8.GetBytes(chars, str.Length, ptr, byteCount - 1);
            ptr[byteCount - 1] = 0;
          }
        }
      }
      else
      {
        // TODO: ask for pin
        throw new NotImplementedException();
      }
    }

    public Text(Segment segment, int baseWordOffset, ListPointer listPointer)
    {
      TypeHelpers.AssertSize<byte>(listPointer.ElementSize);
      _bytes = new FlatArray<byte>(new AbsPointer(segment, baseWordOffset, listPointer));
    }
    
#if SPAN
    public Span<byte> Span => _elementCount == 0
      ? Span<byte>.Empty
      : _segment.Span.Slice(_listWordOffset).Cast<ulong, byte>().Slice(0, _elementCount);
#endif

    public Segment Segment => _bytes.Pointer.Segment;
    public int ListWordOffset => _bytes.Pointer.DataOffset;
    /// <summary>
    /// Number of encoded bytes, including null terminator.
    /// </summary>
    public int ByteLength => (int)_bytes.Pointer.Tag.ElementCount;

    public AbsPointer Pointer => _bytes.Pointer;

    public byte this[int index]
    {
#if SPAN
      get => this.Span[index];
#else
      get
      {
        Check.Range(index, this.ByteLength);

        return this.Segment[this.ListWordOffset * sizeof(ulong) + index | Byte.unit];
      }
#endif
    }

    /// <summary>
    /// Retrieves the segment of bytes encoding this Text, excluding the null terminator.
    /// </summary>
    /// <param name="seg">The <see cref="ArraySegment{T}"/>.</param>
    /// <returns><c>true</c> if Text is held in a <see cref="byte[]"/> array.</returns>
    public bool Is(out ArraySegment<byte> seg)
    {
      if (this.Segment.Is(out ArraySegment<byte> arrSeg))
      {
        seg = new ArraySegment<byte>(
          arrSeg.Array, 
          arrSeg.Offset + this.ListWordOffset * sizeof(ulong),
          this.ByteLength - 1);
        return true;
      }

      seg = new ArraySegment<byte>();
      return false;
    }

    public override string ToString()
    {
#if SPAN
      return this.Span.DecodeUTF8();
#else
      if (this.Is(out ArraySegment<byte> strBytes))
      {
        return Encoding.UTF8.GetString(strBytes.Array, strBytes.Offset, strBytes.Count);
      }

      throw new NotImplementedException();
#endif
    }
  }
}