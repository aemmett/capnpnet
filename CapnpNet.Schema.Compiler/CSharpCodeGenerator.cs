using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CapnpNet.Schema.Compiler
{
  // TOOD: generate AST from FormattableString?
  public class CSharpCodeGenerator
  {
    // TODO: annotations for namespace, name overrides, and doc comments
    public const ulong NamespaceAnnotationId = ~0UL; // TODO

    // TODO: although it is auto-generated, I would like to remove excess qualification...
    public const string StructType = "global::CapnpNet.Struct";
    public const string MessageType = "global::CapnpNet.Message";

    private readonly CodeGeneratorRequest _request;
    private readonly SyntaxGenerator _generator;

    private Stack<string> _nodePath = new Stack<string>();

    public CSharpCodeGenerator(CodeGeneratorRequest request)
      : this(request, "Schema")
    {
    }

    public CSharpCodeGenerator(CodeGeneratorRequest request, string defaultNamespace)
    {
      _request = request;
      this.DefaultNamespace = defaultNamespace;
      _generator = SyntaxGenerator.GetGenerator(new AdhocWorkspace(), LanguageNames.CSharp);
      
    }

    public IEnumerable<string> NodePath => _nodePath;

    public string DefaultNamespace { get; set; }
    public OptionKey CSharpFormattingOption { get; private set; }

    public IReadOnlyDictionary<string, string> GenerateSources()
    {
      return _request.requestedFiles
        .Select(f => new
        {
          name = Path.GetFileNameWithoutExtension(f.filename.ToString()) + ".designer",
          content = this.GenerateContent(f),
        })
        .ToDictionary(
          x => x.name,
          x => x.content);
    }

    private string GenerateContent(CodeGeneratorRequest.RequestedFile file)
    {
      var node = _request.nodes.First(n => n.id == file.id);

      if (node.which != Node.Union.file) throw new InvalidOperationException("Expected file node");

      var src = $@"using CapnpNet;

namespace {GetNamespace(node)}
{{
  {string.Join("\n", GenerateTypesUnder(node))}
}}
";
      var tree = SyntaxFactory.ParseSyntaxTree(src);
      var root = tree.GetRoot()
        .NormalizeWhitespace("  ", false);
      return new WhitespaceRewriter().Visit(root).ToString();
    }

    private class WhitespaceRewriter : CSharpSyntaxRewriter
    {
      public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
      {
        node = node.ReplaceNode(node.ParameterList, node.ParameterList.WithTrailingTrivia(SyntaxFactory.Space));
        return base.VisitConstructorDeclaration(node);
      }

      public override SyntaxNode VisitBlock(BlockSyntax node)
      {
        if (node.Statements.Count == 1
          /*&& (node.Parent is ConstructorDeclarationSyntax
            || node.Parent is AccessorDeclarationSyntax
            || node.Parent is MethodDeclarationSyntax)*/)
        {
          node = node.ReplaceToken(node.OpenBraceToken, node.OpenBraceToken.WithoutTrivia());
          node = node.ReplaceToken(node.CloseBraceToken, node.CloseBraceToken
            .WithLeadingTrivia()
            .WithTrailingTrivia(node.CloseBraceToken.TrailingTrivia.Last()));
          node = node.ReplaceNode(node.Statements[0], node.Statements[0]
            .WithLeadingTrivia(SyntaxFactory.Space)
            .WithTrailingTrivia(SyntaxFactory.Space));
        }

        return node;
      }

      public override SyntaxNode VisitAccessorDeclaration(AccessorDeclarationSyntax node)
      {
        node = node.ReplaceToken(node.Keyword, node.Keyword
          .WithLeadingTrivia(node.Keyword.LeadingTrivia.First())
          .WithTrailingTrivia(SyntaxFactory.Space));
        return base.VisitAccessorDeclaration(node);
      }
    }

    private IEnumerable<string> GenerateTypesUnder(Node parent)
    {
      foreach (var node in _request.nodes.Where(n => n.scopeId == parent.id
          && (n.which != Node.Union.@struct || n.@struct.isGroup == false)))
      {
        _nodePath.Push(node.displayName.ToString());
        yield return GenerateType(node);
        _nodePath.Pop();
      }
    }

    private string GenerateType(Node node)
    {
      var name = _generator.IdentifierName(node.displayName.ToString().Substring((int)node.displayNamePrefixLength)).ToString();

      if (node.Is(out Node.Struct s))
      {
        return $@"
          {GetDocComment(node)}
          public struct {name} : IStruct
          {{
            private {StructType} _s;
            public {name}({MessageType} m) : this(m, {s.dataWordCount.ToString()}, {s.pointerCount.ToString()}) {{ }}
            public {name}({MessageType} m, ushort dataWords, ushort pointers) : this(m.Allocate(dataWors, pointers)) {{ }}
            public {name}({StructType} s) {{ _s = s; }}
            {StructType} IStruct.Struct {{ get {{ return _s; }} set {{ _s = value; }} }}

            {string.Join("\n", GenerateMembers(s))}

            {string.Join("\n", GenerateTypesUnder(node))}
          }}";
      }
      else if (node.Is(out Node.enumGroup @enum))
      {
        return $@"
          {GetDocComment(node)}
          public enum {name} : ushort
          {{
            {string.Join(",\n", @enum.enumerants
              .Select((e, i) => new { e.codeOrder, ordinal = i, enumerant = e })
              .OrderBy(e => e.codeOrder)
              .Select(e => $@"
                {GetDocComment(e.enumerant.annotations)}
                {ToName(e.enumerant.name)} = {e.ordinal}"))}
          }}";
      }
      else if (node.Is(out Node.interfaceGroup i))
      {
        return $@"
          {this.GetDocComment(node)}
          public interface {name} {this.GetSupertypes(i.extends)}
          {{
            {string.Join("\n", GenerateMethods(i.methods))}
          }}";
      }
      else if (node.Is(out Node.constGroup c))
      {
        this.GetTypeInfo(c.type, out var type, out var accessor);
        return $@"
          {this.GetDocComment(node)}
          public const {type} {name} = {this.GetDefaultLiteral(c.value, false)};";
      }
      else
      {
        throw new InvalidOperationException($"Unexpected node: {node.which}");
      }
    }

    private IEnumerable<string> GenerateMethods(CompositeList<Method> methods)
    {
      var ordinal = 0;
      foreach (var method in methods.OrderBy(m => m.codeOrder))
      {
        _nodePath.Push(method.name.ToString());
        yield return $@"
          {GetDocComment(method.annotations)}
          [Ordinal({ordinal})]
          {this.GetTypeName(method.resultStructType)} {_generator.IdentifierName(method.name.ToString())}";
        ordinal++;
        _nodePath.Pop();
      }
    }

    private string GetSupertypes(PrimitiveList<ulong> extends)
    {
      var names = string.Join(", ", extends.Select(this.GetTypeName));
      return string.IsNullOrEmpty(names) ? string.Empty : " : " + names;
    }

    private IEnumerable<string> GenerateMembers(ulong typeId)
    {
      return this.GenerateMembers(_request.nodes.First(n => n.id == typeId && n.which == Node.Union.@struct).@struct);
    }

    private void GetTypeInfo(Type t, out string type, out string accessor)
    {
      switch (t.which)
      {
        //case Type.Union.@void: type = null; accessor = null; return false;
        case Type.Union.@bool: type = "bool"; accessor = "Bool"; break;
        case Type.Union.int8: type = "sbyte"; accessor = "Int8"; break;
        case Type.Union.int16: type = "short"; accessor = "Int16"; break;
        case Type.Union.int32: type = "int"; accessor = "Int32"; break;
        case Type.Union.int64: type = "long"; accessor = "Int64"; break;
        case Type.Union.uint8: type = "byte"; accessor = "UInt8"; break;
        case Type.Union.uint16: type = "ushort"; accessor = "UInt16"; break;
        case Type.Union.uint32: type = "uint"; accessor = "UInt32"; break;
        case Type.Union.uint64: type = "ulong"; accessor = "UInt64"; break;
        case Type.Union.float32: type = "float"; accessor = "Float32"; break;
        case Type.Union.float64: type = "double"; accessor = "Float64"; break;
        case Type.Union.text: type = "Text"; accessor = "Text"; break;
        case Type.Union.data: type = "PrimitiveList<byte>"; accessor = "List<byte>"; break;
        case Type.Union.anyPointer: type = "Pointer"; accessor = "RawPointer"; break;
        case Type.Union.list:
          var elemType = t.list.elementType;
          if (elemType.which == Type.Union.@bool)
          {
            type = "BoolList";
            accessor = "BoolList";
          }
          else if (elemType.which >= Type.Union.int8
           && elemType.which <= Type.Union.float64)
          {
            string elemTypeName, elemAccessor;
            this.GetTypeInfo(elemType, out elemTypeName, out elemAccessor);
            type = $"PrimitiveList<{elemTypeName}>";
            accessor = $"List<{elemTypeName}>";
          }
          else if (elemType.which == Type.Union.@enum)
          {
            type = this.GetTypeName(elemType.@enum.typeId);
            accessor = "UInt16";
          }
          else if (elemType.which >= Type.Union.text
                && elemType.which <= Type.Union.@interface)
          {
            string elemTypeName, elemAccessor;
            this.GetTypeInfo(elemType, out elemTypeName, out elemAccessor);
            type = $"CompositeList<{elemTypeName}>";
            accessor = $"CompositeList<{elemTypeName}>";
          }
          else if (elemType.which == Type.Union.anyPointer)
          {
            type = "Pointer";
            accessor = "RawPointer";
          }
          else throw new InvalidOperationException($"Unexpected element type {elemType.which}");
          break;
        case Type.Union.@enum:
          type = this.GetTypeName(t.@enum.typeId);
          accessor = "UInt16";
          break;
        case Type.Union.@struct:
          type = this.GetTypeName(t.@struct.typeId);
          accessor = $"Struct<{type}>";
          break;
        case Type.Union.@interface:
          type = this.GetTypeName(t.@interface.typeId);
          accessor = $"Interface<{type}>";
          break;
        default:
          throw new InvalidOperationException($"Unexpected type {t.which}");
      }
    }

    private string ToName(Text name) => _generator.IdentifierName(name.ToString()).ToString();

    private IEnumerable<string> GenerateMembers(Node.Struct s)
    {
      if (s.discriminantCount > 0)
      {
        var unionFieldList = s.fields
          .Where(f => f.discriminantValue != Field.noDiscriminant)
          .OrderBy(f => f.codeOrder)
          .ToList();
        yield return $@"
          public enum Union : ushort
          {{
            {string.Join("\n", unionFieldList.Select(f => $"{ToName(f.name)} = {f.discriminantValue},"))}
          }}";
        foreach (var field in unionFieldList.Where(f => f.which == Field.Union.group))
        {
          var groupName = this.GetTypeName(field.group.typeId);
          var groupTypeName = ToGroupName(groupName);
          yield return $@"
            public bool Is(out {groupTypeName} {groupName})
            {{
              var ret = this.which == Union.{ToName(field.name)};
              {groupName} = new {groupTypeName}(ret ? _s : default({StructType}));
              return ret;
            }}";
        }
        yield return $@"
          public Union which
          {{
            get {{ return (Union)_s.ReadUInt16({s.discriminantOffset}); }}
            set {{ _s.WriteUInt16({s.discriminantOffset}, (ushort)value); }}
          }}";
      }

      foreach (var field in s.fields.OrderBy(f => f.codeOrder))
      {
        var name = ToName(field.name);
        _nodePath.Push(name);
        if (field.which == Field.Union.slot)
        {
          var slot = field.slot;
          if (slot.type.which == Type.Union.@void)
          {
            _nodePath.Pop();
            continue;
          }

          string type, accessor;
          this.GetTypeInfo(slot.type, out type, out accessor);
          if (slot.type.which == Type.Union.@enum)
          {
            yield return $@"
              {GetDocComment(field.annotations)}
              public {type} {name}
              {{
                get {{ return ({type})_s.ReadUInt16({slot.offset}  {GetDefaultLiteral(slot.defaultValue)}); }}
                set {{ _s.WriteUInt16({slot.offset}, (ushort)value {GetDefaultLiteral(slot.defaultValue)}); }}
              }}";
          }
          else if (slot.type.which == Type.Union.@struct
                || slot.type.which == Type.Union.@interface
                || slot.type.which == Type.Union.list
                || slot.type.which == Type.Union.text
                || slot.type.which == Type.Union.data)
          {
            yield return $@"
              {GetDocComment(field.annotations)}
              public {type} {name}
              {{
                get {{ return _s.Dereference{accessor}({slot.offset}); }}
                set {{ _s.WritePointer({slot.offset}, value); }}
              }}";
          }
          else if (slot.type.which == Type.Union.anyPointer)
          {
            yield return $@"
              {GetDocComment(field.annotations)}
              public {type} {name}
              {{
                get {{ return _s.Read{accessor}({slot.offset}); }}
                set {{ _s.Write{accessor}({slot.offset}, value); }}
              }}";
          }
          else
          {
            yield return $@"
              {GetDocComment(field.annotations)}
              public {type} {name}
              {{
                get {{ return _s.Read{accessor}({slot.offset}  {GetDefaultLiteral(slot.defaultValue)}); }}
                set {{ _s.Write{accessor}({slot.offset}, value {GetDefaultLiteral(slot.defaultValue)}); }}
              }}";
          }
        }
        else if (field.which == Field.Union.group)
        {
          string groupName = ToGroupName(name);

          yield return $@"
            {GetDocComment(field.annotations)}
            public {groupName} {name} => new {groupName}(_s);
            public struct {groupName}
            {{
              private readonly {StructType} _s;
              public {groupName}({StructType} s) {{ _s = s; }}
            ";
          foreach (var member in this.GenerateMembers(field.group.typeId))
          {
            yield return member;
          }

          yield return "}";
        }
        else
        {
          throw new InvalidOperationException($"Unexpected field type {field.which}");
        }

        _nodePath.Pop();
      }
    }

    private static string ToGroupName(string name)
    {
      var groupName = name.TrimStart('@');
      //if (groupName[0] >= 'a' && groupName[0] <= 'z')
      //{
      //  groupName = char.ToUpperInvariant(groupName[0]) + groupName.Substring(1);
      //}
      //else
      {
        groupName += "Group";
      }

      return groupName;
    }

    private string GetTypeName(ulong typeId) => this.GetTypeName(_request.nodes.First(n => n.id == typeId));

    private string GetTypeName(Node node) => _generator.IdentifierName(node.displayName.ToString().Substring((int)node.displayNamePrefixLength)).ToString();
    
    private string GetDefaultLiteral(Value val, bool leadingComma = true)
    {
      string ret;
      if (val.which == Value.Union.@bool)        ret = val.@bool ? "true" : null;
      else if (val.which == Value.Union.int8)    ret = val.int8 != 0 ? SyntaxFactory.Literal(val.int8).ToString() : null;
      else if (val.which == Value.Union.int16)   ret = val.int16 != 0 ? SyntaxFactory.Literal(val.int16).ToString() : null;
      else if (val.which == Value.Union.int32)   ret = val.int32 != 0 ? SyntaxFactory.Literal(val.int32).ToString() : null;
      else if (val.which == Value.Union.int64)   ret = val.int64 != 0 ? SyntaxFactory.Literal(val.int64).ToString() : null;
      else if (val.which == Value.Union.uint8)   ret = val.uint8 != 0 ? SyntaxFactory.Literal(val.uint8).ToString() : null;
      else if (val.which == Value.Union.uint16)  ret = val.uint16 != 0 ? SyntaxFactory.Literal(val.uint16).ToString() : null;
      else if (val.which == Value.Union.uint32)  ret = val.uint32 != 0 ? SyntaxFactory.Literal(val.uint32).ToString() : null;
      else if (val.which == Value.Union.uint64)  ret = val.uint64 != 0 ? SyntaxFactory.Literal(val.uint64).ToString() : null;
      else if (val.which == Value.Union.float32) ret = val.float32 != 0 ? SyntaxFactory.Literal(val.float32).ToString() : null;
      else if (val.which == Value.Union.float64) ret = val.float64 != 0 ? SyntaxFactory.Literal(val.float64).ToString() : null;
      else if (val.which == Value.Union.@enum)   ret = val.@enum != 0 ? SyntaxFactory.Literal(val.@enum).ToString() : null;
      else throw new ArgumentException($"Value not a primitive type: {val.which}");
      
      return ret == null ? string.Empty
        : leadingComma ? ", " + ret
        : ret;
    }

    private string GetDocComment(Node node)
    {
      return this.GetDocComment(node.annotations);
    }

    private string GetDocComment(CompositeList<Annotation> annotations)
    {
      // TODO
      return string.Empty;
    }

    private string GetNamespace(Node node)
    {
      var ns = node.annotations
        .Where(a => a.id == NamespaceAnnotationId && a.value.which == Value.Union.text)
        .Select(a => a.value.text.ToString())
        .FirstOrDefault();

      return string.IsNullOrEmpty(ns) ? this.DefaultNamespace : ns;
    }
  }
}