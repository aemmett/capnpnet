using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CapnpNet
{
  public sealed class PackStream : Stream
  {
    public enum CompressionMode
    {
      Decompress,
      Compress
    }
    
    private byte[] _leftovers;
    private int _tag;

    public PackStream(Stream stream, CompressionMode mode)
    {
      if (mode == CompressionMode.Compress
        && stream.CanWrite == false)
      {
        throw new ArgumentException("Stream must be writable to compress");
      }
      else if (mode == CompressionMode.Decompress
        && stream.CanRead == false)
      {
        throw new ArgumentException("Stream must be readable to compress");
      }

      this.BaseStream = stream;
      this.Mode = mode;
    }
    
    public Stream BaseStream { get; }

    public CompressionMode Mode { get; }

    public override bool CanRead => this.Mode == CompressionMode.Decompress;

    public override bool CanSeek => false;

    public override bool CanWrite => this.Mode == CompressionMode.Compress;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }

    public override void Flush()
    {
      throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (this.Mode != CompressionMode.Decompress) throw new InvalidOperationException("Must be in decompression mode");

      byte[] tagAndWord = new byte[9];
      int writeCount = 0;
      while (count > 0)
      {
        int readCount = this.BaseStream.Read(tagAndWord, 0, 9);
        if (readCount < 9) throw new NotImplementedException();

        byte tag = tagAndWord[0];

        void HandleByte(int o, int n)
        {
          bool c = (tag & 0x01) > 0;
          byte b = Unsafe.As<bool, byte>(ref c); // c ? 1 : 0
          buffer[o + n] = ((byte)-b) & tag;
        }

        this.BaseStream.
      }

      return writeCount;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
      throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this.Mode != CompressionMode.Compress) throw new InvalidOperationException("Must be in compression mode");

      int i;
      for (i = offset; i < count - offset - 7; i+=8)
      {
      }
    }
  }
}
