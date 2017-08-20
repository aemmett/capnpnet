using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapnpNet.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CapnpNet.Schema.Tests
{
  [TestClass()]
  public class NodeDynamicProviderTests
  {
    [TestMethod()]
    public void NodeDynamicProviderTest()
    {
      Message msg;
      using (var file = File.OpenRead(@"schema.bin"))
      {
        msg = Message.DecodeAsync(file).Result;
      }

      var cgr = msg.GetRoot<CodeGeneratorRequest>();

      Node GetNode(ulong id) => cgr.nodes.First(n => n.id == id);

      var fileNode = GetNode(cgr.requestedFiles[0].id);
      var node = cgr.nodes
        .First(n => n.displayName.ToString().EndsWith("CapnpVersion"));

      dynamic sut = ((IStruct)cgr.capnpVersion).Struct.AsDynamic(new SchemaNode(node));
      
      byte minor = sut.minor;
      Console.WriteLine(minor);
    }
  }
}