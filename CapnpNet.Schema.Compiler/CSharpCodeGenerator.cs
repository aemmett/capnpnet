using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Options;

namespace CapnpNet.Schema.Compiler
{
  // TOOD: generate AST from FormattableString?
  public class CSharpCodeGenerator
  {
    // TODO: annotations for namespace, name overrides, and doc comments
    public const ulong NamespaceAnnotationId = ~0UL; // TODO

    public const ulong CppNamespaceAnnotationId = 0xb9c6f99ebf805f2cUL;

    // TODO: although it is auto-generated, I would like to remove excess qualification...
    public const string StructType = "global::CapnpNet.Struct";
    public const string MessageType = "global::CapnpNet.Message";
    public const string IAbsPointerType = "global::CapnpNet.IAbsPointer";
    public const string CnNamespace = "global::CapnpNet";

    private readonly CodeGeneratorRequest _request;
    private readonly SyntaxGenerator _generator;

    private Dictionary<ulong, TypeName> _typeNames = new Dictionary<ulong, TypeName>();

    private FlatArray<Node.Parameter> _implicitParameters;

    private sealed class TypeName
    {
      public TypeName(ulong id, string name, string identifier, TypeName parent)
      {
        this.Id = id;
        this.Name = name;
        this.Identifier = identifier;
        this.Parent = parent;
      }

      public TypeName(Node node, string name, TypeName parent, SyntaxGenerator syntaxGen)
      {
        this.Id = node.id;
        this.Name = name;
        this.Identifier = syntaxGen.IdentifierName(name).ToString();
        this.Parent = parent;
        this.GenericTypeArgs = node.parameters.Select(p => p.name.ToString()).ToImmutableList();
      }

      public ulong Id { get; }
      public string Name { get; }
      public string Identifier { get; }
      public TypeName Parent { get; }
      public TypeName SideContainer { get; set; }
      public ImmutableList<string> GenericTypeArgs { get; set; } = ImmutableList<string>.Empty;

      public TypeName GetCommonParent(TypeName targetName)
      {
        if (targetName == null) return null;
        
        while (targetName != null)
        {
          if (targetName == this) return this;

          targetName = targetName.Parent;
        }

        return this.Parent?.GetCommonParent(targetName);
      }

      public override string ToString() => $"{this.Parent?.ToString()}.{this.Name}";
    }

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
    
    public string DefaultNamespace { get; set; }
    public OptionKey CSharpFormattingOption { get; private set; }

    private Node this[ulong id] => _request.nodes.First(n => n.id == id);

    public IReadOnlyDictionary<string, string> GenerateSources()
    {
      return _request.requestedFiles
        .Select(f => new
        {
          name = Path.GetFileNameWithoutExtension(f.filename.ToString()),
          content = this.GenerateFile(f),
        })
        .ToDictionary(
          x => x.name,
          x => x.content);
    }

    private void BuildNames(TypeName parent, Node node, string name)
    {
      TypeName typeName;

      if (node.Is(out Node.structGroup @struct))
      {
        typeName = new TypeName(node, name, parent, _generator);
        _typeNames.Add(node.id, typeName);
        foreach (var groupField in @struct.fields.Where(f => f.which == Field.Union.group))
        {
          this.BuildNames(typeName, this[groupField.group.typeId], $"{groupField.name.ToString()}Group");
        }
      }
      else if (node.Is(out Node.interfaceGroup @interface))
      {
        typeName = new TypeName(node, "I" + name, parent, _generator);
        _typeNames.Add(node.id, typeName);
        TypeName container = null;
        void InitContainer() => container = container ?? new TypeName(node, name, parent, _generator);
        
        foreach (var method in @interface.methods)
        {
          var n = this[method.paramStructType];
          if (n.scopeId == 0 && (n.Is(out Node.structGroup s) == false || s.dataWordCount + s.pointerCount > 0))
          {
            InitContainer();
            this.BuildNames(container, n, method.name + "Params");
          }

          n = this[method.resultStructType];
          if (n.scopeId == 0 && (n.Is(out s) == false || s.dataWordCount + s.pointerCount > 0))
          {
            InitContainer();
            this.BuildNames(container, n, method.name + "Results");
          }
        }

        foreach (var nn in node.nestedNodes)
        {
          InitContainer();
          this.BuildNames(container, this[nn.id], nn.name.ToString());
        }

        typeName.SideContainer = container;
        return;
      }
      else
      {
        typeName = new TypeName(node, name, parent, _generator);
        _typeNames.Add(node.id, typeName);
      }

      foreach (var nn in node.nestedNodes)
      {
        this.BuildNames(typeName, this[nn.id], nn.name.ToString());
      }
    }

