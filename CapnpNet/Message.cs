using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpNet
{
  public partial class Message : IDisposable
  {
    private ISegmentFactory _segmentFactory;

    // go back to List?
    private Segment _firstSegment, _lastSegment;
    
    public Message()
    {
    }
    
    public Message(ISegmentFactory segmentFactory)
    {
      _segmentFactory = segmentFactory;
    }

    public ISegmentFactory SegmentFactory => _segmentFactory;

    public SegmentList Segments => new SegmentList(_firstSegment);

    public long WordsToLive { get; set; }
    
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
    
    public T GetRoot<T>() where T : struct, IStruct => this.Root.As<T>();

    public Message Init(ISegmentFactory segmentFactory)
    {
      // throw on null?
      _segmentFactory = segmentFactory;
      this.WordsToLive = 64*1024*1024/8; // 64MB
      return this;
    }

    public Struct Allocate(ushort dataWords, ushort pointerWords)
    {
      this.Allocate(dataWords + pointerWords, out int offset, out Segment segment);
      return new Struct(segment, offset, dataWords, pointerWords, 0);
    }

    public void Allocate(int words, out int offset, out Segment segment)
    {
      if (_lastSegment.TryAllocate(words, out offset))
      {
        segment = _lastSegment;
      }

      segment = _segmentFactory.TryCreatePrompt(this, words);
      if (segment == null)
      {
        // TODO: different exception type
        throw new InvalidOperationException("Temporary allocation failure");
      }

      this.AddSegment(segment);
      
      if (segment.TryAllocate(words, out offset) == false)
      {
        throw new InvalidOperationException("Newly created segment was not big enough");
      }
    }

    public async Task CreateAndAddSegmentAsync(
      int? sizeHint,
      CancellationToken cancellationToken = default(CancellationToken))
    {
      var segment = await _segmentFactory.CreateAsync(this, sizeHint, cancellationToken);
      this.AddSegment(segment);
    }
    
    public void AddSegment(Segment segment)
    {
      var prevSegment = _lastSegment;
      _lastSegment = segment;
      segment.SegmentIndex = prevSegment?.SegmentIndex + 1 ?? 0;

      if (prevSegment == null)
      {
        _firstSegment = _lastSegment;
      }
      else
      {
        prevSegment.Next = _lastSegment;
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
            words = (pointer.ElementCount+63) / 64;
            break;
          case ElementSize.OneByte:
            words = (pointer.ElementCount+7) / 8;
            break;
          case ElementSize.TwoBytes:
            words = (pointer.ElementCount+3) / 4;
            break;
          case ElementSize.FourBytes:
            words = (pointer.ElementCount+1) / 2;
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
        // check !padFarPointer.IsDoubleFar
        segment = this.Segments[(int)landingPadPointer.TargetSegmentId];
        baseOffset = landingPadOffset;
        pointer = Unsafe.As<ulong, Pointer>(ref segment[landingPadOffset + 1 | Word.unit]);
        // check listPointer.WordOffset == 0
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
    }
  }
}