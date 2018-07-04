using CapnpNet.Rpc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpNet
{
  // consider struct Message<TRoot> wrapper?
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
        // TOOD: follow far pointer?
        var rootPointer = Unsafe.As<ulong, Pointer>(ref _firstSegment.GetWord(0));
        return new Struct(_firstSegment, 1, (StructPointer)rootPointer);
      }
      set
      {
        new Struct(_firstSegment, 0, 0, 1).WritePointer(0, value);
      }
    }

    internal RefList<CapEntry> LocalCaps { get; set; }

    public T GetRoot<T>() where T : struct, IStruct => this.Root.As<T>();
    public void SetRoot<T>(T @struct) where T : struct, IStruct => this.Root = @struct.Struct;

    public int CalculateSize() => this.Root.CalculateSize();

    public Message Init(ISegmentFactory segmentFactory, bool allocateRootPointer)
    {
      Check.NotNull(segmentFactory, nameof(segmentFactory));

      _segmentFactory = segmentFactory;
      this.WordsToLive = 64 * 1024 * 1024 / 8; // 64MB

      if (allocateRootPointer) this.Allocate(1); // root pointer

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

    public AllocationContext Allocate(int words)
    {
      this.Allocate(words, out int offset, out var segment);
      return new AllocationContext(segment, offset, words);
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
      CancellationToken cancellationToken = default)
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

#if TRACE && UNSAFE
      unsafe { Trace.WriteLine($"Adding segment {(long)Unsafe.AsPointer(ref segment.GetByte(0)):X8}"); }
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
        baseOffset = default;
        return false;
      }

      var farPointer = (FarPointer)pointer;
      segment = this.Segments[(int)farPointer.TargetSegmentId];

      var landingPadOffset = (int)farPointer.LandingPadOffset;
      var landingPadPointer = Unsafe.As<ulong, Pointer>(ref segment.GetWord(landingPadOffset));

      if (pointer.IsDoubleFar)
      {
        // the pointer is in another castle
        var padFarPointer = (FarPointer)landingPadPointer;
        Debug.Assert(padFarPointer.IsDoubleFar == false);
        pointer = Unsafe.As<ulong, Pointer>(ref segment.GetWord(landingPadOffset));
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
