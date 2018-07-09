using System;
using System.IO;
using Newtonsoft.Json;

namespace CapnpNet.Schema.Json
{
  public sealed class JsonCodec
  {
    private readonly SchemaContainer _schema;

    public static void Write(DynamicStruct dynamicStruct, TextWriter writer)
    {
      var j = new JsonTextWriter(writer);
      var schemaNode = dynamicStruct.SchemaNode;
      JsonCodec.WriteStruct(j, schemaNode, dynamicStruct.Struct);
    }

    private static void WriteStruct(JsonTextWriter j, SchemaNode schemaNode, Struct @struct)
    {
      j.WriteStartObject();
      foreach (var field in schemaNode.Node.@struct.fields)
      {
        if (field.Is(out Field.slotGroup slot))
        {
          j.WritePropertyName(field.name.ToString());
          int offset = (int)slot.offset;
          Value dv = slot.defaultValue;
          switch (slot.type.which)
          {
            case Type.Union.@void:
              j.WriteNull();
              break;
            case Type.Union.@bool:
              j.WriteValue(@struct.ReadBool(offset, dv.@bool));
              break;
            case Type.Union.int8:
              j.WriteValue(@struct.ReadInt8(offset, dv.int8));
              break;
            case Type.Union.int16:
              j.WriteValue(@struct.ReadInt16(offset, dv.int16));
              break;
            case Type.Union.int32:
              j.WriteValue(@struct.ReadInt32(offset, dv.int32));
              break;
            case Type.Union.int64:
              j.WriteValue(@struct.ReadInt64(offset, dv.int64));
              break;
            case Type.Union.uint8:
              j.WriteValue(@struct.ReadUInt8(offset, dv.uint8));
              break;
            case Type.Union.uint16:
              j.WriteValue(@struct.ReadUInt16(offset, dv.uint16));
              break;
            case Type.Union.uint32:
              j.WriteValue(@struct.ReadUInt32(offset, dv.uint32));
              break;
            case Type.Union.uint64:
              j.WriteValue(@struct.ReadUInt64(offset, dv.uint64));
              break;
            case Type.Union.float32:
              j.WriteValue(@struct.ReadFloat32(offset, dv.float32));
              break;
            case Type.Union.float64:
              j.WriteValue(@struct.ReadFloat64(offset, dv.float64));
              break;
            case Type.Union.text:
              // TODO: check null/default semantics
              var textPtr = @struct.DereferencePointer<Text>(offset);
              if (textPtr.Pointer.IsNull) textPtr = dv.text;

              if (textPtr.Pointer.IsNull) j.WriteNull();
              else j.WriteValue(textPtr.ToString());

              break;
            case Type.Union.data:
              var dataPtr = @struct.DereferencePointer<Data>(offset);
              if (dataPtr.Pointer.IsNull) dataPtr = dv.data;

              if (dataPtr.Pointer.IsNull) j.WriteNull();
              else if (dataPtr.Is(out ArraySegment<byte> bytes))
              {
                var array = bytes.Array;
                if (bytes.Offset != 0 || bytes.Count != array.Length)
                {
                  array = new byte[bytes.Count];
                  Array.Copy(bytes.Array, bytes.Offset, array, 0, bytes.Count);
                }
                
                j.WriteValue(array);
              }
              else
              {
                // TODO: copy to array first?
                j.WriteValue(dataPtr.GetStream());
              }

              break;
            case Type.Union.list:
              var elementType = slot.type.list.elementType;
              if (elementType.which == Type.Union.@bool)
              {
                var list = @struct.DereferencePointer<BoolList>(offset);
                if (list.Pointer.IsNull)
                {
                  // TODO: default
                  j.WriteNull();
                }
                else
                {
                  j.WriteStartArray();

                  // TODO
                  

                  j.WriteEndArray();
                }
              }

              break;
            case Type.Union.@enum:
              
              break;
            case Type.Union.@struct:
              break;
            case Type.Union.@interface:
              break;
            case Type.Union.anyPointer:
              break;
            default:
              break;
          }
        }
      }

      j.WriteEndObject();
    }
  }
}