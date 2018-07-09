using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CapnpNet.Schema;
using Xunit;
using Xunit.Abstractions;

namespace CapnpNet.Tests
{
  public class NodeDynamicProviderTests
  {
    private readonly ITestOutputHelper _output;

    public NodeDynamicProviderTests(ITestOutputHelper output)
    {
      _output = output;
    }

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
      _output.WriteLine(minor.ToString());

      _output.WriteLine(
        string.Join(", ",
          (sut as IDynamicMetaObjectProvider)
            .GetMetaObject(Expression.Parameter(typeof(DynamicStruct)))
            .GetDynamicMemberNames()));
    }
  }
}