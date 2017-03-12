using System;
using System.Runtime.CompilerServices;

namespace CapnpNet
{
  public sealed class Word
  {
    public static readonly Index<Word> unit = new Index<Word>(1);

    private Word()
    {
    }
  }

  public sealed class Byte
  {
    public static readonly Index<Byte> unit = new Index<Byte>(1);

    private Byte()
    {
    }
  }

  public struct Index<T>
  {
    public int index;

    public Index(int i)
    {
      index = i;
    }

    public static implicit operator int(Index<T> i) => i.index;

    public static explicit operator Index<T>(int i) => new Index<T>(i);

    public static Index<T> operator |(int i, Index<T> unit) => new Index<T>(i);
  }

  public sealed class Segment
  {
    // TODO: support other hosts of memeory (other objects, native pointers)
    private ArraySegment<byte> _memory;

    public Segment(ArraySegment<byte> memory)
    {
      this.Init(memory);
    }

    public Message Message { get; set; }
    public int SegmentIndex { get; set; }
    public Segment Next { get; set; }

#if SPAN
    public Span<ulong> Span => new Span(_memory).Cast<byte, ulong>();
#endif
    public int Words => _memory.Count / sizeof(ulong);
    public int AllocationIndex { get; set; }

    internal ref ulong this[Index<Word> wordIndex] => ref Unsafe.As<byte, ulong>(ref _memory.Array[_memory.Offset + wordIndex * sizeof(ulong)]);
    internal ref byte this[Index<Byte> byteIndex] => ref _memory.Array[_memory.Offset + byteIndex];

    public void Init(byte[] memory) => this.Init(memory, 0, memory.Length);

    public void Init(byte[] memory, int offset, int length) => this.Init(new ArraySegment<byte>(memory, offset, length));

    public void Init(ArraySegment<byte> memory)
    {
      if (memory.Count % 8 != 0) throw new ArgumentException("Memory length must be a multiple of 8 bytes");

      _memory = memory;
    }

    public void Reset()
    {
      _memory = new ArraySegment<byte>();
    }

    public bool Is(out ArraySegment<byte> arraySegment)
    {
      arraySegment = _memory;
      return _memory.Array != null;
    }

    public bool TryAllocate(int words, out int offset)
    {
      if (this.AllocationIndex + words >= this.Words)
      {
        offset = -1;
        return false;
      }

      offset = this.AllocationIndex;
      this.AllocationIndex += words;
      return true;
    }
  }
}