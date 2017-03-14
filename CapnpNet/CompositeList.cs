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
    
    public CompositeList(Message msg, StructPointer tag, int count)
    {
      _elementCount = count;
      _dataWords = tag.DataWords;
      _pointerWords = tag.PointerWords;
      msg.Allocate(_dataWords + _pointerWords + 1, out _tagOffset, out _segment);
    }

    public CompositeList(Segment segment, int baseOffset, ListPointer listPointer)
    {
      // TODO: check pointer element size, may need to create upgraded structs
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
          Struct = new Struct(_segment, _tagOffset + 1 + elementSize * index, _dataWords, _pointerWords, 0)
        };
      }
    }

    public Segment Segment => _segment;
    public int TagWordOffset => _tagOffset;
    public int ElementWordSize => _dataWords + _pointerWords;
    public int Count => _elementCount;

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