using CapnpNet.Rpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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

  public partial class Message : IDisposable
  {
    private ISegmentFactory _segmentFactory;

    // go back to List? pooled arrays?
    private Segment _firstSegment, _lastSegment;

    public Message()
    {
    }

    public ISegmentFactory SegmentFactory => _segmentFactory;

    public SegmentList Segments => new SegmentList(_firstSegment);

    public long WordsToLive { get; set; }

    public int TotalAllocated
    {
      get
      {
        int sum = 0;
        foreach (var seg in this.Segments)
        {
          sum += seg.AllocationIndex;
        }

        return sum;
      }
    }

    public int TotalCapcity
    {
      get
      {
        int sum = 0;
        foreach (var seg in this.Segments)
        {
          sum += seg.WordCapacity;
        }

        return sum;
      }
    }

    public Struct Root
    {
      get
      {
        var rootPointer = Unsafe.As<ulong, Pointer>(ref _firstSegment[0 | Word.unit]);
        return new Struct(_firstSegment, 1, (StructPointer)rootPointer);
      }
    }

    internal RefList<CapEntry> LocalCaps { get; set; }

    public T GetRoot<T>() where T : struct, IStruct => this.Root.As<T>();

    public int CalculateSize() => this.Root.CalculateSize();

    public Message Init(ISegmentFactory segmentFactory)
    {
      Check.NotNull(segmentFactory, nameof(segmentFactory));

      _segmentFactory = segmentFactory;
      this.WordsToLive = 64 * 1024 * 1024 / 8; // 64MB
      return this;
    }

    public Struct Allocate(ushort dataWords, ushort pointerWords)
    {
      this.Allocate(dataWords + pointerWords, out int offset, out Segment segment);
      return new Struct(segment, offset, dataWords, pointerWords);
    }

    public void Allocate(int words, out int offset, out Segment segment)
    {
      if (_lastSegment != null && _lastSegment.TryAllocate(words, out offset))
      {
        segment = _lastSegment;
        return;
      }

      segment = _segmentFactory.TryCreatePrompt(this, words);
      if (segment == null)
      {
        // TODO: different exception type
        throw new InvalidOperationException("Temporary allocation failure");
      }

      if (segment.TryAllocate(words, out offset) == false)
      {
        throw new InvalidOperationException("Newly created segment was not big enough");
      }
    }
    
    public Pointer GetCapPointer(ICapability cap)
    {
      uint index;
      for (index = 0; index < this.LocalCaps.Count; index++)
      {
        ref CapEntry entry = ref this.LocalCaps[index];
        if (entry.Capability == cap)
        {
          entry.RefCount++;
          return new OtherPointer
          {
            Type = PointerType.Other,
            OtherPointerType = OtherPointerType.Capability,
            CapabilityId = index
          };
        }
      }

      this.LocalCaps.Add(out index) = new CapEntry
      {
        Capability = cap,
        RefCount = 1
      };

      return new OtherPointer
      {
        Type = PointerType.Other,
        OtherPointerType = OtherPointerType.Capability,
        CapabilityId = index
      };
    }

    public void DecrementCapRefCount(uint capId)
    {
      // TODO: safety checks
      this.LocalCaps[capId].RefCount--;
    }

    public async Task CreateAndAddSegmentAsync(
      int? sizeHint,
      CancellationToken cancellationToken = default(CancellationToken))
    {
      await _segmentFactory.CreateAsync(this, sizeHint, cancellationToken);
    }

    public void AddSegment(Segment segment)
    {
      var seg = _firstSegment;
      while (seg != null)
      {
        if (seg == segment) throw new ArgumentException("Segment already added");

        seg = seg.Next;
      }

      var prevSegment = _lastSegment;
      _lastSegment = segment;
      segment.SegmentIndex = (prevSegment?.SegmentIndex + 1) ?? 0;

#if TRACE
      unsafe { Trace.WriteLine($"Adding segment {(long)Unsafe.AsPointer(ref segment[0 | Byte.unit]):X8}"); }
#endif

      if (prevSegment == null)
      {
        _firstSegment = _lastSegment;
      }
      else
      {
        prevSegment.Next = _lastSegment;
      }
    }

    public void PreAllocate(int words)
    {
      if (_lastSegment == null || _lastSegment.WordCapacity - _lastSegment.AllocationIndex < words)
      {
        var segment = _segmentFactory.TryCreatePrompt(this, words);
        if (segment == null) throw new InvalidOperationException("Temporary allocation failure"); // TODO: different type
      }
    }

    public async Task PreAllocateAsync(int words)
    {
      if (_lastSegment == null || _lastSegment.WordCapacity - _lastSegment.AllocationIndex < words)
      {
        await _segmentFactory.CreateAsync(this, words);
      }
    }

    public long CheckTraversalLimit(Pointer pointer)
    {
      long words = 1;
      if (pointer.Type == PointerType.Struct)
      {
        words = Math.Min(1, pointer.DataWords + pointer.PointerWords);
      }
      else if (pointer.Type == PointerType.List)
      {
        switch (pointer.ElementSize)
        {
          case ElementSize.OneBit:
            words = (pointer.ElementCount + 63) / 64;
            break;
          case ElementSize.OneByte:
            words = (pointer.ElementCount + 7) / 8;
            break;
          case ElementSize.TwoBytes:
            words = (pointer.ElementCount + 3) / 4;
            break;
          case ElementSize.FourBytes:
            words = (pointer.ElementCount + 1) / 2;
            break;
          case ElementSize.EightBytesNonPointer:
          case ElementSize.EightBytesPointer:
          case ElementSize.Composite:
            words = pointer.ElementCount;
            break;
        }
      }

      this.WordsToLive -= words;

      if (this.WordsToLive < 0) // TODO: option to disable exception
      {
        throw new InvalidOperationException("Traversal limit exceeded");
      }

      return words;
    }

    public bool Traverse(ref Pointer pointer, ref Segment segment, out int baseOffset)
    {
      if (pointer.Type != PointerType.Far)
      {
        this.CheckTraversalLimit(pointer);
        baseOffset = default(int);
        return false;
      }

      var farPointer = (FarPointer)pointer;
      segment = this.Segments[(int)farPointer.TargetSegmentId];

      var landingPadOffset = (int)farPointer.LandingPadOffset;
      var landingPadPointer = Unsafe.As<ulong, Pointer>(ref segment[landingPadOffset | Word.unit]);

      if (pointer.IsDoubleFar)
      {
        // the pointer is in another castle
        var padFarPointer = (FarPointer)landingPadPointer;
        Debug.Assert(padFarPointer.IsDoubleFar == false);
        pointer = Unsafe.As<ulong, Pointer>(ref segment[landingPadOffset + 1 | Word.unit]);
        segment = this.Segments[(int)padFarPointer.TargetSegmentId];
        baseOffset = (int)padFarPointer.LandingPadOffset;
        Debug.Assert(pointer.WordOffset == 0);
      }
      else
      {
        baseOffset = landingPadOffset + 1;
        pointer = landingPadPointer;
      }

      this.CheckTraversalLimit(pointer);
      return true;
    }

    public void Dispose()
    {
      var segment = _firstSegment;
      while (segment != null)
      {
        var nextSegment = segment.Next;
        segment.Dispose();
        segment = nextSegment;
      }

      _firstSegment = null;
      _lastSegment = null;
      _segmentFactory = null;
      this.LocalCaps = null;
    }

    internal struct CapEntry
    {
      public ICapability Capability;
      public int RefCount;
    }
  }
}
