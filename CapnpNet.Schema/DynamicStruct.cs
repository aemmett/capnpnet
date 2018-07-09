using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Linq.Expressions.Expression;

namespace CapnpNet.Schema
{
  public static class MessageExtensions
  {
    public static DynamicStruct AsDynamic(this Struct root, SchemaNode type)
    {
      return new DynamicStruct(type, root);
    }
  }

  public sealed class SchemaContainer
  {
    public Dictionary<ulong, SchemaNode> Nodes { get; } = new Dictionary<ulong, SchemaNode>();

    public SchemaNode this[ulong id] => this.Nodes[id];
  }

  public sealed class SchemaNode
  {
    public SchemaNode(SchemaContainer container, Node node)
    {
      this.Container = container;
      this.Node = node;
    }

    public Node Node { get; }

    public SchemaContainer Container { get; }
  }

  public struct DynamicStruct : IDynamicMetaObjectProvider
  {
    private static readonly ConstructorInfo DynamicStructConstructor = typeof(DynamicStruct)
      .GetTypeInfo()
      .DeclaredConstructors
      .First(ci => ci.GetParameters().Length == 2);

    public DynamicStruct(SchemaNode schemaNode, Struct @struct)
    {
      this.SchemaNode = schemaNode;
      this.Struct = @struct;
    }

    public SchemaNode SchemaNode { get; }

    public Struct Struct { get; }

    public DynamicMetaObject GetMetaObject(Expression parameter)
    {
      return new DynamicStructMetaObject(parameter, this);
    }
    
    public sealed class DynamicStructMetaObject : DynamicMetaObject
    {
      // TODO: binding restrictions?
      public DynamicStructMetaObject(Expression exp, DynamicStruct ds)
        : base(exp, BindingRestrictions.Empty, ds)
      {
      }

      public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
      {
        var ds = (DynamicStruct)this.Value;
        var dsP = Parameter(typeof(DynamicStruct), "ds");
        var structExp = Property(dsP, "Struct");
        var node = ds.SchemaNode.Node;
        Expression valueExp;
        switch (node.which)
        {
          case Node.Union.@struct:
            foreach (var field in node.@struct.fields)
            {
              if (field.name.ToString() != binder.Name) continue;
              
              if (field.Is(out Field.groupGroup group))
              {
                var newSchemaNode = ds.SchemaNode.Container[group.typeId];
                valueExp = New(
                  DynamicStructConstructor,
                  Constant(newSchemaNode),
                  structExp);
              }
              else if (field.Is(out Field.slotGroup slot))
              {
                var dv = slot.defaultValue;
                string readMethod;
                object defaultValue;
                System.Type type;
                switch (slot.type.which)
                {
                  case Type.Union.@void: throw new NotSupportedException();
                  case Type.Union.@bool: readMethod = "ReadBool"; defaultValue = dv.@bool; type = typeof(bool); break;
                  case Type.Union.int8: readMethod = "ReadInt8"; defaultValue = dv.int8; type = typeof(sbyte); break;
                  case Type.Union.int16: readMethod = "ReadInt16"; defaultValue = dv.int16; type = typeof(short); break;
                  case Type.Union.int32: readMethod = "ReadInt32"; defaultValue = dv.int32; type = typeof(int); break;
                  case Type.Union.int64: readMethod = "ReadInt64"; defaultValue = dv.int64; type = typeof(long); break;
                  case Type.Union.uint8: readMethod = "ReadUInt8"; defaultValue = dv.uint8; type = typeof(byte); break;
                  case Type.Union.uint16: readMethod = "ReadUInt16"; defaultValue = dv.uint16; type = typeof(ushort); break;
                  case Type.Union.uint32: readMethod = "ReadUInt32"; defaultValue = dv.uint32; type = typeof(uint); break;
                  case Type.Union.uint64: readMethod = "ReadUInt64"; defaultValue = dv.uint64; type = typeof(ulong); break;
                  case Type.Union.float32: readMethod = "ReadFloat32"; defaultValue = dv.float32; type = typeof(float); break;
                  case Type.Union.float64: readMethod = "ReadFloat64"; defaultValue = dv.float64; type = typeof(double); break;
                  case Type.Union.@enum:
                    readMethod = "ReadUInt16";
                    defaultValue = dv.uint16;
                    type = typeof(ushort); // TODO: should be an enum type; dynamically generate one?
                    break;
                  case Type.Union.text:
                  case Type.Union.data:
                  case Type.Union.list:
                  case Type.Union.@struct:
                  case Type.Union.@interface:
                  case Type.Union.anyPointer:
                    readMethod = "DereferencePointer";
                    defaultValue = null;
                    type = null;
                    
                    break;
                  default:
                    throw new NotSupportedException();
                }
                
                if (readMethod.StartsWith("Read"))
                {
                  valueExp = Call(
                    structExp,
                    typeof(Struct).GetTypeInfo().GetDeclaredMethod(readMethod),
                    Constant((int)slot.offset),
                    Constant(defaultValue));
                }
                else if (slot.type.Is(out Type.structGroup @struct))
                {
                  var newSchemaNode = ds.SchemaNode.Container[@struct.typeId];
                  valueExp = New(
                    DynamicStructConstructor,
                    Constant(newSchemaNode),
                    Call(
                      structExp,
                      typeof(Struct).GetTypeInfo().GetDeclaredMethod("DereferencePointer").MakeGenericMethod(typeof(Struct)),
                      Constant((int)slot.offset)));
                }
                else throw new NotImplementedException();
              }
              else throw new NotSupportedException();
              
              var target = Block(
                new[] { dsP },
                Assign(dsP, Convert(this.Expression, typeof(DynamicStruct))),
                Condition(
                  // if 'this' refers to a new with the same schema...
                  Equal(Property(dsP, "SchemaNode"), Constant(ds.SchemaNode)),
                  // then: perform the get-member
                  Convert(
                    valueExp,
                    binder.ReturnType),
                  // else: try to look up the member under the other schema
                  binder.GetUpdateExpression(binder.ReturnType)));
                  
              return new DynamicMetaObject(
                target,
                BindingRestrictions.GetTypeRestriction(this.Expression, typeof(DynamicStruct)));
            }

            return binder.FallbackGetMember(this);
          case Node.Union.@enum:
            break;
          case Node.Union.@interface:
            break;
          case Node.Union.@const:
            break;
          case Node.Union.file:
          case Node.Union.annotation:
          default:
            throw new NotSupportedException();
        }

        throw new NotImplementedException();
      }

      public override IEnumerable<string> GetDynamicMemberNames()
      {
        var ds = (DynamicStruct)this.Value;
        var node = ds.SchemaNode.Node;
        switch (node.which)
        {
          case Node.Union.@struct:
            return node.@struct.fields.Select(f => f.name.ToString());
          case Node.Union.@enum:
            break;
          case Node.Union.@interface:
            break;
          case Node.Union.@const:
            break;
          case Node.Union.file:
          case Node.Union.annotation:
          default:
            throw new NotSupportedException();
        }

        return base.GetDynamicMemberNames();
      }
    }
  }
}
