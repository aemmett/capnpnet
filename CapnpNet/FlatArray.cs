using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CapnpNet
{
  public struct FlatArray<T> : IEnumerable<T>
    where T : struct
  {
    private readonly AbsPointer _pointer;
    
    public FlatArray(AbsPointer pointer)
    {
      _pointer = pointer;
      _pointer.Dereference();
      // TODO: verify correct size
    }

    public int Count
    {
      get
      {
        if (_pointer.Tag.ElementSize == ElementSize.Composite)
        {
          return Unsafe.As<ulong, StructPointer>(ref _pointer.Data).WordOffset;
        }
        else
        {
          return (int)_pointer.Tag.ElementCount;
        }
      }
    }

    public T this[int index]
    {
      get
      {
        Check.Range(index, this.Count);
        ref ulong listStart = ref _pointer.Data;
        if (_pointer.Tag.ElementSize == ElementSize.Composite)
        {
          // TODO: check if jit will inline, otherwise might need to make this branch a method
          var tag = Unsafe.As<ulong, StructPointer>(ref listStart);
          var wordOffset = 1 + index * (tag.DataWords + tag.PointerWords);
          ref ulong dataRef = ref Unsafe.Add(ref listStart, wordOffset);
          if (TypeHelpers.IsNumericPrimitive<T>())
          {
            return Unsafe.As<ulong, T>(ref dataRef);
          }
          else if (typeof(T) == typeof(AbsPointer))
          {
            // TODO: verify jit elides boxing
            return (T)(object)new AbsPointer(
              _pointer.Segment,
              _pointer.DataOffset + wordOffset,
              Unsafe.As<ulong, Pointer>(ref dataRef));
          }
          else
          {
            var s = new Struct(_pointer.Segment, _pointer.DataOffset + wordOffset, tag.DataWords, tag.PointerWords);
            return Unsafe.As<Struct, T>(ref s);
          }
        }
        else
        {
          if (TypeHelpers.IsNumericPrimitive<T>())
          {
            return Unsafe.Add(ref Unsafe.As<ulong, T>(ref listStart), index);
          }
          else if (typeof(T) == typeof(AbsPointer))
          {
            return (T)(object)new AbsPointer(
              _pointer.Segment,
              _pointer.DataOffset + index,
              Unsafe.As<ulong, Pointer>(ref Unsafe.Add(ref listStart, index)));
          }
          else
          {
            var wordOffset = _pointer.DataOffset + index * Unsafe.SizeOf<T>() / 8;
            sbyte byteRem = (sbyte)(index & 7);
            var dataWords = _pointer.Tag.ElementSize == ElementSize.EightBytesPointer
              ? (ushort)0
              : (ushort)1;
            var s = new Struct(_pointer.Segment, wordOffset, dataWords, (ushort)(1 - dataWords), byteRem);
            return Unsafe.As<Struct, T>(ref s);
          }
        }
      }
    }

    public Enumerator GetEnumerator() => new Enumerator(this);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public struct Enumerator : IEnumerator<T>
    {
      private FlatArray<T> _array;
      private int _index;
      private int _count;

      internal Enumerator(FlatArray<T> array)
      {
        _array = array;
        _index = -1;
        _count = array.Count;
      }

      public T Current => _array[_index];

      object IEnumerator.Current => this.Current;

      public void Dispose()
      {
        _array = default(FlatArray<T>);
      }

      public bool MoveNext()
      {
        _index++;
        return _index < _count;
      }

      public void Reset()
      {
        _index = -1;
      }
    }
  }
}