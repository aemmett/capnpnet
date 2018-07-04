using CapnpNet;
using CapnpNet.Rpc;
using System.IO;
using Xunit;

namespace CapnpNet.Tests
{
  public class SegmentTests
  {
    //private Segment CreateSegment()
    //{
    //  var msg = new Message();
    //  var seg = new Segment().Init(msg, new byte[4096]);

    //}

    public void RpcStrawman()
    {
      RpcConnection con = null;

      var service = con.GetMain<global::Schema.ITestInterface>();
      
      
    }

    [Fact]
    public void RpcTest()
    {
      var msg = new Message();
      msg.Init(new ArrayPoolSegmentFactory(), false);
      msg.PreAllocate(10);
      msg.Allocate(1); // HACK allocate root pointer after pre-alloc
      msg.SetRoot(new Rpc.Message(msg)
      {
        which = Rpc.Message.Union.join,
        join = new Join(msg)
        {
          questionId = 123
        }
      });
      
      Assert.Equal(
        new StructPointer
        {
          Type = PointerType.Struct,
          WordOffset = 0,
          DataWords = 1,
          PointerWords = 1
        }.ToString(),
        new Pointer(msg.Segments[0].GetWord(0)).ToString());

      //var ms = new MemoryStream();
      //msg.SerializeAsync(ms).Wait();

      //msg.Dispose();

      //ms.Position = 0;
      
      var ms2 = new MemoryStream();
      var rc = new RpcConnection();
      rc.Init(null, ms2, null);
      rc.Process(msg);

      ms2.Position = 0;
      var msg2 = Message.DecodeAsync(ms2).Result;
      var rpcMsg2 = msg2.GetRoot<Rpc.Message>();
      Assert.Equal(
        Rpc.Message.Union.unimplemented,
        rpcMsg2.which);
      Assert.Equal(
        Rpc.Message.Union.join,
        rpcMsg2.unimplemented.which);
      Assert.Equal(
        123U,
        rpcMsg2
          .unimplemented
          .join
          .questionId);
    }
  }
}