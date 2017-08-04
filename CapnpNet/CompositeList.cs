using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CapnpNet
{
  public struct CompositeList<T> : IEnumerable<T>
    where T : struct, IStruct
  {
    private readonly Segment _segment;
    private readonly int _tagOffset, _elementCount;
    private readonly ushort _dataWords, _pointerWords;
    
    public CompositeList(Message msg, int count, out AllocationContext allocContext)
    {
      var tag = new StructPointer
      {
        Type = PointerType.Struct,
        WordOffset = 0,
        DataWords = ReflectionCache<T>.KnownDataWords,
        PointerWords = ReflectionCache<T>.KnownPointerWords,
      };
      _elementCount = 0;
      _dataWords = tag.DataWords;
      _pointerWords = tag.PointerWords;
      var elementWords = count * (_dataWords + _pointerWords);
      msg.Allocate(elementWords + 1, out _tagOffset, out _segment);
      allocContext = new AllocationContext(_segment, _tagOffset + 1, elementWords);
      _segment[_tagOffset | Word.unit] = tag.RawValue;
    }

    public CompositeList(Message msg, StructPointer tag)
    {
      _elementCount = tag.WordOffset;
      _dataWords = tag.DataWords;
      _pointerWords = tag.PointerWords;
      msg.Allocate(_elementCount * (_dataWords + _pointerWords) + 1, out _tagOffset, out _segment);
      _segment[_tagOffset | Word.unit] = tag.RawValue;
    }

    public CompositeList(Segment segment, int baseOffset, ListPointer listPointer)
    {
      if (listPointer.ElementSize != ElementSize.Composite)
      {
        throw new NotSupportedException("Element size not composite (upgraded struct lists not fully supported)");
      }

      _segment = segment;
      _tagOffset = baseOffset + listPointer.WordOffset;
      var tag = Unsafe.As<ulong, StructPointer>(ref _segment[_tagOffset | Word.unit]);
      _elementCount = tag.WordOffset;
      _dataWords = tag.DataWords;
      _pointerWords = tag.PointerWords;
    }

    public T this[int index]
    {
      get
      {
        if (index < 0 || index >= _elementCount) throw new ArgumentOutOfRangeException();

        var elementSize = _dataWords + _pointerWords;
        return new T
        {
          Struct = new Struct(_segment, _tagOffset + 1 + elementSize * index, _dataWords, _pointerWords)
        };
      }
    }

    public Segment Segment => _segment;
    public int TagWordOffset => _tagOffset;
    public int ElementWordSize => _dataWords + _pointerWords;
    public int Count => _elementCount;
    
    public void Add(T element)
    {
      // TODO: only check in debug builds?
      var s = element.Struct;
      if (s.Segment != this.Segment)
      {
        throw new ArgumentException("Element not allocated in same segment");
      }
      
      // TODO: check element offset within list
      // also check offset at multiple of EWS?
      // TODO: modify _elementCount
    }

    public CompositeList<T> CopyTo(Message dest)
    {
      var ret = new CompositeList<T>(
        dest,
        new StructPointer
        {
          WordOffset = this.Count,
          DataWords = _dataWords,
          PointerWords = _pointerWords
        });
      
      // TODO: block copy method on Segment?
      ref ulong src = ref _segment[_tagOffset + 1 | Word.unit];
      ref ulong dst = ref ret._segment[ret._tagOffset + 1 | Word.unit];
      for (int i = 0; i < (_dataWords + _pointerWords) * _elementCount; i++)
      {
        Unsafe.Add(ref dst, i) = Unsafe.Add(ref src, i);
      }

      return ret;
    }

    public IEnumerator<T> GetEnumerator() // TODO: non-allocating enumerator
    {
      for (int i = 0; i < this.Count; i++)
      {
        yield return this[i];
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
  }
}