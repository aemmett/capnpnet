using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CapnpNet
{
  public sealed class ArraySlabMemoryPool : MemoryPool<byte>
  {
    private const int DefaultSlabSize = 85000;

    private ImmutableStack<Slab> _slabs = ImmutableStack<Slab>.Empty;

    public override int MaxBufferSize { get; } = (int)Math.Pow(2, Math.Floor(Math.Log(DefaultSlabSize) / Math.Log(2)));

    private sealed class Segment : IMemoryOwner<byte>
    {
      private Slab _slab;

      public Segment(Slab slab, Memory<byte> memory)
      {
        _slab = slab;
        this.Memory = memory;
      }

      public Memory<byte> Memory { get; internal set; }

      public void Dispose()
      {
        _slab.Return(this);
        _slab = null;
        this.Memory = null;
      }
    }

    private sealed class Slab
    {
      private const int MinSegmentSizeBytes = 128; // (16 words)

      private enum SegmentState : byte
      {
        Joined = 0,
        Free = 1,
        Split = 2,
        Allocated = 3
      }

      private byte[] _slab;
      private GCHandle _handle;
      private int _alignOffset;
      private SegmentState[][] _segments;
      private object _segmentsLock;

      public void Init()
      {
        // allocate an array large enough to be Large Object Heap
        _slab = new byte[DefaultSlabSize];
      
        // pin so we can find a 64-bit-aligned offset
        _handle = GCHandle.Alloc(_slab, GCHandleType.Pinned);        
        _alignOffset = (int)((_handle.AddrOfPinnedObject().ToInt64() + 7) & ~7);
      
        int maxBytes = _slab.Length - _alignOffset;        
        var remaining = maxBytes / MinSegmentSizeBytes;
        var generations = new List<SegmentState[]>();
        while (remaining > 0)
        {
          var order = new SegmentState[remaining];
          if ((remaining & 1) == 1)
          {
            order[remaining - 1] = SegmentState.Free;
          }

          generations.Add(order);
          remaining >>= 1;
        }

        generations.Reverse();
        _segments = generations.ToArray();
      }

      public IMemoryOwner<byte> TryRent(int size)
      {
        Check.NonNegative(size);

        if (_slab == null)
        {
          this.Init();
        }

        var (level, orderSize) = this.GetOrderStats(size);

        if (level >= _segments.Length)
        {
          return null;
        }

        var order = _segments[level];
        lock (_segmentsLock)
        {
          var joinedIndex = -1;
          for (var i = 0; i < order.Length; i++)
          {
            // might be able to check for free with just interlocked?
            if (order[i] == SegmentState.Free)
            {
              order[i] = SegmentState.Allocated;
              return new Segment(this, MemoryMarshal.CreateFromPinnedArray(_slab, _alignOffset + i * orderSize, orderSize));
            }
            else if (order[i] == SegmentState.Joined)
            {
              joinedIndex = i;
            }
          }

          if (joinedIndex == -1)
          {
            return null;
          }

          var ret = new Segment(this, MemoryMarshal.CreateFromPinnedArray(_slab, _alignOffset + joinedIndex * orderSize, orderSize));

          _segments[level][joinedIndex ^ 1] = SegmentState.Free;
          _segments[level][joinedIndex] = SegmentState.Allocated;

          do
          {
            level--;
            joinedIndex >>= 1;
            _segments[level][joinedIndex ^ 1] = SegmentState.Free;
            _segments[level][joinedIndex] = SegmentState.Split;
          }
          while (level > 0);

          return ret;
        }
      }

      public void Return(Segment segment)
      {
        if (MemoryMarshal.TryGetArray<byte>(segment.Memory, out var arraySegment) == false
          || arraySegment.Array != _slab)
        {
          Debug.Assert(false);
          return;
        }

        var (orderNum, _) = this.GetOrderStats(arraySegment.Count);
        var offset = (arraySegment.Offset - _alignOffset) >> orderNum;

        lock (_segmentsLock)
        {
          while (orderNum > 0)
          {
            var order = _segments[orderNum];
            if ((offset & 1) == 1 && offset == order.Length - 1)
            {
              order[offset] = SegmentState.Free;
              return;
            }

            // consider: don't immediately join?
            if (order[offset ^ 1] == SegmentState.Free)
            {
              order[offset ^ 1] = SegmentState.Joined;
              order[offset] = SegmentState.Joined;
              offset >>= 1;
              orderNum--;
              continue;
            }
            else
            {
              order[offset] = SegmentState.Free;
              return;
            }
          }
        }
      }

      private (int orderNum, int orderSize) GetOrderStats(int size)
      {
        var orderNum = 0;
        var orderSize = MinSegmentSizeBytes;
        while (size > orderSize && orderNum < _segments.Length)
        {
          orderNum++;
          orderSize <<= 1;
        }

        return (orderNum, orderSize);
      }
    }

    public override IMemoryOwner<byte> Rent(int minBufferSize = -1)
    {
      if (_slabs == null)
      {
        throw new ObjectDisposedException(nameof(ArraySlabMemoryPool));
      }

      if (minBufferSize > this.MaxBufferSize)
      {
        throw new ArgumentOutOfRangeException(nameof(minBufferSize));
      }

      if (minBufferSize < 0)
      {
        minBufferSize = 4096;
      }

      foreach (var slab in _slabs)
      {
        var segment = slab.TryRent(minBufferSize);
        if (segment != null)
        {
          return segment;
        }
      }

      var newSlab = new Slab();
      _slabs = _slabs.Push(newSlab);
      return newSlab.TryRent(minBufferSize);
    }
    
    protected override void Dispose(bool disposing)
    {
      _slabs = null;
    }
  }
}