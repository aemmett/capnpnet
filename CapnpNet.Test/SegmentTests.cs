using CapnpNet;
using CapnpNet.Rpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace CapnpNet.Test
{
  [TestClass]
  public class SegmentTests
  {
    //private Segment CreateSegment()
    //{
    //  var msg = new Message();
    //  var seg = new Segment().Init(msg, new byte[4096]);

    //}

    [TestMethod]
    public void RpcTest()
    {
      var msg = new Message();
      msg.Init(new ArrayPoolSegmentFactory());
      msg.PreAllocate(10);
      msg.Allocate(0, 1).WritePointer(0, new Rpc.Message(msg)
      {
        which = Rpc.Message.Union.join,
        join = new Join(msg)
        {
          questionId = 123
        }
      });
      
      Assert.AreEqual(
        msg.Segments[0][0 | Word.unit],
        new StructPointer
        {
          Type = PointerType.Struct,
          WordOffset = 0,
          DataWords = 1,
          PointerWords = 1
        }.RawValue);

      //var ms = new MemoryStream();
      //msg.SerializeAsync(ms).Wait();

      //msg.Dispose();

      //ms.Position = 0;
      
      var ms2 = new MemoryStream();
      var rc = new RpcConnection();
      rc.Init(null, ms2, null);
      rc.ProcessAsync(msg).Wait();

      ms2.Position = 0;
      var msg2 = Message.DecodeAsync(ms2).Result;
      var rpcMsg2 = msg2.GetRoot<Rpc.Message>();
      Assert.AreEqual(
        Rpc.Message.Union.unimplemented,
        rpcMsg2.which);
      Assert.AreEqual(
        Rpc.Message.Union.join,
        rpcMsg2.unimplemented.which);
      Assert.AreEqual(
        123,
        rpcMsg2
          .unimplemented
          .join
          .questionId);
    }
  }
}