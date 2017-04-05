using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CapnpNet
{
  public struct BoolList : IEnumerable<bool>
  {
    private readonly Segment _segment;
    private readonly int _listWordOffset, _count;
    
    public BoolList(Message msg, int count)
    {
      _count = count;
      msg.Allocate((count + 63) / 64, out _listWordOffset, out _segment);
    }

    public BoolList(Segment segment, int baseWordOffset, ListPointer listPointer)
    {
      if (listPointer.ElementSize != ElementSize.OneBit) throw new ArgumentException("Expected list pointer with ElementSize of OneBit");

      _segment = segment;
      _listWordOffset = baseWordOffset + listPointer.WordOffset;
      _count = (int)listPointer.ElementCount;
    }

#if SPAN
    public Span<ulong> Span => _segment.Span.Slice(_listWordOffset, (_count + 63) / 64); 
#endif

    public Segment Segment => _segment;
    public int ListWordOffset => _listWordOffset;
    public int Count => _count;

    public bool this[int index]
    {
      get
      {
        if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException("index");

        var mask = 1UL << (index & 63);
#if SPAN
        return (this.Span[index >> 6] & mask) > 0; 
#else
        return (_segment[index >> 6 | Word.unit] & mask) > 0;
#endif
      }
      set
      {
        if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException("index");

        var mask = 1UL << (index & 63);
#if SPAN
        var span = this.Span[index >> 6]; 
        if (value)
        {
          span |= mask;
        }
        else
        {
          span &= ~mask;
        }
#else
        ref var word = ref _segment[index >> 6 | Word.unit];
        if (value)
        {
          word |= mask;
        }
        else
        {
          word &= ~mask;
        }
#endif
      }
    }

    public BoolList CopyTo(Message dest)
    {
      var ret = new BoolList(dest, this.Count);
      
      ref ulong src = ref _segment[_listWordOffset | Word.unit];
      ref ulong dst = ref ret._segment[ret._listWordOffset | Word.unit];
      for (int i = 0; i < (_count + 63) / 64; i++)
      {
        Unsafe.Add(ref dst, i) = Unsafe.Add(ref src, i);
      }

      return ret;
    }

    // TODO: implement same APIs as Spans?

    public IEnumerator<bool> GetEnumerator()
    {
      for (int i = 0; i < _count; i++)
      {
        yield return this[i];
      }
    }
    
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /*
    void ICollection<bool>.CopyTo(bool[] array, int arrayIndex)
    {
      for (int i = 0; i < _elements; i++)
      {
        // could unroll, if we cared to
        array[arrayIndex] = this[i];
        arrayIndex++;
      }
    }

    bool ICollection<bool>.IsReadOnly { get { return false; } }
    IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

    void ICollection<bool>.Add(bool item) { throw new NotSupportedException(); }
    void ICollection<bool>.Clear() { throw new NotSupportedException(); }
    bool ICollection<bool>.Contains(bool item) { throw new NotSupportedException(); }
    int IList<bool>.IndexOf(bool item) { throw new NotSupportedException(); }
    void IList<bool>.Insert(int index, bool item) { throw new NotSupportedException(); }
    bool ICollection<bool>.Remove(bool item) { throw new NotSupportedException(); }
    void IList<bool>.RemoveAt(int index) { throw new NotSupportedException(); }
    */
  }
}