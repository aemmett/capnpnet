using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CapnpNet
{
  public struct PrimitiveList<T> : IEnumerable<T>
    where T : struct
  {
    public PrimitiveList(Message msg, StructPointer tag, int count)
    {
      msg.Allocate(count * Unsafe.SizeOf<T>() / sizeof(ulong), out int offset, out Segment seg);
      this.Segment = seg;
      this.ListWordOffset = offset;
      this.Count = count;
    }

    public PrimitiveList(Segment segment, int baseWordOffset, ListPointer listPointer)
    {
      TypeHelpers.AssertSize<T>(listPointer.ElementSize);
      this.Segment = segment;
      this.ListWordOffset = baseWordOffset + listPointer.WordOffset;
      this.Count = (int)listPointer.ElementCount;
    }

#if SPAN
    public Span<T> Span => _elementCount == 0
      ? Span<T>.Empty
      : _segment.Span.Slice(_listWordOffset).Cast<ulong, T>().Slice(0, _elementCount); 
#endif

    public Segment Segment { get; }
    public int ListWordOffset { get; }
    public int Count { get; }

#if SPAN
    public T this[int index] => this.Span[index]; 
#else
    public T this[int index]
    {
      get
      {
        Check.Range(index, this.Count);

        return Unsafe.Add(ref Unsafe.As<ulong, T>(ref this.Segment[this.ListWordOffset | Word.unit]), index);
      }
    }
#endif

    public IEnumerator<T> GetEnumerator()
    {
      for (int i = 0; i < this.Count; i++)
      {
        yield return this[i];
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    // TODO: mutation methods; probably a helper Items property (set Count then Items.Add)
    // collection initializers, do they save the reference to .Item? .ItemAdder mutable struct?

    //public IEnumerable<T> GetEnumerator()
    //{
    //  for (int i = 0; i < this.Count; i++)
    //  {
    //    yield return this[i];
    //  }
    //}
  }
}