using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CapnpNet
{
  [StructLayout(LayoutKind.Sequential)]
  public struct BoolList : IPureAbsPointer, IEnumerable<bool>
  {
    private readonly AbsPointer _pointer;

    public BoolList(Message msg, int count)
    {
      int words = (count + 63) / 64;
      msg.Allocate(words, out var offset, out var segment);
      _pointer = new AbsPointer(
        segment,
        0,
        new ListPointer
        {
          Type = PointerType.List,
          WordOffset = offset,
          ElementSize = ElementSize.OneBit,
          ElementCount = (uint)count
        });
    }

    public AbsPointer Pointer => _pointer;

    public Span<byte> Span => this.Segment.Span.Slice(this.ListWordOffset << 3, (this.Count + 7) / 8);

    public Segment Segment => _pointer.Segment;
    public int ListWordOffset => _pointer.DataOffset;
    public int Count => (int)_pointer.Tag.ElementCount;

    public bool this[int index]
    {
      get
      {
        if (index < 0 || index >= this.Count) throw new ArgumentOutOfRangeException("index");

        var mask = 1 << (index & 7);
        return (this.Span[index >> 3] & mask) > 0;
      }
      set
      {
        if (index < 0 || index >= this.Count) throw new ArgumentOutOfRangeException("index");

        var mask = 1 << (index & 7);
        ref byte span = ref this.Span[index >> 3];
        if (value)
        {
          span |= (byte)mask;
        }
        else
        {
          span &= (byte)~mask;
        }
      }
    }

    public BoolList CopyTo(Message dest)
    {
      var ret = new BoolList(dest, this.Count);
      this.Span.CopyTo(ret.Span);
      return ret;
    }

    // TODO: implement same APIs as Spans?

    public IEnumerator<bool> GetEnumerator()
    {
      for (int i = 0; i < this.Count; i++)
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