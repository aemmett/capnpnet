using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using static Xunit.Assert;

namespace CapnpNet.Tests
{
  public sealed class PackStreamTests
  {
    private static readonly byte[] Packed = new byte[] {
      0x51, 0x08, 0x03, 0x02, 0x31, 0x19, 0xaa, 0x01,
      0x00, 0x02,
      0xff, 1, 2, 3, 4, 5, 6, 7, 8,
      0x02, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16
    };

    private static readonly byte[] Unpacked = new byte[]
    {
      0x08, 0x00, 0x00, 0x00, 0x03, 0x00, 0x02, 0x00, 0x19, 0x00, 0x00, 0x00, 0xaa, 0x01, 0x00, 0x00
    }.Concat(new byte[8*3])
    .Concat(new byte[]
    {
      1, 2, 3, 4, 5, 6, 7, 8,
      1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16
    }).ToArray();

    private readonly ITestOutputHelper _output;

    public PackStreamTests(ITestOutputHelper output)
    {
      _output = output;
    }

    [Fact]
    public void Pack()
    {
      using (var ms = new MemoryStream(Packed))
      {
        var ps = new PackStream(ms, PackStream.CompressionMode.Decompress);
        var buf = new byte[1024];
        Equal(Unpacked.Length, ps.Read(buf, 0, buf.Length));
        Equal(Unpacked, buf.Take(Unpacked.Length));
      }
    }

    [Fact]
    public void Unpack()
    {
      using (var ms = new MemoryStream())
      {
        var ps = new PackStream(ms, PackStream.CompressionMode.Compress);
        ps.Write(Unpacked, 0, Unpacked.Length);
        ps.Dispose();
        var buf = ms.ToArray();
        
        _output.WriteLine(string.Join(" ", buf.Select(b => b.ToString("X2"))));
        Equal(Packed, buf);
      } 
    }
  }
}
