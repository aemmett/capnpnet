using System;

namespace CapnpNet
{
  // TODO: make generic, should always allocate the same size/shape object. Account for tightly encoded struct lists
  public struct AllocationContext
  {
    private int _nextOffset;

    public AllocationContext(Segment segment, int startOffset, int count)
    {
      this.Segment = segment;
      _nextOffset = startOffset;
      this.EndOffset = startOffset + count;
    }

    public Segment Segment { get; }
    public int NextOffset => _nextOffset;
    public int EndOffset { get; }
    
    public Struct Allocate(ushort dataWords, ushort pointerWords)
    {
      if (_nextOffset + dataWords + pointerWords > this.EndOffset)
      {
        throw new InvalidOperationException("");
      }

      var offset = _nextOffset;
      _nextOffset += dataWords + pointerWords;
      return new Struct(this.Segment, offset, dataWords, pointerWords);
    }
  }

  public struct AllocationContext<T>
  {
    private int _nextByteOffset;
    private int _count;

    public AllocationContext(Segment segment, int byteOffset, int count)
    {
      this.Segment = segment;
      _nextByteOffset = byteOffset;
      _count = count;
    }

    public Segment Segment { get; }


  }
}