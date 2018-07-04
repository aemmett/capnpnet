using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
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

  public sealed class ArraySlabSegmentFactory : ISegmentFactory
  {
    private sealed class Slab
    {
      private const int MinSegmentSizeBytes = 128;
      private const int DefaultSlabSize = 85000;

      private readonly byte[] _slab;
      private readonly GCHandle _handle;
      private readonly int _alignOffset;
      private readonly int _maxSegments;
      private readonly int _maxOrder;
      private readonly byte[] _allocTable;
      private readonly List<int> _orderOffsets;
      private readonly object _allocTableLock = new object();

      public Slab()
      {
        // allocate an array large enough to be Large Object Heap
        _slab = new byte[DefaultSlabSize];
      
        // pin so we can find a 64-bit-aligned offset
        _handle = GCHandle.Alloc(_slab, GCHandleType.Pinned);
        _alignOffset = 0;
      
        unsafe
        {
          fixed (byte* ptr = &_slab[0])
          {
            var lowThreeBits = (int)((IntPtr)ptr).ToInt64() & 0x7;
            if (lowThreeBits > 0)
            {
              _alignOffset += 8 - lowThreeBits;
            }
          }
        }

        // probably a smarter way to do all of this
        int maxBytes = _slab.Length - _alignOffset;
        _maxSegments = maxBytes / MinSegmentSizeBytes;
        _orderOffsets = new List<int>
        {
          0
        };
        
        var numNodes = _maxSegments;
        var generation = numNodes / 2;
        _maxOrder = 0;
        while (generation > 0)
        {
          _orderOffsets.Add(numNodes);
          numNodes += generation;
          generation /= 2;
          _maxOrder++;
        }
                
        _allocTable = new byte[numNodes];
      }

      public ArraySegment<byte> TryAllocate(int size)
      {
        Check.NonNegative(size);

        var order = (size + MinSegmentSizeBytes - 1) / MinSegmentSizeBytes;
        if (order > _maxOrder)
        {
          return default;
        }

        var startOffset = _orderOffsets[order];
        var endOffset = order == _orderOffsets.Count ? _allocTable.Length : _orderOffsets[order + 1];
        var orderCount = endOffset - startOffset;

        // consider: abandon the flat array approach and use some dynamic tree, try to make it more concurrent?
        lock (_allocTableLock)
        {
          int orderOffset;
          for (orderOffset = 0; orderOffset < orderCount; orderOffset++)
          {
            if (_allocTable[startOffset + orderOffset] == 0)
            {
              break;
            }
          }

          if (orderOffset == orderCount)
          {
            return default;
          }

          var order0Start = orderOffset << order;
          var order0Count = 1 << order;

          // _allocTable[startOffset + orderOffset] = 1;
          for (int o = 0; o < _maxOrder; o++)
          {
            var offset = order0Start >> o;
            var count = Math.Min(1, order0Count >> o);
            var allocTableStart = _orderOffsets[o];
            var allocTableEnd = Math.Min(
              allocTableStart + offset + count,
              o == _orderOffsets.Count ? _allocTable.Length : _orderOffsets[o + 1]);
            for (int i = allocTableStart; i < allocTableEnd; i++)
            {
              _allocTable[i] = 1;
            }
          }
        }
      }

      public void Return(ArraySegment<byte> seg)
      {
        if (seg.Array != _slab)
        {
          return;
        }


      }
    }

    public ValueTask<Segment> CreateAsync(Message msg, int? sizeHint = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Segment TryCreatePrompt(Message msg, int? sizeHint = null)
    {
      throw new NotImplementedException();
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