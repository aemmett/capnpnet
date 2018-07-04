using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CapnpNet
{
  public struct PrimitiveList<T> : IEnumerable<T>
    where T : struct
  {
    public PrimitiveList(Message msg, int count)
    {
      var divisor = sizeof(ulong) / Unsafe.SizeOf<T>();
      msg.Allocate((count + divisor - 1) / divisor, out int offset, out Segment seg);
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
    
    public Span<T> Span => this.Count == 0
      ? Span<T>.Empty
      : MemoryMarshal.Cast<ulong, T>(this.Segment.Span.Slice(this.ListWordOffset)).Slice(0, this.Count); 

    public Segment Segment { get; }
    public int ListWordOffset { get; }
    public int Count { get; }
    
    public T this[int index] => this.Span[index];

    public PrimitiveList<T> CopyTo(Message dest)
    {
      var ret = new PrimitiveList<T>(dest, this.Count);
      
      ref ulong src = ref this.Segment.GetWord(this.ListWordOffset);
      ref ulong dst = ref ret.Segment.GetWord(ret.ListWordOffset );
      for (int i = 0; i < this.Count * Unsafe.SizeOf<T>() / sizeof(ulong); i++)
      {
        Unsafe.Add(ref dst, i) = Unsafe.Add(ref src, i);
      }

      return ret;
    }

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