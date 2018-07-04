using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpNet
{
  // TODO: just use a Dispose delegate in Segment to allow disposal independent of Message? (there
  //       are rare situations where an "active" Segment may need to be disposed before it is
  //       attached to a Message)
  // TODO: require Message at segment construction/init time? (i.e. eliminate the above rare case)
  // TODO: disallow return of null in (Try)CreatePrompt, and instead opt for more explicit intent?
  //       Behavior flags?
  public interface ISegmentFactory // disposable?
  {
    /// <summary>
    /// Try to create (or reuse a pooled) Segment. This should not block, may return null if
    /// sufficient memory is temporarily unavailable, and may throw in other exceptional situations.
    /// </summary>
    /// <remarks>
    /// Although this method is allowed to return null, implementations should make an effort to come
    /// up with memory even if it has to allocate. CreateAsync exists for allocation back-pressure.
    /// </remarks>
    /// <param name="msg">Message this segment will belong to</param>
    /// <param name="sizeHint">Optional hint for how many words this segment should accommodate</param>
    /// <returns>A Segment, or null</returns>
    Segment TryCreatePrompt(Message msg, int? sizeHint = null);

    /// <summary>
    /// Create (or reuse a pooled) Segment. This may be asynchronous if sufficient memory is
    /// temporarily unavailable, and throw in other exceptional situations. However, the result if
    /// successful should always be non-null. Implementations may apply back-pressure to producers
    /// through this method.
    /// </summary>
    /// <param name="msg">Message this segment will belong to</param>
    /// <param name="sizeHint">Optional hint for how many words this segment should accommodate</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A ValueTask with a non-null Segment, or exception</returns>
    ValueTask<Segment> CreateAsync(Message msg, int? sizeHint = null, CancellationToken cancellationToken = default);
  }

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
      _slabs.Add(newSlab);
      return newSlab.TryRent(minBufferSize);
    }
    
    protected override void Dispose(bool disposing)
    {
      _slabs = null;
    }
  }

  public sealed class ArrayPoolSegmentFactory : ISegmentFactory
  {
    private ArrayPool<byte> _pool;

    public ArrayPoolSegmentFactory()
      : this(ArrayPool<byte>.Shared)
    {
    }

    public ArrayPoolSegmentFactory(ArrayPool<byte> pool)
    {
      _pool = pool;
    }

    public ValueTask<Segment> CreateAsync(Message msg, int? sizeHint = default, CancellationToken cancellationToken = default)
    {
      // TODO: do we even bother checking for cancellation?
      cancellationToken.ThrowIfCancellationRequested();
      return new ValueTask<Segment>(this.TryCreatePrompt(msg, sizeHint));
    }

    public Segment TryCreatePrompt(Message msg, int? sizeHint = default)
    {
      var buf = _pool.Rent(sizeHint * sizeof(ulong) ?? 4096); // TODO: tune

      // TODO: pooling
      var returner = new ArrayReturner();
      returner.Init(buf, this);
      
      var seg = new Segment();
      seg.Init(msg, buf, this.Return);
      seg.AllocationIndex = 0;

      return seg;
    }

    private void Return(Memory<byte> memory)
    {
      MemoryMarshal.TryGetArray<byte>(memory, out var segment);
      _pool.Return(segment.Array);
    }

    private sealed class ArrayReturner : IDisposable
    {
      private byte[] _buffer;
      private ArrayPoolSegmentFactory _factory;

      public void Init(byte[] buffer, ArrayPoolSegmentFactory factory)
      {
        _buffer = buffer;
        _factory = factory;
      }

      public void Dispose()
      {
        _factory._pool.Return(_buffer);
        _buffer = null;
        _factory = null;
      }
    }
  }
}