    private string GenerateFile(CodeGeneratorRequest.RequestedFile file)
    {
      var node = this[file.id];

      var @namespace = this.GetNamespace(node);

      this.BuildNames(null, node, "global::" + @namespace);
      
      var src = $@"namespace {@namespace}
{{
  {string.Join("\n", node.nestedNodes.SelectMany(this.GenerateNode))}
}}
";
      // TODO: merge adjacent GlobalConstants classes
      var tree = SyntaxFactory.ParseSyntaxTree(src);
      var root = tree.GetRoot()
        .NormalizeWhitespace("  ", false);
      return new WhitespaceRewriter().Visit(root).ToString();
    }

    private IEnumerable<string> GenerateNode(Node.NestedNode nestedNode)
    {
      return this.GenerateNode(this[nestedNode.id]);
    }

    private IEnumerable<string> GenerateNode(Node node)
    {
      var typeName = _typeNames[node.id];
      return node.which == Node.Union.@struct ? this.GenerateStruct(typeName, node)
        : node.which == Node.Union.@enum ? this.GenerateEnum(typeName, node)
        : node.which == Node.Union.@interface ? this.GenerateInterface(typeName, node)
        : node.which == Node.Union.@const ? this.GenerateConst(typeName, node)
        : node.which == Node.Union.annotation ? this.GenerateAnnotation(typeName, node)
        : null;
    }

