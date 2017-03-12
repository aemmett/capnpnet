using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CapnpNet
{
  public class Message : IDisposable
  {
    // TODO: readable vs. writable?
    // TODO: how is empty message represented?
    // TODO: incremental reads?
    private Segment _firstSegment;
    
    public Message()
    {
      this.WordsToLive = 64*1024*1024/8; // 64MB
    }
    
    public Message(Segment firstSegment)
      : this()
    {
      _firstSegment = firstSegment;
    }
    
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
    
    public T GetRoot<T>() where T : struct, IStruct
    {
      return new T
      {
        Struct = this.Root,
      };
    }

    public void AddSegment(ArraySegment<byte> memory) // TODO: read vs. write
    {
      var lastSegment = this.Segments.LastOrDefault();
      // TODO: pool
      var segment = new Segment(memory)
      {
        Message = this,
        SegmentIndex = lastSegment?.SegmentIndex + 1 ?? 0,
      };

      if (lastSegment == null)
      {
        _firstSegment = segment;
      }
      else
      {
        lastSegment.Next = segment;
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