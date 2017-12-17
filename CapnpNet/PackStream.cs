using System;
using System.Buffers;
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
      
      // TODO: buffer reads from BaseStream to reduce number of virtual calls
      byte[] tagAndWord = new byte[9];
      var inbuf = new byte[8];
      var outbuf = new byte[8];
      ulong word;

      int originalOffset = offset;
      while (count > 0)
      {
        int tag = this.BaseStream.ReadByte();
        if (tag == -1) break;
        
        // https://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetKernighan
        int tagCopy = tag;
        int numBitsSet;
        for (numBitsSet = 0; tagCopy != 0; numBitsSet++)
        {
          tagCopy &= tagCopy - 1;
        }
    
        int ib = 0;
        this.BaseStream.Read(inbuf, 0, numBitsSet); // TODO: reliable read
    
        byte c = (byte)(tag & 1);
        word = ((ulong)-c) & inbuf[ib];
        ib += c;
        tag >>= 1;
        c = (byte)(tag & 1);
        word |= ((ulong)-c) & ((ulong)inbuf[ib] << 8);
        ib += c;
        tag >>= 1;
        c = (byte)(tag & 1);
        word |= ((ulong)-c) & ((ulong)inbuf[ib] << 16);
        ib += c;
        tag >>= 1;
        c = (byte)(tag & 1);
        word |= ((ulong)-c) & ((ulong)inbuf[ib] << 24);
        ib += c;
        tag >>= 1;
        c = (byte)(tag & 1);
        word |= ((ulong)-c) & ((ulong)inbuf[ib] << 32);
        ib += c;
        tag >>= 1;
        c = (byte)(tag & 1);
        word |= ((ulong)-c) & ((ulong)inbuf[ib] << 40);
        ib += c;
        tag >>= 1;
        c = (byte)(tag & 1);
        word |= ((ulong)-c) & ((ulong)inbuf[ib] << 48);
        ib += c;
        tag >>= 1;
        c = (byte)(tag & 1);
        word |= ((ulong)-c) & ((ulong)inbuf[ib] << 56);
    
        Unsafe.As<byte, ulong>(ref buffer[offset]) = word;
        offset += 8;
        count -= 8;
    
        if (numBitsSet == 0)
        {
          Array.Clear(outbuf, 0, 8);
          int numZeroWords = this.BaseStream.ReadByte();
          if (numZeroWords == -1) throw new InvalidOperationException();
      
          for (int i = 0; i < numZeroWords; i++)
          {
            Unsafe.As<byte, ulong>(ref buffer[offset]) = 0;
            offset += 8;
            count -= 8;
          }
        }
        else if (numBitsSet == 8)
        {
          int numWordsToCopy = this.BaseStream.ReadByte();
          if (numWordsToCopy == -1) throw new InvalidOperationException();
      
          for (int i = 0; i < numWordsToCopy; i++)
          {
            this.BaseStream.Read(inbuf, 0, 8);
            Array.Copy(inbuf, 0, buffer, offset, 8);
            offset += 8;
            count -= 8;
          }
        }
      }

      return offset - originalOffset;
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

      Check.Range(offset, count, buffer.Length);

      if (count % 8 != 0) throw new ArgumentException("Expected count to be a multiple of 8");
      
      var buf = ArrayPool<byte>.Shared.Rent(9);

      int i = offset;
      int lastWordOffset = count - offset - 7;
      while (i < lastWordOffset)
      {
        ref byte tag = ref buf[0];
        int numBytesToWrite = 1;
        for (var j = 0; j < 8; j++)
        {
          byte b = buffer[i] != 0 ? (byte)1 : (byte)0;
          tag = (byte)((tag >> 1) | (b << 7));
          Unsafe.Add(ref tag, numBytesToWrite) = buffer[i];
          i++;
          numBytesToWrite += b;
        }

        this.BaseStream.Write(buf, 0, numBytesToWrite);

        if (tag == 0)
        {
          var numZeroWords = 0;
          while (Unsafe.As<byte, ulong>(ref buffer[i]) == 0 && i < lastWordOffset && numZeroWords <= 255)
          {
            numZeroWords++;
            i += 8;
          }

          this.BaseStream.WriteByte((byte)numZeroWords);
        }
        else if (tag == 0xff)
        {
          var numLiteralWords = 0;

          var startOffset = i;
          while (i < lastWordOffset)
          {
            var numZeroBytes = 0
              + buffer[i + 0] == 0 ? 1 : 0
              + buffer[i + 1] == 0 ? 1 : 0
              + buffer[i + 2] == 0 ? 1 : 0
              + buffer[i + 3] == 0 ? 1 : 0
              + buffer[i + 4] == 0 ? 1 : 0
              + buffer[i + 5] == 0 ? 1 : 0
              + buffer[i + 6] == 0 ? 1 : 0
              + buffer[i + 7] == 0 ? 1 : 0;

            if (numZeroBytes > 2)
            {
              break;
            }

            numLiteralWords++;
            i += 8;
          }

          this.BaseStream.WriteByte((byte)numLiteralWords);
          this.BaseStream.Write(buffer, startOffset, numLiteralWords * sizeof(ulong));
        }
      }
    }
  }
}
