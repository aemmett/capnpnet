using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CapnpNet
{
  public sealed class Segment
  {
    // TODO: other memory modes, or a unified representation (span-like pinnable + offset)
    private ArraySegment<byte> _array;
    private SafeBuffer _safeBuf;
    private IntPtr _offset;
    private IntPtr _length;

    public Segment(ArraySegment<byte> memory) => this.Init(memory);
    public Segment(SafeBuffer safeBuffer) => this.Init(safeBuffer);

    public Message Message { get; set; }
    public int SegmentIndex { get; set; }
    public Segment Next { get; set; }
    
    // TODO: conditionals for accessing other memory modes
#if SPAN
    public Span<ulong> Span => new Span(_memory).Cast<byte, ulong>();
#endif
    public int WordCapacity => _length.ToInt32() / sizeof(ulong);
    public int AllocationIndex { get; set; }

    public bool IsFixedMemory => _safeBuf != null;

    internal ref ulong this[Index<Word> wordIndex] => ref Unsafe.As<byte, ulong>(ref this[wordIndex * sizeof(ulong) | Byte.unit]);
    internal ref byte this[Index<Byte> byteIndex]
    {
      get
      {
        if (_array.Array != null) return ref _array.Array[_array.Offset + byteIndex];
        else
        {
          unsafe
          {
            Check.Range(byteIndex, _length.ToInt32());
            return ref Unsafe.AsRef<byte>((byte*)_offset.ToPointer() + byteIndex);
          }
        };
      }
    }

    public void Init(byte[] memory) => this.Init(memory, 0, memory.Length);

    public void Init(byte[] memory, int offset, int length) => this.Init(new ArraySegment<byte>(memory, offset, length));

    public void Init(ArraySegment<byte> memory)
    {
      if (memory.Count % 8 != 0) throw new ArgumentException("Memory length must be a multiple of 8 bytes");

      _array = memory;
      _length = (IntPtr)memory.Count;
    }

    public void Init(SafeBuffer safeBuffer)
    {
      if (safeBuffer.ByteLength % 8 != 0) throw new ArgumentException("Memory length must be a multiple of 8 bytes");

      _safeBuf = safeBuffer;
      _length = new IntPtr((long)safeBuffer.ByteLength);
      unsafe
      {
        byte* ptr = null;
        safeBuffer.AcquirePointer(ref ptr);
        _offset = new IntPtr(ptr);
      }
    }

    public void Reset()
    {
      if (_safeBuf != null)
      {
        _safeBuf.ReleasePointer();
      }

      _array = default(ArraySegment<byte>);
      _safeBuf = null;
      _offset = IntPtr.Zero;
      _length = IntPtr.Zero;
    }

    public bool Is(out ArraySegment<byte> arraySegment)
    {
      arraySegment = _array;
      return _array.Array != null;
    }

    public bool Is(out SafeBuffer safeBuffer)
    {
      safeBuffer = _safeBuf;
      return safeBuffer != null;
    }

    public bool TryAllocate(int words, out int offset)
    {
      if (this.AllocationIndex + words >= this.WordCapacity)
      {
        offset = -1;
        return false;
      }

      offset = this.AllocationIndex;
      this.AllocationIndex += words;
      return true;
    }

    public bool TryReclaim(int endWordIndex, int words)
    {
      if (this.AllocationIndex != endWordIndex) return false;

      this.AllocationIndex -= words;
      return true;
    }
  }
}