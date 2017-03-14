using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CapnpNet
{
  public partial class Message : IDisposable
  {
    // TODO: Segment pooling
    // move this factory to the Segment class?
    public static Func<int, Segment> DefaultSegmentFactory { get; set; }
      = minRequiredSize => new Segment(new ArraySegment<byte>(new byte[Math.Min(minRequiredSize, 4008)]));
    
    private Segment _firstSegment, _lastSegment;
    
    public Message()
    {
      this.WordsToLive = 64*1024*1024/8; // 64MB
      this.SegmentFactory = Message.DefaultSegmentFactory;
    }
    
    public Message(Segment firstSegment)
      : this()
    {
      _firstSegment = _lastSegment = firstSegment;
    }
    
    public Func<int, Segment> SegmentFactory { get; set; }

    public SegmentList Segments => new SegmentList(_firstSegment);

    public long WordsToLive { get; set; }

    public Struct Root
    {
      get
      {
        var rootPointer = Unsafe.As<ulong, Pointer>(ref _firstSegment[0 | Word.unit]);
        return new Struct(_firstSegment, 1, (StructPointer)rootPointer);
      }
    }

    public T GetRoot<T>() where T : struct, IStruct => this.Root.As<T>();

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
      
      this.AddSegment(this.SegmentFactory(words));
      
      if (_lastSegment.TryAllocate(words, out offset))
      {
        segment = _lastSegment;
      }

      throw new InvalidOperationException("Cannot allocate");
    }
    
    public void AddSegment(byte[] memory) => this.AddSegment(new ArraySegment<byte>(memory, 0, memory.Length));

    public void AddSegment(ArraySegment<byte> memory) => this.AddSegment(new Segment(memory));

    private void AddSegment(Segment segment)
    {
      var prevSegment = _lastSegment;
      _lastSegment = segment;
      segment.Message = this;
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
    }
  }
}