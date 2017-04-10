using System;
using System.Buffers;
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
    ValueTask<Segment> CreateAsync(Message msg, int? sizeHint = null, CancellationToken cancellationToken = default(CancellationToken));
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

    public ValueTask<Segment> CreateAsync(Message msg, int? sizeHint = default(int?), CancellationToken cancellationToken = default(CancellationToken))
    {
      // TODO: do we even bother checking for cancellation?
      cancellationToken.ThrowIfCancellationRequested();
      return new ValueTask<Segment>(this.TryCreatePrompt(msg, sizeHint));
    }

    public Segment TryCreatePrompt(Message msg, int? sizeHint = default(int?))
    {
      var buf = _pool.Rent(sizeHint * sizeof(ulong) ?? 4096); // TODO: tune

      // TODO: pooling
      var returner = new ArrayReturner();
      returner.Init(buf, this);
      
      var seg = new Segment();
      seg.Init(msg, new ArraySegment<byte>(buf), returner);
      seg.AllocationIndex = 0;

      return seg;
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