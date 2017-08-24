using System;
using System.IO;
using System.Linq;
using CapnpNet.Test.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapnpNet.Schema.Tests
{
  [TestClass()]
  public class NodeDynamicProviderTests
  {
    [TestMethod()]
    public void NodeDynamicProviderTest()
    {
      Message msg = Message.Decode(Resources.schema);

      var cgr = msg.GetRoot<CodeGeneratorRequest>();

      var sc = new SchemaContainer();

      foreach (var node in cgr.nodes)
      {
        sc.Nodes.Add(node.id, new SchemaNode(sc, node));
      }
      
      SchemaNode GetNode(string name) => sc[cgr.nodes.First(n => n.displayName.ToString().EndsWith(name)).id];
      
      dynamic sut = msg.Root.AsDynamic(GetNode("CodeGeneratorRequest"));
      
      byte minor = sut.capnpVersion.minor;
      Console.WriteLine(minor);
    }
  }
}