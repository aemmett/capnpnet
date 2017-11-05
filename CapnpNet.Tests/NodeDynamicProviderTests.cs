using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CapnpNet.Schema;
using Xunit;

namespace CapnpNet.Tests
{
  public class NodeDynamicProviderTests
  {
    [Fact]
    public async Task NodeDynamicProviderTest()
    {
      var schema = typeof(NodeDynamicProviderTests).GetTypeInfo().Assembly
        .GetManifestResourceStream("CapnpNet.Tests.schema.bin");

      Message msg = await Message.DecodeAsync(schema);

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