    private IEnumerable<string> GenerateStruct(TypeName name, Node node)
    {
      var s = node.@struct;
      var genericParams = this.GetGenericParams(node);
      var genericParamSpec = genericParams.IsEmpty ? string.Empty : $"<{string.Join(", ", genericParams)}>";
      yield return $@"
        {this.GetDocComment(node)}
        [{CnNamespace}.PreferredListEncoding({CnNamespace}.ElementSize.{(CapnpNet.ElementSize)s.preferredListEncoding})]
        public struct {name.Identifier}{genericParamSpec} : {CnNamespace}.IStruct
          {this.GetGenericConstraints(genericParams)}
        {{
          public const int KnownDataWords = {s.dataWordCount.ToString()};
          public const int KnownPointerWords = {s.pointerCount.ToString()};
          public const int KnownTotalWords = {(s.dataWordCount + s.pointerCount).ToString()};
          private {StructType} _s;
          public {name.Identifier}(ref {CnNamespace}.AllocationContext allocContext) : this(allocContext.Allocate(KnownDataWords, KnownPointerWords)) {{ }}
          public {name.Identifier}({MessageType} m) : this(m, KnownDataWords, KnownPointerWords) {{ }}
          public {name.Identifier}({MessageType} m, ushort dataWords, ushort pointers) : this(m.Allocate(dataWords, pointers)) {{ }}
          public {name.Identifier}({StructType} s) {{ _s = s; }}

          {StructType} {CnNamespace}.IStruct.Struct {{ get {{ return _s; }} }}
          {CnNamespace}.AbsPointer {IAbsPointerType}.Pointer {{ get {{ return _s.Pointer; }} }}";

      foreach (var code in this.GenerateMembers(name, s))
      {
        yield return code;
      }

      foreach (var code in node.nestedNodes.SelectMany(this.GenerateNode))
      {
        yield return code;
      }

      yield return "}";
    }

    private string GetGenericConstraints(IEnumerable<string> genericParams)
    {
      return string.Join("\n", genericParams.Select(gp => $"where {gp} : {IAbsPointerType}\n"));
    }

    private ImmutableList<string> GetGenericParams(Node node)
    {
      return node.isGeneric && node.parameters.Count > 0
        ? node.parameters.Select(p => this.ToName(p.name)).ToImmutableList()
        : ImmutableList<string>.Empty;
    }

    private IEnumerable<string> GenerateEnum(TypeName name, Node node)
    {
      var @enum = node.@enum;
      yield return $@"
        {GetDocComment(node)}
        public enum {name.Identifier} : ushort
        {{
          {string.Join(",\n", @enum.enumerants
            .Select((e, i) => new { e.codeOrder, ordinal = i, enumerant = e })
            .OrderBy(e => e.codeOrder)
            .Select(e => $@"
              {GetDocComment(e.enumerant.annotations)}
              {ToName(e.enumerant.name)} = {e.ordinal}"))}
        }}";
    }

    private IEnumerable<string> GenerateInterface(TypeName name, Node node)
    {
      var i = node.@interface;
      var genericParams = this.GetGenericParams(node);
      var genericParamSpec = genericParams.IsEmpty ? string.Empty : $"<{string.Join(", ", genericParams)}>";
      yield return $@"
        {this.GetDocComment(node)}
        public interface {name.Identifier}{genericParamSpec} : {CnNamespace}.Rpc.ICapability{GetSupertypes(i.superclasses)}
          {this.GetGenericConstraints(genericParams)}
        {{";
      
      foreach (var tuple in i.methods
        .Select((method, ordinal) => ((method: method, ordinal)))
        .OrderBy(m => m.method.codeOrder))
      {
        var (method, ordinal) = tuple;

        _implicitParameters= method.implicitParameters;

        var returnType = _typeNames.ContainsKey(method.resultStructType)
          ? $"{this.GetTypeName(name, this[method.resultStructType], method.resultBrand)}"
          : "void";

        IEnumerable<string> genericArgsSpec = method.implicitParameters.Select(p => this.ToName(p.name));
        var genericArgs = method.implicitParameters.Count > 0
          ? $"<{string.Join(", ", genericArgsSpec)}>"
          : string.Empty;

        var parameter = _typeNames.ContainsKey(method.paramStructType)
          ? $"{this.GetTypeName(name, this[method.paramStructType], method.paramBrand)} parameters"
          : string.Empty;
        
        yield return $@"
          {GetDocComment(method.annotations)}
          [{CnNamespace}.Ordinal({ordinal})]
          {returnType} {this.ToName(method.name)}{genericArgs}({parameter})
          {this.GetGenericConstraints(genericArgsSpec)};";
        
        ordinal++;
      }

      _implicitParameters = default;

      yield return "}";

      if (name.SideContainer != null)
      {
        yield return $@"
          public static class {name.SideContainer.Identifier}{genericParamSpec}
            {this.GetGenericConstraints(genericParams)}
          {{";

        foreach (var code in _typeNames
          .Where(kvp => kvp.Value.Parent == name.SideContainer)
          .SelectMany(kvp => this.GenerateNode(this[kvp.Key])))
        {
          yield return code;
        }
        
        yield return "}";
      }

      string GetSupertypes(FlatArray<Superclass> extends)
      {
        var names = string.Join(", ", extends.Select(e => this.GetTypeName(name.Parent, e)));
        return string.IsNullOrEmpty(names) ? string.Empty : ", " + names;
      }
    }

    private IEnumerable<string> GenerateConst(TypeName name, Node node)
    {
      var c = node.@const;
      this.GetTypeInfo(name.Parent, c.type, out var type, out var accessor);
      var isConstType = (c.type.which >= Type.Union.@bool && c.type.which <= Type.Union.float64)
        || c.type.which == Type.Union.@enum;

      var isInGlobal = name.Parent?.Parent == null;

      if (isInGlobal) yield return "public static partial class GlobalConstants {";

      yield return $@"
        {this.GetDocComment(node)}
        public {(isConstType ? "const" : "static readonly")} {type} {name.Identifier}
          = {(c.type.which == Type.Union.@enum ? $"({type})" : "")} {this.GetDefaultLiteral(c.value, type)};";

      if (isInGlobal) yield return "}";
    }

    private IEnumerable<string> GenerateAnnotation(TypeName name, Node node)
    {
      yield return $"/*TODO*/";
    }
    
    private void GetTypeInfo(TypeName container, Type t, out string type, out string accessor)
    {
      switch (t.which)
      {
        case Type.Union.@void: type = $"{CnNamespace}.Void"; accessor = null; break;
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
        case Type.Union.text: type = $"{CnNamespace}.Text"; accessor = "Pointer"; break;
        case Type.Union.data: type = $"{CnNamespace}.Data"; accessor = "Pointer"; break;
        case Type.Union.anyPointer:
          switch (t.anyPointer.which)
          {
            case Type.anyPointerGroup.Union.unconstrained:
              switch (t.anyPointer.unconstrained.which)
              {
                case Type.anyPointerGroup.unconstrainedGroup.Union.anyKind:
                  type = $"{CnNamespace}.AbsPointer";
                  accessor = "Pointer";
                  break;
                case Type.anyPointerGroup.unconstrainedGroup.Union.@struct:
                  type = $"{CnNamespace}.Struct";
                  accessor = "Pointer";
                  break;
                case Type.anyPointerGroup.unconstrainedGroup.Union.list:
                  type = $"{CnNamespace}.AbsPointer";
                  accessor = "Pointer";
                  break;
                case Type.anyPointerGroup.unconstrainedGroup.Union.capability:
                  type = $"{CnNamespace}.Rpc.ICapability";
                  accessor = "Pointer";
                  break;
                default:
                  throw new System.InvalidOperationException("Unknown kind of unconstrained AnyPointer");
              }

              break;
            case Type.anyPointerGroup.Union.parameter:
              var typeParamInfo = t.anyPointer.parameter;
              type = this[typeParamInfo.scopeId].parameters[typeParamInfo.parameterIndex].name.ToString();
              accessor = "Pointer";
              break;
            case Type.anyPointerGroup.Union.implicitMethodParameter:
              type = _implicitParameters[t.anyPointer.implicitMethodParameter.parameterIndex].name.ToString();
              accessor = null;
              break;
            default:
              throw new System.InvalidOperationException("Unknown kind of anyPointer");
          }

          break;
        case Type.Union.list:
          var elemType = t.list.elementType;
          this.GetTypeInfo(container, elemType, out type, out _);
          type = $"{CnNamespace}.FlatArray<{type}>";
          accessor = "Pointer";
          break;
        case Type.Union.@enum:
          // TODO: generic enums? I guess maybe from parent scope?
          type = this.GetTypeName(container, t.@enum.typeId);
          accessor = "UInt16";
          break;
        case Type.Union.@struct:
          type = this.GetTypeName(container, t.@struct.typeId, t.@struct.brand);
          accessor = $"Pointer";
          break;
        case Type.Union.@interface:
          type = this.GetTypeName(container, t.@interface.typeId, t.@interface.brand);
          accessor = "Pointer";
          break;
        default:
          throw new System.InvalidOperationException($"Unexpected type {t.which}");
      }
    }

    private string ToName(Text name) => _generator.IdentifierName(name.ToString()).ToString();

    private IEnumerable<string> GenerateMembers(TypeName container, Node.structGroup s)
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
            {string.Join("\n", unionFieldList.Select(f => $"{this.ToName(f.name)} = {f.discriminantValue},"))}
          }}";
        foreach (var field in unionFieldList.Where(f => f.which == Field.Union.group))
        {
          var discriminantName = this.ToName(field.name);
          var type = this.GetTypeName(container, field.group.typeId);
          var returnValueName = discriminantName == "ret" ? "ret1" : "ret";
          yield return $@"
            public bool Is(out {type} {discriminantName})
            {{
              var {returnValueName} = this.which == Union.{discriminantName};
              {discriminantName} = new {type}({returnValueName} ? _s : default({StructType}));
              return {returnValueName};
            }}";
        }
        foreach (var field in unionFieldList
            .Where(f => f.Is(out Field.slotGroup slot) && slot.type.which == Type.Union.@struct))
        {
          var discriminantName = this.ToName(field.name);
          var type = this.GetTypeName(container, field.slot.type.@struct.typeId);
          var returnValueName = discriminantName == "ret" ? "ret1" : "ret";
          yield return $@"
            public bool Is(out {type} {discriminantName})
            {{
              var {returnValueName} = this.which == Union.{discriminantName};
              {discriminantName} = {returnValueName} ? this.{discriminantName} : default({type});
              return {returnValueName};
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
        if (field.which == Field.Union.slot)
        {
          var slot = field.slot;
          if (slot.type.which == Type.Union.@void)
          {
            continue;
          }

          this.GetTypeInfo(container, slot.type, out string type, out string accessor);
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
          else if (accessor == "Pointer")
          {
            yield return $@"
              {GetDocComment(field.annotations)}
              public {type} {name}
              {{
                get {{ return _s.DereferencePointer<{type}>({slot.offset}); }}
                set {{ _s.WritePointer({slot.offset}, value); }}
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
          string groupName = _typeNames[field.group.typeId].Identifier;

          yield return $@"
            {GetDocComment(field.annotations)}
            public {groupName} {name} => new {groupName}(_s);
            public struct {groupName}
            {{
              private readonly {StructType} _s;
              public {groupName}({StructType} s) {{ _s = s; }}
            ";
          foreach (var member in this.GenerateMembers(
            container,
            _request.nodes.First(n => n.id == field.group.typeId && n.which == Node.Union.@struct).@struct))
          {
            yield return member;
          }

          yield return "}";
        }
        else
        {
          throw new System.InvalidOperationException($"Unexpected field type {field.which}");
        }
      }
    }
    
    private string GetTypeName(TypeName container, Superclass super) => this.GetTypeName(container, super.id);

    private string GetTypeName(TypeName container, ulong typeId, Brand brand = default) => this.GetTypeName(container, this[typeId], brand);

    private string GetTypeName(
      TypeName container,
      Node node,
      Brand brand = default)
    {
      var targetName = _typeNames[node.id];

      var importedNames = new List<TypeName>();
      var parent = container;
      while (parent != null)
      {
        importedNames.Add(parent);
        parent = parent.Parent;
      }

      var name = this.GetTypeName(targetName, brand);
      parent = targetName.Parent;
      while (importedNames.Contains(parent) == false)
      {
        name = $"{this.GetTypeName(parent, brand)}.{name}";
        parent = parent.Parent;
      }

      return name;
    }
    
    private string GetTypeName(TypeName name, Brand typeArgs)
    {
      var node = this[name.Id];
      if (name.GenericTypeArgs.IsEmpty) return name.Identifier;
      
      var scope = typeArgs.scopes
        .FirstOrDefault(s => s.scopeId == node.id);

      IEnumerable<string> typeArgsSpec;
      if (scope.which == Brand.Scope.Union.bind)
      {
        typeArgsSpec = name.GenericTypeArgs
          .Select((ga, i) =>
          {
            if (scope.bind.Count > i)
            {
              var b = scope.bind[i];
              if (b.which == Brand.Binding.Union.type)
              {
                this.GetTypeInfo(name, b.type, out var type, out _);
                return type;
              }
            }
            
            return $"{CnNamespace}.AbsPointer";
          });
      }
      else if (scope.which == Brand.Scope.Union.inherit)
      {
        typeArgsSpec = name.GenericTypeArgs;
      }
      else throw new System.NotSupportedException();

      return $"{name.Identifier}<{string.Join(", ", typeArgsSpec)}>";
    }

    private string GetDefaultLiteral(Value val, string type)
    {
      if (val.which == Value.Union.@bool) return val.@bool ? "true" : "false";
      else if (val.which == Value.Union.int8) return SyntaxFactory.Literal(val.int8).ToString();
      else if (val.which == Value.Union.int16) return SyntaxFactory.Literal(val.int16).ToString();
      else if (val.which == Value.Union.int32) return SyntaxFactory.Literal(val.int32).ToString();
      else if (val.which == Value.Union.int64) return SyntaxFactory.Literal(val.int64).ToString();
      else if (val.which == Value.Union.uint8) return SyntaxFactory.Literal(val.uint8).ToString();
      else if (val.which == Value.Union.uint16) return SyntaxFactory.Literal(val.uint16).ToString();
      else if (val.which == Value.Union.uint32) return SyntaxFactory.Literal(val.uint32).ToString();
      else if (val.which == Value.Union.uint64) return SyntaxFactory.Literal(val.uint64).ToString();
      else if (val.which == Value.Union.float32) return SyntaxFactory.Literal(val.float32).ToString();
      else if (val.which == Value.Union.float64) return SyntaxFactory.Literal(val.float64).ToString();
      else if (val.which == Value.Union.@enum) return SyntaxFactory.Literal(val.@enum).ToString();
      //else if (val.which == Value.Union.text) return SyntaxFactory.Literal(val.text.ToString()).ToString();
      else return $"default({type})/*not yet supported*/";
    }

    private string GetDefaultLiteral(Value val)
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
      else if (val.which == Value.Union.@void)   ret = $"default({CnNamespace}.Void)";
      else if (val.which == Value.Union.text)    ret = SyntaxFactory.Literal(val.text.ToString()).ToString();
      else ret = $"default /*not yet supported*/";
      //else throw new System.ArgumentException($"Value not a primitive type: {val.which}");
      
      return ret == null ? string.Empty : ", " + ret;
    }

    private string GetDocComment(Node node)
    {
      return this.GetDocComment(node.annotations);
    }

    private string GetDocComment(FlatArray<Annotation> annotations)
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

      if (string.IsNullOrEmpty(ns))
      {
        // try looking for the C++ annotation
        ns = node.annotations
          .Where(a => a.id == CppNamespaceAnnotationId)
          .Select(a => a.value.text.ToString())
          .FirstOrDefault()
          ?.Split(new string[] { "::" }, StringSplitOptions.None)
          // capitalize first letter of each namespace segment
          .Select(w => w.Length == 0 ? w : char.ToUpperInvariant(w[0]) + w.Substring(1))
          .StringJoin(".");
      }

      return string.IsNullOrEmpty(ns) ? this.DefaultNamespace : ns;
    }
  }
}