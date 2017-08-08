using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CapnpNet
{
  public struct Void
  {
  }

  public interface IAbsPointer
  {
    AbsPointer Pointer { get; }
  }

  public interface IPureAbsPointer : IAbsPointer
  {
  }

  public struct FlatArray<T> : IEnumerable<T>, IPureAbsPointer
    where T : struct
  {
    private readonly AbsPointer _pointer;
    
    public FlatArray(AbsPointer pointer)
      : this()
    {
      _pointer = pointer;
      _pointer.Dereference();
      // TODO: assert pointer type
      // TODO: verify correct size
    }
    
    public AbsPointer Pointer => _pointer;

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
        if (typeof(T) == typeof(Void))
        {
          return default(T);
        }
        else if (typeof(T) == typeof(bool))
        {
          var mask = 1UL << (index & 63);
          return (T)(object)((_pointer.Segment[index >> 6 | Word.unit] & mask) > 0);
        }

        ref ulong listStart = ref _pointer.Data;
        if (_pointer.Tag.ElementSize == ElementSize.Composite)
        {
          var tag = Unsafe.As<ulong, StructPointer>(ref listStart);
          var wordOffset = 1 + index * (tag.DataWords + tag.PointerWords);
          ref ulong dataRef = ref Unsafe.Add(ref listStart, wordOffset);
          if (TypeHelpers.IsNumericPrimitive<T>())
          {
            return Unsafe.As<ulong, T>(ref dataRef);
          }
          else if (ReflectionCache<T>.ImplementsIStruct)
          {
            var s = new Struct(_pointer.Segment, _pointer.DataOffset + wordOffset, tag.DataWords, tag.PointerWords);
            return Unsafe.As<Struct, T>(ref s);
          }
          else if (ReflectionCache<T>.ImplementsIPureAbsPointer)
          {
            var p = new AbsPointer(
              _pointer.Segment,
              _pointer.DataOffset + wordOffset + 1,
              Unsafe.As<ulong, Pointer>(ref dataRef));
            return Unsafe.As<AbsPointer, T>(ref p);
          }
          else
          {
            // TOOD: Text, Data (once implemented)
            throw new NotImplementedException();
          }
        }
        else if (ReflectionCache<T>.ImplementsIStruct)
        {
          var wordOffset = _pointer.DataOffset + index * _pointer.Tag.ElementSize.SizeOf() / 8;
          sbyte byteRem = (sbyte)(index & 7);
          var dataWords = _pointer.Tag.ElementSize == ElementSize.EightBytesPointer
            ? (ushort)0
            : (ushort)1;
          var s = new Struct(_pointer.Segment, wordOffset, dataWords, (ushort)(1 - dataWords), byteRem);
          return Unsafe.As<Struct, T>(ref s);
        }
        else if (_pointer.Tag.ElementSize == ElementSize.EightBytesPointer)
        {
          if (ReflectionCache<T>.ImplementsIPureAbsPointer)
          {
            var absPointer = new AbsPointer(
              _pointer.Segment,
              _pointer.DataOffset + index + 1,
              Unsafe.As<ulong, Pointer>(ref Unsafe.Add(ref listStart, index)));
            return Unsafe.As<AbsPointer, T>(ref absPointer);
          }
          else
          {
            throw new NotImplementedException();
          }
        }
        else
        {
          if (TypeHelpers.IsNumericPrimitive<T>())
          {
            return Unsafe.Add(ref Unsafe.As<ulong, T>(ref listStart), index);
          }
          else
          {
            throw new NotImplementedException();
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