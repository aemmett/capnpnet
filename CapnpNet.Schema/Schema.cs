﻿/*using CapnpNet;

namespace CapnpNet.Schema
{
  public struct Node : IStruct
  {
    private Struct _s;
    public Node(Struct s) { _s = s; }
    Struct IStruct.Struct { get { return _s; } set { _s = value; } }

    public ulong id
    {
      get { return _s.ReadUInt64(0); }
      set { _s.WriteUInt64(0, value); }
    }
    
    public Text displayName
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }

    public uint displayNamePrefixLength
    {
      get { return _s.ReadUInt32(2); }
      set { _s.WriteUInt32(2, value); }
    }
    
    public ulong scopeId
    {
      get { return _s.ReadUInt64(2); }
      set { _s.WriteUInt64(2, value); }
    }
    
    public CompositeList<NestedNode> nestedNodes
    {
      get { return _s.DereferenceCompositeList<NestedNode>(1); }
      set { _s.WritePointer(1, value); }
    }

    public CompositeList<Annotation> annotations
    {
      get { return _s.DereferenceCompositeList<Annotation>(2); }
      set { _s.WritePointer(2, value); }
    }

    public enum Union : ushort
    {
      file = 0,
      @struct = 1,
      @enum = 2,
      @interface = 3,
      @const = 4,
      annotation = 5
    }

    public Union which
    {
      get { return (Union)_s.ReadUInt16(6); }
      set { _s.WriteUInt16(6, (ushort)value); }
    }

    public structGroup @struct => new structGroup(_s);
    public struct structGroup : IStruct
    {
      private Struct _s;
      public structGroup(Struct s) { _s = s; }
      Struct IStruct.Struct { get { return _s; } set { _s = value; } }
      
      //dataWordCount @7 :UInt16;  # bits[112, 128)
      public ushort dataWordCount
      {
        get { return _s.ReadUInt16(7); }
        set { _s.WriteUInt16(7, value); }
      }
      //pointerCount @8 :UInt16;  # bits[192, 208)
      public ushort pointerCount
      {
        get { return _s.ReadUInt16(12); }
        set { _s.WriteUInt16(12, value); }
      }
      //preferredListEncoding @9 :ElementSize;  # bits[208, 224)
      public ElementSize preferredListEncoding
      {
        get { return (ElementSize)_s.ReadUInt16(13); }
        set { _s.WriteUInt16(13, (ushort)value); }
      }
      //isGroup @10 :Bool;  # bits[224, 225)
      public bool isGroup
      {
        get { return _s.ReadBool(224); }
        set { _s.ReadBool(224, value); }
      }
      //discriminantCount @11 :UInt16;  # bits[240, 256)
      public ushort discriminantCount
      {
        get { return _s.ReadUInt16(15); }
        set { _s.WriteUInt16(15, value); }
      }
      //discriminantOffset @12 :UInt32;  # bits[256, 288)
      public uint discriminantOffset
      {
        get { return _s.ReadUInt32(8); }
        set { _s.WriteUInt32(8, value); }
      }
      //fields @13 :List(Field);  # ptr[3]
      public CompositeList<Field> fields
      {
        get { return _s.DereferenceCompositeList<Field>(3); }
        set { _s.WritePointer(3, value); }
      }
    }

    public enumGroup @enum => new enumGroup(_s);
    public struct enumGroup : IStruct
    {
      private Struct _s;
      public enumGroup(Struct s) { _s = s; }
      Struct IStruct.Struct { get { return _s; } set { _s = value; } }
      
      public CompositeList<Enumerant> enumerants
      {
        get { return _s.DereferenceCompositeList<Enumerant>(3); }
        set { _s.WritePointer(3, value); }
      }
    }

    public interfaceGroup @interface => new interfaceGroup(_s);
    public struct interfaceGroup
    {
      private readonly Struct _s;
      public interfaceGroup(Struct s) { _s = s; }

      public CompositeList<Method> methods
      {
        get { return _s.DereferenceCompositeList<Method>(3); }
        set { _s.WritePointer(3, value); }
      }

      public PrimitiveList<ulong> extends
      {
        get { return _s.DereferenceList<ulong>(4); }
        set { _s.WritePointer(4, value); }
      }
    }

    public constGroup @const => new constGroup(_s);
    public struct constGroup
    {
      private readonly Struct _s;
      public constGroup(Struct s) { _s = s; }

      public Type type
      {
        get { return _s.DereferenceStruct<Type>(3); }
        set { _s.WritePointer(3, value); }
      }

      public Value value
      {
        get { return _s.DereferenceStruct<Value>(4); }
        set { _s.WritePointer(4, value); }
      }
    }

    public annotationGroup annotation => new annotationGroup(_s);
    public struct annotationGroup
    {
      private readonly Struct _s;
      public annotationGroup(Struct s) { _s = s; }

      public Type type
      {
        get { return _s.DereferenceStruct<Type>(3); }
        set { _s.WritePointer(3, value); }
      }

      public bool targetsFile
      {
        get { return _s.ReadBool(112); }
        set { _s.WriteBool(112, value); }
      }

      public bool targetsConst
      {
        get { return _s.ReadBool(113); }
        set { _s.WriteBool(113, value); }
      }

      public bool targetsEnum
      {
        get { return _s.ReadBool(114); }
        set { _s.WriteBool(114, value); }
      }

      public bool targetsEnumerant
      {
        get { return _s.ReadBool(115); }
        set { _s.WriteBool(115, value); }
      }

      public bool targetsStruct
      {
        get { return _s.ReadBool(116); }
        set { _s.WriteBool(116, value); }
      }

      public bool targetsField
      {
        get { return _s.ReadBool(117); }
        set { _s.WriteBool(117, value); }
      }

      public bool targetsUnion
      {
        get { return _s.ReadBool(118); }
        set { _s.WriteBool(118, value); }
      }

      public bool targetsGroup
      {
        get { return _s.ReadBool(119); }
        set { _s.WriteBool(119, value); }
      }

      public bool targetsInterface
      {
        get { return _s.ReadBool(120); }
        set { _s.WriteBool(120, value); }
      }

      public bool targetsMethod
      {
        get { return _s.ReadBool(121); }
        set { _s.WriteBool(121, value); }
      }

      public bool targetsParam
      {
        get { return _s.ReadBool(122); }
        set { _s.WriteBool(122, value); }
      }

      public bool targetsAnnotation
      {
        get { return _s.ReadBool(123); }
        set { _s.WriteBool(123, value); }
      }
    }
  }

  public struct NestedNode : IStruct
  {
    private Struct _s;
    public NestedNode(Struct s) { _s = s; }
    Struct IStruct.Struct { get { return _s; } set { _s = value; } }

    public Text name
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }

    public ulong id
    {
      get { return _s.ReadUInt64(0); }
      set { _s.WriteUInt64(0, value); }
    }
  }

  public struct Field : IStruct
  {
    private Struct _s;
    public Field(Struct s) { _s = s; }
    Struct IStruct.Struct { get { return _s; } set { _s = value; } }

    public Text name
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }

    public ushort codeOrder
    {
      get { return _s.ReadUInt16(0); }
      set { _s.WriteUInt16(0, value); }
    }

    public CompositeList<Annotation> annotations
    {
      get { return _s.DereferenceCompositeList<Annotation>(1); }
      set { _s.WritePointer(1, value); }
    }

    public ushort discriminantValue
    {
      get { return _s.ReadUInt16(1, 65535); }
      set { _s.WriteUInt16(1, value, 65535); }
    }

    public enum Union : ushort
    {
      slot = 0,
      group = 1,
    }

    public Union which
    {
      get { return (Union)_s.ReadUInt16(4); }
      set { _s.WriteUInt16(4, (ushort)value); }
    }
    
    public slotGroup slot => new slotGroup(_s);
    public struct slotGroup
    {
      private readonly Struct _s;
      public slotGroup(Struct s) { _s = s; }

      public uint offset
      {
        get { return _s.ReadUInt32(1); }
        set { _s.ReadUInt32(1, value); }
      }

      public Type type
      {
        get { return _s.DereferenceStruct<Type>(2); }
        set { _s.WritePointer(2, value); }
      }

      public Value defaultValue
      {
        get { return _s.DereferenceStruct<Value>(3); }
        set { _s.WritePointer(3, value); }
      }

      public bool hadExplicitDefault
      {
        get { return _s.ReadBool(128); }
        set { _s.WriteBool(128, value); }
      }
    }

    public groupGroup group => new groupGroup(_s);
    public struct groupGroup
    {
      private Struct _s;
      public groupGroup(Struct s) { _s = s; }

      public ulong typeId
      {
        get { return _s.ReadUInt64(2); }
        set { _s.ReadUInt64(2, value); }
      }
    }

    public ordinalGroup ordinal => new ordinalGroup(_s);
    public struct ordinalGroup
    {
      private readonly Struct _s;
      public ordinalGroup(Struct s) { _s = s; }

      public enum Union : ushort
      {
        @implicit = 0,
        @explicit = 1,
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(5); }
        set { _s.WriteUInt16(5, (ushort)value); }
      }
      
      public ushort @explicit
      {
        get { return _s.ReadUInt16(6); }
        set { _s.WriteUInt16(6, value); }
      }
    }

    public const ushort noDiscriminant = 65535;
  }

  public struct Enumerant : IStruct
  {
    private Struct _s;
    public Enumerant(Struct s) { _s = s; }
    Struct IStruct.Struct { get { return _s; } set { _s = value; } }
    
    public Text name
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }

    public ushort codeOrder
    {
      get { return _s.ReadUInt16(0); }
      set { _s.WriteUInt16(0, value); }
    }

    public CompositeList<Annotation> annotations
    {
      get { return _s.DereferenceCompositeList<Annotation>(1); }
      set { _s.WritePointer(1, value); }
    }
  }

  public struct Method : IStruct
  {
    private Struct _s;
    public Method(Struct s) { _s = s; }
    Struct IStruct.Struct { get { return _s; } set { _s = value; } }

    public Text name
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }

    public ushort codeOrder
    {
      get { return _s.ReadUInt16(0); }
      set { _s.WriteUInt16(0, value); }
    }

    public ulong paramStructType
    {
      get { return _s.ReadUInt64(1); }
      set { _s.WriteUInt64(1, value); }
    }

    public ulong resultStructType
    {
      get { return _s.ReadUInt64(2); }
      set { _s.WriteUInt64(2, value); }
    }

    public CompositeList<Annotation> annotations
    {
      get { return _s.DereferenceCompositeList<Annotation>(1); }
      set { _s.WritePointer(1, value); }
    }
  }
  
  public struct Type : IStruct
  {
    private Struct _s;
    public Type(Struct s) { _s = s; }
    Struct IStruct.Struct { get { return _s; } set { _s = value; } }

    public override string ToString() => _s.ToString();

    public enum Union : ushort
    {
      @void = 0,
      @bool = 1,
      int8 = 2,
      int16 = 3,
      int32 = 4,
      int64 = 5,
      uint8 = 6,
      uint16 = 7,
      uint32 = 8,
      uint64 = 9,
      float32 = 10,
      float64 = 11,
      text = 12,
      data = 13,
      list = 14,
      @enum = 15,
      @struct = 16,
      @interface = 17,
      anyPointer = 18,
    }

    public Union which
    {
      get { return (Union)_s.ReadUInt16(0); }
      set { _s.WriteUInt16(0, (ushort)value); }
    }

    public listGroup list => new listGroup(_s);
    public struct listGroup
    {
      private readonly Struct _s;
      public listGroup(Struct s) { _s = s; }

      public Type elementType
      {
        get { return _s.DereferenceStruct<Type>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    public enumGroup @enum => new enumGroup(_s);
    public struct enumGroup
    {
      private readonly Struct _s;
      public enumGroup(Struct s) { _s = s; }

      public ulong typeId
      {
        get { return _s.ReadUInt64(1); }
        set { _s.WriteUInt64(1, value); }
      }
    }

    public structGroup @struct => new structGroup(_s);
    public struct structGroup
    {
      private readonly Struct _s;
      public structGroup(Struct s) { _s = s; }
      
      public ulong typeId
      {
        get { return _s.ReadUInt64(1); }
        set { _s.WriteUInt64(1, value); }
      }
    }

    public interfaceGroup @interface => new interfaceGroup(_s);
    public struct interfaceGroup
    {
      private readonly Struct _s;
      public interfaceGroup(Struct s) { _s = s; }
      
      public ulong typeId
      {
        get { return _s.ReadUInt64(1); }
        set { _s.WriteUInt64(1, value); }
      }
    }
  }

  public struct Value : IStruct
  {
    private Struct _s;
    public Value(Struct s) { _s = s; }
    Struct IStruct.Struct { get { return _s; } set { _s = value; } }
    
    public enum Union : ushort
    {
      @void = 0,
      @bool = 1,
      int8 = 2,
      int16 = 3,
      int32 = 4,
      int64 = 5,
      uint8 = 6,
      uint16 = 7,
      uint32 = 8,
      uint64 = 9,
      float32 = 10,
      float64 = 11,
      text = 12,
      data = 13,
      list = 14,
      @enum = 15,
      @struct = 16,
      @interface = 17,
      anyPointer = 18,
    }

    public Union which
    {
      get { return (Union)_s.ReadUInt16(0); }
      set { _s.WriteUInt16(0, (ushort)value); }
    }

    public bool @bool
    {
      get { return _s.ReadBool(16); }
      set { _s.WriteBool(16, value); }
    }

    public sbyte int8
    {
      get { return _s.ReadInt8(2); }
      set { _s.WriteInt8(2, value); }
    }

    public short int16
    {
      get { return _s.ReadInt16(1); }
      set { _s.WriteInt16(1, value); }
    }

    public int int32
    {
      get { return _s.ReadInt32(1); }
      set { _s.WriteInt32(1, value); }
    }

    public long int64
    {
      get { return _s.ReadInt64(1); }
      set { _s.WriteInt64(1, value); }
    }

    public byte uint8
    {
      get { return _s.ReadUInt8(2); }
      set { _s.WriteUInt8(2, value); }
    }

    public ushort uint16
    {
      get { return _s.ReadUInt16(1); }
      set { _s.WriteUInt16(1, value); }
    }

    public uint uint32
    {
      get { return _s.ReadUInt32(1); }
      set { _s.WriteUInt32(1, value); }
    }

    public ulong uint64
    {
      get { return _s.ReadUInt64(1); }
      set { _s.WriteUInt64(1, value); }
    }

    public float float32
    {
      get { return _s.ReadFloat32(1); }
      set { _s.WriteFloat32(1, value); }
    }

    public double float64
    {
      get { return _s.ReadFloat64(1); }
      set { _s.WriteFloat64(1, value); }
    }
    
    public Text text
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }

    public PrimitiveList<byte> data
    {
      get { return _s.DereferenceList<byte>(0); }
      set { _s.WritePointer(0, value); }
    }

    public ushort @enum
    {
      get { return _s.ReadUInt16(1); }
      set { _s.WriteUInt16(1, value); }
    }

    public Struct @struct
    {
      get { return _s.DereferenceRawStruct(0); }
      set { _s.WritePointer(0, value); }
    }

    public Struct anyPointer
    {
      get { return _s.DereferenceRawStruct(0); }
      set { _s.WritePointer(0, value); }
    }
  }

  public struct Annotation : IStruct
  {
    private Struct _s;
    public Annotation(Struct s) { _s = s; }
    Struct IStruct.Struct { get { return _s; } set { _s = value; } }


    public ulong id
    {
      get { return _s.ReadUInt64(0); }
      set { _s.WriteUInt64(0, value); }
    }

    public Value value
    {
      get { return _s.DereferenceStruct<Value>(0); }
      set { _s.WritePointer(0, value); }
    }
  }
  
  public enum ElementSize : ushort
  {
    empty = 0,
    bit = 1,
    @byte = 2,
    twoBytes = 3,
    fourBytes = 4,
    eightBytes = 5,
    pointer = 6,
    inlineComposite = 7,
  }

  public struct CodeGeneratorRequest : IStruct
  {
    private Struct _s;

    public CodeGeneratorRequest(Struct s) { _s = s; }

    Struct IStruct.Struct
    {
      get { return _s; }
      set { _s = value; }
    }

    public CompositeList<Node> nodes
    {
      get { return _s.DereferenceCompositeList<Node>(0); }
      set { _s.WritePointer(0, value); }
    }

    public CompositeList<RequestedFile> requestedFiles
    {
      get { return _s.DereferenceCompositeList<RequestedFile>(1); }
      set { _s.WritePointer(1, value); }
    }

    public struct RequestedFile : IStruct
    {
      private Struct _s;
      
      public RequestedFile(Struct s) { _s = s; }

      Struct IStruct.Struct
      {
        get { return _s; }
        set { _s = value; }
      }

      public ulong id
      {
        get { return _s.ReadUInt64(0); }
        set { _s.WriteUInt64(0, value); }
      }

      public Text filename
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }

      public CompositeList<Import> imports
      {
        get { return _s.DereferenceCompositeList<Import>(1); }
        set { _s.WritePointer(1, value); }
      }

      public struct Import : IStruct
      {
        private Struct _s;
        public Import(Struct s) { _s = s; }
        Struct IStruct.Struct { get { return _s; } set { _s = value; } }

        public ulong id
        {
          get { return _s.ReadUInt64(0); }
          set { _s.WriteUInt64(0, value); }
        }

        public Text name
        {
          get { return _s.DereferenceText(0); }
          set { _s.WritePointer(0, value); }
        }
      }
    }
  }
}
*/