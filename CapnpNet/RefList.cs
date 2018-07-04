using System;
using System.Collections.Generic;
using System.Collections;

namespace CapnpNet
{
  public sealed class RefList<T> : IEnumerable<T>
  {
    private static T _empty;

    private T[] _slots = new T[16]; // TODO: pool
    
    public uint Count { get; private set; }

    public ref T this[uint index]
    {
      get
      {
        if (index > this.Count) throw new ArgumentOutOfRangeException();

        return ref _slots[index];
      }
    }
    
    public ref T Add(out uint index)
    {
      if (this.Count >= _slots.Length)
      {
        Array.Resize(ref _slots, _slots.Length * 2);
      }
      
      index = this.Count;
      this.Count++;
      return ref _slots[index];
    }

    public ref T TryGet(uint index, out bool found)
    {
      if (index > this.Count)
      {
        found = false;
        return ref _empty;
      }

      found = true;
      return ref _slots[index];
    }

    public Enumerator GetEnumerator() => new Enumerator(this);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public struct Enumerator : IEnumerator<T>
    {
      private RefList<T> _list;
      private uint _index;

      public Enumerator(RefList<T> list)
      {
        _list = list;
        _index = 0xffffffff;
      }

      public ref T Current => ref _list[_index];

      T IEnumerator<T>.Current => this.Current;

      object IEnumerator.Current => this.Current;

      public void Dispose()
      {
        _list = null;
      }

      public bool MoveNext()
      {
        unchecked { _index++; }
        return _index < _list.Count;
      }

      public void Reset()
      {
        _index = 0xffffffff;
      }
    }
  }
}
