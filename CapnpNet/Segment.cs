using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CapnpNet
{
  public sealed class Segment : IDisposable
  {
    private Memory<byte> _memory;
    
    private IDisposable _disposer;
    
    public Message Message { get; private set; }
    public int SegmentIndex { get; set; }
    public Segment Next { get; set; }
    
    public Span<ulong> Span => MemoryMarshal.Cast<byte, ulong>(_memory.Span);

    public int WordCapacity => _memory.Length / sizeof(ulong);
    public int AllocationIndex { get; set; }
    
    // TODO: get rid of int wrappers and just have GetByteRef / GetWordRef methods.
    internal ref ulong this[Index<Word> wordIndex] => ref Unsafe.As<byte, ulong>(ref this[wordIndex * sizeof(ulong) | Byte.unit]);
    internal ref byte this[Index<Byte> byteIndex]
    {
      get
      {
        if (_array.Array != null) return ref _array.Array[_array.Offset + byteIndex];
        else
        {
#if UNSAFE
          unsafe
          {
            Check.Range(byteIndex, _byteLength);
            return ref Unsafe.AsRef<byte>((byte*)_fixedMemPointer.ToPointer() + byteIndex);
          } 
#else
          throw new NotSupportedException();
#endif
        };
      }
    }

    // consider: the managed pointers doesn't seem that useful over some generic Read<T>/Write<T>
    // methods that do the pointer arithmetic and reinterpret-casting internally. The best use case
    // for the managed pointer seems to be to avoid redundant bounds checks, but currently I haven't
    // built the rigor into the code to do the ahead-of-time bounds checks at construction.
    internal ref ulong GetWord(int i) => ref Unsafe.As<byte, ulong>(ref this.GetByte(i >> 3));
    internal ref byte GetByte(int i) => ref _memory.Span[i];

    public Segment Init(Message message, byte[] memory) => this.Init(message, memory, 0, memory.Length);

    public Segment Init(Message message, byte[] memory, int offset, int length) => this.Init(message, new ArraySegment<byte>(memory, offset, length));

    public Segment Init(Message message, ArraySegment<byte> memory, IDisposable disposer = null)
    {
      if (this.Message != null) throw new InvalidOperationException("Segment already initialized");

      if (memory.Count % 8 != 0) throw new ArgumentException("Memory length must be a multiple of 8 bytes");

      this.Message = message;
      message.AddSegment(this);
      _array = memory;
      _byteLength = memory.Count;
      _disposer = disposer;
      return this;
    }
    
    public Segment Init(Message message, SafeBuffer safeBuffer)
    {
      if (this.Message != null) throw new InvalidOperationException("Segment already initialized");

      if (safeBuffer.ByteLength % 8 != 0) throw new ArgumentException("Memory length must be a multiple of 8 bytes");

      this.Message = message;
      _byteLength = (int)safeBuffer.ByteLength;

      _fixedMemHandle = safeBuffer;
      unsafe
      {
        byte* ptr = null;
        try
        {
          // TODO: can we just use DangerousGetHandle?
          safeBuffer.AcquirePointer(ref ptr);
          _disposer = new SafeBufferReleaser(safeBuffer);
          _fixedMemPointer = new IntPtr(ptr);
          return this;
        }
        finally
        {
          if (ptr != null)
          {
            if (_disposer == null) _disposer.Dispose();
            else safeBuffer.ReleasePointer();
          }
        }
      }
    }

    public void Dispose()
    {
      _disposer?.Dispose();
      
      _disposer = null;
      _array = default;
      _fixedMemHandle = null;
      _fixedMemPointer = IntPtr.Zero;
      _byteLength = 0;
      this.Message = null;
      this.Next = null;
      this.AllocationIndex = 0;
    }
    
    public bool Is(out ArraySegment<byte> arraySegment)
    {
      return MemoryMarshal.TryGetArray(_memory, out arraySegment);
    }

    public bool Is(out SafeBuffer safeBuffer)
    {
      safeBuffer = _fixedMemHandle as SafeBuffer;
      return safeBuffer != null;
    }

    public bool Is(out (IntPtr offset, IntPtr length) fixedMemroy)
    {
      fixedMemroy = (_fixedMemPointer, (IntPtr)_byteLength);
      return this.IsFixedMemory;
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