using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace CapnpNet.Schema.Compiler
{
  /*public sealed class CSharpCodeGenerator2
  {
    private readonly CodeGeneratorRequest _request;
    private readonly SyntaxGenerator _generator;
    private readonly SyntaxNode _capnpNet;
    private readonly SyntaxNode _iStruct;

    public CSharpCodeGenerator2(CodeGeneratorRequest request)
    {
      _request = request;
      var workspace = new AdhocWorkspace();
      _generator = SyntaxGenerator.GetGenerator(workspace, LanguageNames.CSharp);

      _capnpNet = _generator.IdentifierName("global::CapnpNet");
      _iStruct = _generator.QualifiedName(_capnpNet, _generator.IdentifierName("IStruct"));
    }

    public IReadOnlyDictionary<string, string> GenerateSources()
    {
      return _request.requestedFiles
        .Select(f => new
        {
          name = Path.GetFileNameWithoutExtension(f.filename.ToString()),
          content = this.GenerateFile(f).NormalizeWhitespace().ToString(),
        })
        .ToDictionary(
          x => x.name,
          x => x.content);
    }

    private Node this[ulong id] => _request.nodes.First(n => n.id == id);

    private SyntaxNode GenerateFile(CodeGeneratorRequest.RequestedFile file)
    {
      var node = this[file.id];
      Debug.Assert(node.which == Node.Union.file);

      string targetNamespace = "Schema"; // TODO

      return _generator.CompilationUnit(
        _generator.NamespaceDeclaration(
          targetNamespace,
          node.nestedNodes.Select(this.GenerateNode)));
    }

    private SyntaxNode GenerateNode(Node.NestedNode nestedNode)
    {
      return this.GenerateNode(nestedNode.name.ToString(), nestedNode.id);
    }

    private SyntaxNode GenerateNode(string name, ulong id)
    {
      var node = this[id];

      return node.which == Node.Union.@struct ? this.GenerateStruct(name, node)
        : node.which == Node.Union.@enum ? this.GenerateEnum(name, node)
        : node.which == Node.Union.@interface ? this.GenerateInterface(name, node)
        : node.which == Node.Union.@const ? this.GenerateConst(name, node)
        : node.which == Node.Union.annotation ? this.GenerateAnnotation(name, node)
        : null;
    }

    private SyntaxNode GenerateStruct(string name, Node node)
    {
      var s = node.@struct;

      var members = new List<SyntaxNode>
      {
        _generator.FieldDeclaration(
          "KNOWN_DATA_WORDS",
          _generator.TypeExpression(SpecialType.System_Int32),
          Accessibility.Public,
          DeclarationModifiers.Const,
          _generator.LiteralExpression(s.dataWordCount)),
        _generator.FieldDeclaration(
          "KNOWN_POINTER_WORDS",
          _generator.TypeExpression(SpecialType.System_Int32),
          Accessibility.Public,
          DeclarationModifiers.Const,
          _generator.LiteralExpression(s.pointerCount)),
        _generator.FieldDeclaration(
          "_s",
          _iStruct,
          Accessibility.Private),
        _generator.ConstructorDeclaration(
          name,
          new[] {
            _generator.ParameterDeclaration(
              "allocContext",
              this.CapnpNetType("AllocationContext"),
              refKind: RefKind.Ref)
          },
          Accessibility.Public,
          baseConstructorArguments: new[]
          {
            _generator.InvocationExpression(
              _generator.mem)
          })
      };

      members.AddRange(node.nestedNodes.Select(this.GenerateNode));
      
      return _generator.StructDeclaration(
        name,
        node.parameters.Select(p => p.name.ToString()),
        Accessibility.Public,
        DeclarationModifiers.Partial,
        new[] { _iStruct },
        members);
    }

    private SyntaxNode CapnpNetType(string name) => _generator.QualifiedName(_capnpNet, _generator.IdentifierName(name));
  }*/
}
