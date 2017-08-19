using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CapnpNet
{
  public struct Void
  {
  }

  // TODO: a different root "pointer" type e.g. for capability interfaces?
  public interface IAbsPointer
  {
    AbsPointer Pointer { get; }
  }

  public interface IPureAbsPointer : IAbsPointer
  {
  }

  public static class FlatArrayExtensions
  {
    public static bool Is(this FlatArray<byte> flatArray, out ArraySegment<byte> arraySegment)
    {
      if (flatArray.Segment.Is(out arraySegment))
      {
        arraySegment = new ArraySegment<byte>(
          arraySegment.Array,
          arraySegment.Offset + flatArray.Pointer.DataOffset * sizeof(ulong),
          flatArray.Count);
        return true;
      }

      return false;
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct FlatArray<T> : IEnumerable<T>, IPureAbsPointer
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

    public FlatArray(Message msg, int count, out AllocationContext allocContext) : this()
    {
      var elementSize = this.GetElementSize();

      // TODO: handle composite
      if (elementSize == ElementSize.Composite)
      {
        int words = 1 + count * (ReflectionCache<T>.KnownDataWords + ReflectionCache<T>.KnownPointerWords);
        
        msg.Allocate(
          words,
          out var offset,
          out var segment);

        segment[offset | Word.unit] = new StructPointer
        {
          Type = PointerType.Struct,
          WordOffset = count,
          DataWords = ReflectionCache<T>.KnownDataWords,
          PointerWords = ReflectionCache<T>.KnownPointerWords,
        }.RawValue;

        allocContext = new AllocationContext(segment, offset + 1, words - 1);
      
        _pointer = new AbsPointer(
          segment,
          0,
          new ListPointer
          {
            Type = PointerType.List,
            WordOffset = offset,
            ElementSize = elementSize,
            ElementCount = (uint)words - 1
          });
      }
      else
      {
        int words = TypeHelpers.SizeOf(elementSize) * count / 8;
        msg.Allocate(
          words,
          out var offset,
          out var segment);

        allocContext = new AllocationContext(segment, offset, words);
      
        _pointer = new AbsPointer(
          segment,
          0,
          new ListPointer
          {
            Type = PointerType.List,
            WordOffset = offset,
            ElementSize = elementSize,
            ElementCount = (uint)count
          });
      }
    }

    private ElementSize GetElementSize()
    {
      if (ReflectionCache<T>.ImplementsIStruct)
      {
        return ReflectionCache<T>.PreferredElementSize;
      }
      else if (ReflectionCache<T>.ImplementsIAbsPointer)
      {
        return ElementSize.EightBytesPointer;
      }
      else if (typeof(T) == typeof(Void))
      {
        return ElementSize.Zero;
      }

      return TypeHelpers.ToElementSize<T>();
    }

    public AbsPointer Pointer => _pointer;
    
    public Segment Segment => this.Pointer.Segment;

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
          return default;
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
          else if (ReflectionCache<T>.ImplementsICapability)
          {
            var ptr = Unsafe.As<ulong, Pointer>(ref dataRef);
            Check.IsTrue(ptr.Type == PointerType.Other && ptr.OtherPointerType == OtherPointerType.Capability);
            var cap = _pointer.Segment.Message.LocalCaps[ptr.CapabilityId].Capability;
            return (T)cap;
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
          Pointer elem = Unsafe.As<ulong, Pointer>(ref Unsafe.Add(ref listStart, index));
          if (ReflectionCache<T>.ImplementsIPureAbsPointer)
          {
            var absPointer = new AbsPointer(
              _pointer.Segment,
              _pointer.DataOffset + index + 1,
              elem);
            return Unsafe.As<AbsPointer, T>(ref absPointer);
          }
          else if (ReflectionCache<T>.ImplementsICapability)
          {
            Check.IsTrue(elem.Type == PointerType.Other && elem.OtherPointerType == OtherPointerType.Capability);
            var cap = _pointer.Segment.Message.LocalCaps[elem.CapabilityId].Capability;
            return (T)cap;
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

    public void Add(T element)
    {
      // TODO: check if element is within list?
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
        _array = default;
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