using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace CapnpNet
{
  public struct Text
  {
    private readonly Segment _segment;
    private readonly int _listWordOffset, _elementCount;
    
    public Text(Message msg, int bytesWithNulTerminator)
    {
      _elementCount = bytesWithNulTerminator;
      var words = (_elementCount + 7) / 8;
      msg.Allocate(words, out _listWordOffset, out _segment);
    }
    
    public Text(Message msg, string str)
    {
      _elementCount = Encoding.UTF8.GetByteCount(str) + 1;
      var words = (_elementCount + 7) / 8;
      msg.Allocate(words, out _listWordOffset, out _segment);
      if (_segment.Is(out ArraySegment<byte> arrSeg))
      {
        var writeOffset = arrSeg.Offset + _listWordOffset * sizeof(ulong);
        Encoding.UTF8.GetBytes(str, 0, str.Length, arrSeg.Array, writeOffset);
        arrSeg.Array[writeOffset + _elementCount - 1] = 0; // null terminator
      }
      else if (_segment.IsFixedMemory)
      {
        unsafe
        {
          fixed (char* chars = str)
          {
            var ptr = (byte*)Unsafe.AsPointer(ref _segment[_listWordOffset | Word.unit]);
            Encoding.UTF8.GetBytes(chars, str.Length, ptr, _elementCount - 1);
            ptr[_elementCount - 1] = 0;
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
      _segment = segment;
      _listWordOffset = baseWordOffset + listPointer.WordOffset;
      _elementCount = (int)listPointer.ElementCount;
    }
    
#if SPAN
    public Span<byte> Span => _elementCount == 0
      ? Span<byte>.Empty
      : _segment.Span.Slice(_listWordOffset).Cast<ulong, byte>().Slice(0, _elementCount);
#endif

    public Segment Segment => _segment;
    public int ListWordOffset => _listWordOffset;
    public int Count => _elementCount;

    public byte this[int index]
    {
#if SPAN
      get => this.Span[index];
#else
      get
      {
        Check.Range(index, _elementCount);

        return _segment[_listWordOffset * sizeof(ulong) + index | Byte.unit];
      }
#endif
    }

    public bool Is(out ArraySegment<byte> seg)
    {
      if (_segment.Is(out ArraySegment<byte> arrSeg))
      {
        seg = new ArraySegment<byte>(arrSeg.Array, arrSeg.Offset + _listWordOffset * sizeof(ulong), _elementCount - 1);
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