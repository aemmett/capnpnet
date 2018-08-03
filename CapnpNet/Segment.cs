using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CapnpNet
{
  public sealed class Segment : IDisposable
  {
    private Memory<byte> _memory;
    
    private Action<Memory<byte>> _disposer;
    
    public Message Message { get; private set; }
    public int SegmentIndex { get; set; }
    public Segment Next { get; set; }
    
    public Span<byte> Span => _memory.Span;

    public Memory<byte> Memory => _memory;

    public int WordCapacity => _memory.Length / sizeof(ulong);
    public int AllocationIndex { get; set; }
    
    // consider: the managed pointers doesn't seem that useful over some generic Read<T>/Write<T>
    // methods that do the pointer arithmetic and reinterpret-casting internally. The best use case
    // for the managed pointer seems to be to avoid redundant bounds checks, but currently I haven't
    // built the rigor into the code to do the ahead-of-time bounds checks at construction.
    internal ref ulong GetWord(int i) => ref Unsafe.As<byte, ulong>(ref this.GetByte(i * sizeof(ulong)));
    internal ref byte GetByte(int i) => ref _memory.Span[i];

    // TODO: remove all the lifecycle-related public methods, move that responsibility into ISegmentFactory
    public Segment Init(Message message, byte[] memory, Action<Memory<byte>> disposer = null)
      => this.Init(message, new Memory<byte>(memory), disposer);

    public Segment Init(Message message, byte[] memory, int offset, int length, Action<Memory<byte>> disposer = null)
      => this.Init(message, new Memory<byte>(memory, offset, length), disposer);

    public Segment Init(Message message, ArraySegment<byte> memory, Action<Memory<byte>> disposer = null)
      => this.Init(message, memory.Array, memory.Offset, memory.Count, disposer);

    public Segment Init(Message message, Memory<byte> memory, Action<Memory<byte>> disposer = null)
    {
      Check.NotNull(message, nameof(message));
      if (memory.Length % 8 != 0) throw new ArgumentException("Memory length must be a multiple of 8 bytes");

      _memory = memory;
      this.Message = message;
      message.AddSegment(this);
      _disposer = disposer;
      return this;
    }
    
    public void Dispose()
    {
      _disposer?.Invoke(_memory);
      
      _memory = default;
      _disposer = null;
      this.Message = null;
      this.Next = null;
      this.AllocationIndex = 0;
    }
    
    public bool Is(out ArraySegment<byte> arraySegment)
    {
      return MemoryMarshal.TryGetArray(_memory, out arraySegment);
    }
    
    public bool TryAllocate(int words, out int offset)
    {
      if (this.AllocationIndex + words > this.WordCapacity)
      {
        offset = -1;
        return false;
      }

      offset = this.AllocationIndex;
      this.AllocationIndex += words;
      return true;
    }

    public bool TryReclaim(int endWordIndex, int words)
    {
      if (this.AllocationIndex != endWordIndex) return false;

      this.AllocationIndex -= words;
      return true;
    }
  }
}