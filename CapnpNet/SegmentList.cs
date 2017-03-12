using System.Collections;
using System.Collections.Generic;

namespace CapnpNet
{
  public struct SegmentList : IReadOnlyList<Segment>
  {
    private readonly Segment _firstSegment;

    public SegmentList(Segment firstSegment)
    {
      _firstSegment = firstSegment;
    }

    public Segment this[int index]
    {
      get
      {
        var segment = _firstSegment;
        while (index > 0)
        {
          index--;
          segment = segment.Next;
        }

        return segment;
      }
    }

    public int Count
    {
      get
      {
        int count = 0;
        var segment = _firstSegment;
        while (segment != null)
        {
          count++;
          segment = segment.Next;
        }

        return count;
      }
    }
    
    public IEnumerator<Segment> GetEnumerator()
    {
      var segment = _firstSegment;
      while (segment != null)
      {
        yield return segment;
        segment = segment.Next;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}