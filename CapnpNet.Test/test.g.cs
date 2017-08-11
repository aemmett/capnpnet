namespace Schema
{
  public enum TestEnum : ushort
  {
    foo = 0,
    bar = 1,
    baz = 2,
    qux = 3,
    quux = 4,
    corge = 5,
    grault = 6,
    garply = 7
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestAllTypes : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 6;
    public const int KNOWN_POINTER_WORDS = 20;
    private global::CapnpNet.Struct _s;
    public TestAllTypes(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestAllTypes(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestAllTypes(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestAllTypes(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public bool boolField
    {
      get { return _s.ReadBool(0); }
      set { _s.WriteBool(0, value); }
    }

    public sbyte int8Field
    {
      get { return _s.ReadInt8(1); }
      set { _s.WriteInt8(1, value); }
    }

    public short int16Field
    {
      get { return _s.ReadInt16(1); }
      set { _s.WriteInt16(1, value); }
    }

    public int int32Field
    {
      get { return _s.ReadInt32(1); }
      set { _s.WriteInt32(1, value); }
    }

    public long int64Field
    {
      get { return _s.ReadInt64(1); }
      set { _s.WriteInt64(1, value); }
    }

    public byte uInt8Field
    {
      get { return _s.ReadUInt8(16); }
      set { _s.WriteUInt8(16, value); }
    }

    public ushort uInt16Field
    {
      get { return _s.ReadUInt16(9); }
      set { _s.WriteUInt16(9, value); }
    }

    public uint uInt32Field
    {
      get { return _s.ReadUInt32(5); }
      set { _s.WriteUInt32(5, value); }
    }

    public ulong uInt64Field
    {
      get { return _s.ReadUInt64(3); }
      set { _s.WriteUInt64(3, value); }
    }

    public float float32Field
    {
      get { return _s.ReadFloat32(8); }
      set { _s.WriteFloat32(8, value); }
    }

    public double float64Field
    {
      get { return _s.ReadFloat64(5); }
      set { _s.WriteFloat64(5, value); }
    }

    public global::CapnpNet.Text textField
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }

    public global::CapnpNet.FlatArray<byte> dataField
    {
      get { return new global::CapnpNet.FlatArray<byte>(_s.DereferenceAbsPointer(1)); }
      set { _s.WritePointer(1, value.Pointer); }
    }

    public TestAllTypes structField
    {
      get { return _s.DereferenceStruct<TestAllTypes>(2); }
      set { _s.WritePointer(2, value); }
    }

    public TestEnum enumField
    {
      get { return (TestEnum)_s.ReadUInt16(18); }
      set { _s.WriteUInt16(18, (ushort)value); }
    }

    public global::CapnpNet.FlatArray<global::CapnpNet.Void> voidList
    {
      get { return new global::CapnpNet.FlatArray<global::CapnpNet.Void>(_s.DereferenceAbsPointer(3)); }
      set { _s.WritePointer(3, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<bool> boolList
    {
      get { return new global::CapnpNet.FlatArray<bool>(_s.DereferenceAbsPointer(4)); }
      set { _s.WritePointer(4, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<sbyte> int8List
    {
      get { return new global::CapnpNet.FlatArray<sbyte>(_s.DereferenceAbsPointer(5)); }
      set { _s.WritePointer(5, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<short> int16List
    {
      get { return new global::CapnpNet.FlatArray<short>(_s.DereferenceAbsPointer(6)); }
      set { _s.WritePointer(6, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<int> int32List
    {
      get { return new global::CapnpNet.FlatArray<int>(_s.DereferenceAbsPointer(7)); }
      set { _s.WritePointer(7, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<long> int64List
    {
      get { return new global::CapnpNet.FlatArray<long>(_s.DereferenceAbsPointer(8)); }
      set { _s.WritePointer(8, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<byte> uInt8List
    {
      get { return new global::CapnpNet.FlatArray<byte>(_s.DereferenceAbsPointer(9)); }
      set { _s.WritePointer(9, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<ushort> uInt16List
    {
      get { return new global::CapnpNet.FlatArray<ushort>(_s.DereferenceAbsPointer(10)); }
      set { _s.WritePointer(10, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<uint> uInt32List
    {
      get { return new global::CapnpNet.FlatArray<uint>(_s.DereferenceAbsPointer(11)); }
      set { _s.WritePointer(11, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<ulong> uInt64List
    {
      get { return new global::CapnpNet.FlatArray<ulong>(_s.DereferenceAbsPointer(12)); }
      set { _s.WritePointer(12, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<float> float32List
    {
      get { return new global::CapnpNet.FlatArray<float>(_s.DereferenceAbsPointer(13)); }
      set { _s.WritePointer(13, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<double> float64List
    {
      get { return new global::CapnpNet.FlatArray<double>(_s.DereferenceAbsPointer(14)); }
      set { _s.WritePointer(14, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<global::CapnpNet.Text> textList
    {
      get { return new global::CapnpNet.FlatArray<global::CapnpNet.Text>(_s.DereferenceAbsPointer(15)); }
      set { _s.WritePointer(15, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<global::CapnpNet.FlatArray<byte>> dataList
    {
      get { return new global::CapnpNet.FlatArray<global::CapnpNet.FlatArray<byte>>(_s.DereferenceAbsPointer(16)); }
      set { _s.WritePointer(16, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<TestAllTypes> structList
    {
      get { return new global::CapnpNet.FlatArray<TestAllTypes>(_s.DereferenceAbsPointer(17)); }
      set { _s.WritePointer(17, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<TestEnum> enumList
    {
      get { return new global::CapnpNet.FlatArray<TestEnum>(_s.DereferenceAbsPointer(18)); }
      set { _s.WritePointer(18, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<global::CapnpNet.Void> interfaceList
    {
      get { return new global::CapnpNet.FlatArray<global::CapnpNet.Void>(_s.DereferenceAbsPointer(19)); }
      set { _s.WritePointer(19, value.Pointer); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestDefaults : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 6;
    public const int KNOWN_POINTER_WORDS = 20;
    private global::CapnpNet.Struct _s;
    public TestDefaults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestDefaults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestDefaults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestDefaults(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public bool boolField
    {
      get { return _s.ReadBool(0, true); }
      set { _s.WriteBool(0, value, true); }
    }

    public sbyte int8Field
    {
      get { return _s.ReadInt8(1, -123); }
      set { _s.WriteInt8(1, value, -123); }
    }

    public short int16Field
    {
      get { return _s.ReadInt16(1, -12345); }
      set { _s.WriteInt16(1, value, -12345); }
    }

    public int int32Field
    {
      get { return _s.ReadInt32(1, -12345678); }
      set { _s.WriteInt32(1, value, -12345678); }
    }

    public long int64Field
    {
      get { return _s.ReadInt64(1, -123456789012345L); }
      set { _s.WriteInt64(1, value, -123456789012345L); }
    }

    public byte uInt8Field
    {
      get { return _s.ReadUInt8(16, 234); }
      set { _s.WriteUInt8(16, value, 234); }
    }

    public ushort uInt16Field
    {
      get { return _s.ReadUInt16(9, 45678); }
      set { _s.WriteUInt16(9, value, 45678); }
    }

    public uint uInt32Field
    {
      get { return _s.ReadUInt32(5, 3456789012U); }
      set { _s.WriteUInt32(5, value, 3456789012U); }
    }

    public ulong uInt64Field
    {
      get { return _s.ReadUInt64(3, 12345678901234567890UL); }
      set { _s.WriteUInt64(3, value, 12345678901234567890UL); }
    }

    public float float32Field
    {
      get { return _s.ReadFloat32(8, 1234.5F); }
      set { _s.WriteFloat32(8, value, 1234.5F); }
    }

    public double float64Field
    {
      get { return _s.ReadFloat64(5, -1.23E+47); }
      set { _s.WriteFloat64(5, value, -1.23E+47); }
    }

    public global::CapnpNet.Text textField
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }

    public global::CapnpNet.FlatArray<byte> dataField
    {
      get { return new global::CapnpNet.FlatArray<byte>(_s.DereferenceAbsPointer(1)); }
      set { _s.WritePointer(1, value.Pointer); }
    }

    public TestAllTypes structField
    {
      get { return _s.DereferenceStruct<TestAllTypes>(2); }
      set { _s.WritePointer(2, value); }
    }

    public TestEnum enumField
    {
      get { return (TestEnum)_s.ReadUInt16(18, 5); }
      set { _s.WriteUInt16(18, (ushort)value, 5); }
    }

    public global::CapnpNet.FlatArray<global::CapnpNet.Void> voidList
    {
      get { return new global::CapnpNet.FlatArray<global::CapnpNet.Void>(_s.DereferenceAbsPointer(3)); }
      set { _s.WritePointer(3, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<bool> boolList
    {
      get { return new global::CapnpNet.FlatArray<bool>(_s.DereferenceAbsPointer(4)); }
      set { _s.WritePointer(4, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<sbyte> int8List
    {
      get { return new global::CapnpNet.FlatArray<sbyte>(_s.DereferenceAbsPointer(5)); }
      set { _s.WritePointer(5, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<short> int16List
    {
      get { return new global::CapnpNet.FlatArray<short>(_s.DereferenceAbsPointer(6)); }
      set { _s.WritePointer(6, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<int> int32List
    {
      get { return new global::CapnpNet.FlatArray<int>(_s.DereferenceAbsPointer(7)); }
      set { _s.WritePointer(7, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<long> int64List
    {
      get { return new global::CapnpNet.FlatArray<long>(_s.DereferenceAbsPointer(8)); }
      set { _s.WritePointer(8, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<byte> uInt8List
    {
      get { return new global::CapnpNet.FlatArray<byte>(_s.DereferenceAbsPointer(9)); }
      set { _s.WritePointer(9, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<ushort> uInt16List
    {
      get { return new global::CapnpNet.FlatArray<ushort>(_s.DereferenceAbsPointer(10)); }
      set { _s.WritePointer(10, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<uint> uInt32List
    {
      get { return new global::CapnpNet.FlatArray<uint>(_s.DereferenceAbsPointer(11)); }
      set { _s.WritePointer(11, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<ulong> uInt64List
    {
      get { return new global::CapnpNet.FlatArray<ulong>(_s.DereferenceAbsPointer(12)); }
      set { _s.WritePointer(12, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<float> float32List
    {
      get { return new global::CapnpNet.FlatArray<float>(_s.DereferenceAbsPointer(13)); }
      set { _s.WritePointer(13, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<double> float64List
    {
      get { return new global::CapnpNet.FlatArray<double>(_s.DereferenceAbsPointer(14)); }
      set { _s.WritePointer(14, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<global::CapnpNet.Text> textList
    {
      get { return new global::CapnpNet.FlatArray<global::CapnpNet.Text>(_s.DereferenceAbsPointer(15)); }
      set { _s.WritePointer(15, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<global::CapnpNet.FlatArray<byte>> dataList
    {
      get { return new global::CapnpNet.FlatArray<global::CapnpNet.FlatArray<byte>>(_s.DereferenceAbsPointer(16)); }
      set { _s.WritePointer(16, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<TestAllTypes> structList
    {
      get { return new global::CapnpNet.FlatArray<TestAllTypes>(_s.DereferenceAbsPointer(17)); }
      set { _s.WritePointer(17, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<TestEnum> enumList
    {
      get { return new global::CapnpNet.FlatArray<TestEnum>(_s.DereferenceAbsPointer(18)); }
      set { _s.WritePointer(18, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<global::CapnpNet.Void> interfaceList
    {
      get { return new global::CapnpNet.FlatArray<global::CapnpNet.Void>(_s.DereferenceAbsPointer(19)); }
      set { _s.WritePointer(19, value.Pointer); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestAnyPointer : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public TestAnyPointer(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestAnyPointer(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestAnyPointer(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestAnyPointer(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public global::CapnpNet.AbsPointer anyPointerField
    {
      get { return _s.DereferenceAbsPointer(0); }
      set { _s.WritePointer(0, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestAnyOthers : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 3;
    private global::CapnpNet.Struct _s;
    public TestAnyOthers(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestAnyOthers(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestAnyOthers(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestAnyOthers(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public global::CapnpNet.Struct anyStructField
    {
      get { return _s.DereferenceRawStruct(0); }
      set { _s.WritePointer(0, value); }
    }

    public global::CapnpNet.AbsPointer anyListField
    {
      get { return _s.DereferenceAbsPointer(1); }
      set { _s.WritePointer(1, value); }
    }

    public global::CapnpNet.Rpc.ICapability capabilityField
    {
      get { return _s.ReadInterface<global::CapnpNet.Rpc.ICapability>(2); }
      set { _s.WritePointer(2, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestOutOfOrder : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 9;
    private global::CapnpNet.Struct _s;
    public TestOutOfOrder(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestOutOfOrder(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestOutOfOrder(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestOutOfOrder(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public global::CapnpNet.Text foo
    {
      get { return _s.DereferenceText(3); }
      set { _s.WritePointer(3, value); }
    }

    public global::CapnpNet.Text bar
    {
      get { return _s.DereferenceText(2); }
      set { _s.WritePointer(2, value); }
    }

    public global::CapnpNet.Text baz
    {
      get { return _s.DereferenceText(8); }
      set { _s.WritePointer(8, value); }
    }

    public global::CapnpNet.Text qux
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }

    public global::CapnpNet.Text quux
    {
      get { return _s.DereferenceText(6); }
      set { _s.WritePointer(6, value); }
    }

    public global::CapnpNet.Text corge
    {
      get { return _s.DereferenceText(4); }
      set { _s.WritePointer(4, value); }
    }

    public global::CapnpNet.Text grault
    {
      get { return _s.DereferenceText(1); }
      set { _s.WritePointer(1, value); }
    }

    public global::CapnpNet.Text garply
    {
      get { return _s.DereferenceText(7); }
      set { _s.WritePointer(7, value); }
    }

    public global::CapnpNet.Text waldo
    {
      get { return _s.DereferenceText(5); }
      set { _s.WritePointer(5, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestUnion : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 8;
    public const int KNOWN_POINTER_WORDS = 2;
    private global::CapnpNet.Struct _s;
    public TestUnion(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestUnion(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestUnion(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestUnion(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public union0Group union0 => new union0Group(_s);
    public struct union0Group
    {
      private readonly global::CapnpNet.Struct _s;
      public union0Group(global::CapnpNet.Struct s) { _s = s; }
      public enum Union : ushort
      {
        u0f0s0 = 0,
        u0f0s1 = 1,
        u0f0s8 = 2,
        u0f0s16 = 3,
        u0f0s32 = 4,
        u0f0s64 = 5,
        u0f0sp = 6,
        u0f1s0 = 7,
        u0f1s1 = 8,
        u0f1s8 = 9,
        u0f1s16 = 10,
        u0f1s32 = 11,
        u0f1s64 = 12,
        u0f1sp = 13,
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(0); }
        set { _s.WriteUInt16(0, (ushort)value); }
      }

      public bool u0f0s1
      {
        get { return _s.ReadBool(64); }
        set { _s.WriteBool(64, value); }
      }

      public sbyte u0f0s8
      {
        get { return _s.ReadInt8(8); }
        set { _s.WriteInt8(8, value); }
      }

      public short u0f0s16
      {
        get { return _s.ReadInt16(4); }
        set { _s.WriteInt16(4, value); }
      }

      public int u0f0s32
      {
        get { return _s.ReadInt32(2); }
        set { _s.WriteInt32(2, value); }
      }

      public long u0f0s64
      {
        get { return _s.ReadInt64(1); }
        set { _s.WriteInt64(1, value); }
      }

      public global::CapnpNet.Text u0f0sp
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }

      public bool u0f1s1
      {
        get { return _s.ReadBool(64); }
        set { _s.WriteBool(64, value); }
      }

      public sbyte u0f1s8
      {
        get { return _s.ReadInt8(8); }
        set { _s.WriteInt8(8, value); }
      }

      public short u0f1s16
      {
        get { return _s.ReadInt16(4); }
        set { _s.WriteInt16(4, value); }
      }

      public int u0f1s32
      {
        get { return _s.ReadInt32(2); }
        set { _s.WriteInt32(2, value); }
      }

      public long u0f1s64
      {
        get { return _s.ReadInt64(1); }
        set { _s.WriteInt64(1, value); }
      }

      public global::CapnpNet.Text u0f1sp
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    public bool bit0
    {
      get { return _s.ReadBool(128); }
      set { _s.WriteBool(128, value); }
    }

    public union1Group union1 => new union1Group(_s);
    public struct union1Group
    {
      private readonly global::CapnpNet.Struct _s;
      public union1Group(global::CapnpNet.Struct s) { _s = s; }
      public enum Union : ushort
      {
        u1f0s0 = 0,
        u1f0s1 = 1,
        u1f1s1 = 2,
        u1f0s8 = 3,
        u1f1s8 = 4,
        u1f0s16 = 5,
        u1f1s16 = 6,
        u1f0s32 = 7,
        u1f1s32 = 8,
        u1f0s64 = 9,
        u1f1s64 = 10,
        u1f0sp = 11,
        u1f1sp = 12,
        u1f2s0 = 13,
        u1f2s1 = 14,
        u1f2s8 = 15,
        u1f2s16 = 16,
        u1f2s32 = 17,
        u1f2s64 = 18,
        u1f2sp = 19,
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(1); }
        set { _s.WriteUInt16(1, (ushort)value); }
      }

      public bool u1f0s1
      {
        get { return _s.ReadBool(129); }
        set { _s.WriteBool(129, value); }
      }

      public bool u1f1s1
      {
        get { return _s.ReadBool(129); }
        set { _s.WriteBool(129, value); }
      }

      public sbyte u1f0s8
      {
        get { return _s.ReadInt8(17); }
        set { _s.WriteInt8(17, value); }
      }

      public sbyte u1f1s8
      {
        get { return _s.ReadInt8(17); }
        set { _s.WriteInt8(17, value); }
      }

      public short u1f0s16
      {
        get { return _s.ReadInt16(9); }
        set { _s.WriteInt16(9, value); }
      }

      public short u1f1s16
      {
        get { return _s.ReadInt16(9); }
        set { _s.WriteInt16(9, value); }
      }

      public int u1f0s32
      {
        get { return _s.ReadInt32(5); }
        set { _s.WriteInt32(5, value); }
      }

      public int u1f1s32
      {
        get { return _s.ReadInt32(5); }
        set { _s.WriteInt32(5, value); }
      }

      public long u1f0s64
      {
        get { return _s.ReadInt64(3); }
        set { _s.WriteInt64(3, value); }
      }

      public long u1f1s64
      {
        get { return _s.ReadInt64(3); }
        set { _s.WriteInt64(3, value); }
      }

      public global::CapnpNet.Text u1f0sp
      {
        get { return _s.DereferenceText(1); }
        set { _s.WritePointer(1, value); }
      }

      public global::CapnpNet.Text u1f1sp
      {
        get { return _s.DereferenceText(1); }
        set { _s.WritePointer(1, value); }
      }

      public bool u1f2s1
      {
        get { return _s.ReadBool(129); }
        set { _s.WriteBool(129, value); }
      }

      public sbyte u1f2s8
      {
        get { return _s.ReadInt8(17); }
        set { _s.WriteInt8(17, value); }
      }

      public short u1f2s16
      {
        get { return _s.ReadInt16(9); }
        set { _s.WriteInt16(9, value); }
      }

      public int u1f2s32
      {
        get { return _s.ReadInt32(5); }
        set { _s.WriteInt32(5, value); }
      }

      public long u1f2s64
      {
        get { return _s.ReadInt64(3); }
        set { _s.WriteInt64(3, value); }
      }

      public global::CapnpNet.Text u1f2sp
      {
        get { return _s.DereferenceText(1); }
        set { _s.WritePointer(1, value); }
      }
    }

    public bool bit2
    {
      get { return _s.ReadBool(130); }
      set { _s.WriteBool(130, value); }
    }

    public bool bit3
    {
      get { return _s.ReadBool(131); }
      set { _s.WriteBool(131, value); }
    }

    public bool bit4
    {
      get { return _s.ReadBool(132); }
      set { _s.WriteBool(132, value); }
    }

    public bool bit5
    {
      get { return _s.ReadBool(133); }
      set { _s.WriteBool(133, value); }
    }

    public bool bit6
    {
      get { return _s.ReadBool(134); }
      set { _s.WriteBool(134, value); }
    }

    public bool bit7
    {
      get { return _s.ReadBool(135); }
      set { _s.WriteBool(135, value); }
    }

    public union2Group union2 => new union2Group(_s);
    public struct union2Group
    {
      private readonly global::CapnpNet.Struct _s;
      public union2Group(global::CapnpNet.Struct s) { _s = s; }
      public enum Union : ushort
      {
        u2f0s64 = 4,
        u2f0s32 = 3,
        u2f0s16 = 2,
        u2f0s8 = 1,
        u2f0s1 = 0,
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(2); }
        set { _s.WriteUInt16(2, (ushort)value); }
      }

      public long u2f0s64
      {
        get { return _s.ReadInt64(6); }
        set { _s.WriteInt64(6, value); }
      }

      public int u2f0s32
      {
        get { return _s.ReadInt32(10); }
        set { _s.WriteInt32(10, value); }
      }

      public short u2f0s16
      {
        get { return _s.ReadInt16(18); }
        set { _s.WriteInt16(18, value); }
      }

      public sbyte u2f0s8
      {
        get { return _s.ReadInt8(33); }
        set { _s.WriteInt8(33, value); }
      }

      public bool u2f0s1
      {
        get { return _s.ReadBool(256); }
        set { _s.WriteBool(256, value); }
      }
    }

    public union3Group union3 => new union3Group(_s);
    public struct union3Group
    {
      private readonly global::CapnpNet.Struct _s;
      public union3Group(global::CapnpNet.Struct s) { _s = s; }
      public enum Union : ushort
      {
        u3f0s64 = 4,
        u3f0s32 = 3,
        u3f0s16 = 2,
        u3f0s8 = 1,
        u3f0s1 = 0,
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(3); }
        set { _s.WriteUInt16(3, (ushort)value); }
      }

      public long u3f0s64
      {
        get { return _s.ReadInt64(7); }
        set { _s.WriteInt64(7, value); }
      }

      public int u3f0s32
      {
        get { return _s.ReadInt32(11); }
        set { _s.WriteInt32(11, value); }
      }

      public short u3f0s16
      {
        get { return _s.ReadInt16(19); }
        set { _s.WriteInt16(19, value); }
      }

      public sbyte u3f0s8
      {
        get { return _s.ReadInt8(34); }
        set { _s.WriteInt8(34, value); }
      }

      public bool u3f0s1
      {
        get { return _s.ReadBool(257); }
        set { _s.WriteBool(257, value); }
      }
    }

    public byte byte0
    {
      get { return _s.ReadUInt8(35); }
      set { _s.WriteUInt8(35, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestUnnamedUnion : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 2;
    public const int KNOWN_POINTER_WORDS = 2;
    private global::CapnpNet.Struct _s;
    public TestUnnamedUnion(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestUnnamedUnion(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestUnnamedUnion(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestUnnamedUnion(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public enum Union : ushort
    {
      foo = 0,
      bar = 1,
    }

    public Union which
    {
      get { return (Union)_s.ReadUInt16(2); }
      set { _s.WriteUInt16(2, (ushort)value); }
    }

    public global::CapnpNet.Text before
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }

    public ushort foo
    {
      get { return _s.ReadUInt16(0); }
      set { _s.WriteUInt16(0, value); }
    }

    public uint bar
    {
      get { return _s.ReadUInt32(2); }
      set { _s.WriteUInt32(2, value); }
    }

    public ushort middle
    {
      get { return _s.ReadUInt16(1); }
      set { _s.WriteUInt16(1, value); }
    }

    public global::CapnpNet.Text after
    {
      get { return _s.DereferenceText(1); }
      set { _s.WritePointer(1, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestUnionInUnion : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 2;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public TestUnionInUnion(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestUnionInUnion(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestUnionInUnion(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestUnionInUnion(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public outerGroup outer => new outerGroup(_s);
    public struct outerGroup
    {
      private readonly global::CapnpNet.Struct _s;
      public outerGroup(global::CapnpNet.Struct s) { _s = s; }
      public enum Union : ushort
      {
        inner = 0,
        baz = 1,
      }

      public bool Is(out outerGroup.innerGroup inner)
      {
        var ret = this.which == Union.inner;
        inner = new outerGroup.innerGroup(ret ? _s : default (global::CapnpNet.Struct));
        return ret;
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(4); }
        set { _s.WriteUInt16(4, (ushort)value); }
      }

      public innerGroup inner => new innerGroup(_s);
      public struct innerGroup
      {
        private readonly global::CapnpNet.Struct _s;
        public innerGroup(global::CapnpNet.Struct s) { _s = s; }
        public enum Union : ushort
        {
          foo = 0,
          bar = 1,
        }

        public Union which
        {
          get { return (Union)_s.ReadUInt16(2); }
          set { _s.WriteUInt16(2, (ushort)value); }
        }

        public int foo
        {
          get { return _s.ReadInt32(0); }
          set { _s.WriteInt32(0, value); }
        }

        public int bar
        {
          get { return _s.ReadInt32(0); }
          set { _s.WriteInt32(0, value); }
        }
      }

      public int baz
      {
        get { return _s.ReadInt32(0); }
        set { _s.WriteInt32(0, value); }
      }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestGroups : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 2;
    public const int KNOWN_POINTER_WORDS = 2;
    private global::CapnpNet.Struct _s;
    public TestGroups(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestGroups(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestGroups(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestGroups(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public groupsGroup groups => new groupsGroup(_s);
    public struct groupsGroup
    {
      private readonly global::CapnpNet.Struct _s;
      public groupsGroup(global::CapnpNet.Struct s) { _s = s; }
      public enum Union : ushort
      {
        foo = 0,
        bar = 2,
        baz = 1,
      }

      public bool Is(out groupsGroup.fooGroup foo)
      {
        var ret = this.which == Union.foo;
        foo = new groupsGroup.fooGroup(ret ? _s : default (global::CapnpNet.Struct));
        return ret;
      }

      public bool Is(out groupsGroup.barGroup bar)
      {
        var ret = this.which == Union.bar;
        bar = new groupsGroup.barGroup(ret ? _s : default (global::CapnpNet.Struct));
        return ret;
      }

      public bool Is(out groupsGroup.bazGroup baz)
      {
        var ret = this.which == Union.baz;
        baz = new groupsGroup.bazGroup(ret ? _s : default (global::CapnpNet.Struct));
        return ret;
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(2); }
        set { _s.WriteUInt16(2, (ushort)value); }
      }

      public fooGroup foo => new fooGroup(_s);
      public struct fooGroup
      {
        private readonly global::CapnpNet.Struct _s;
        public fooGroup(global::CapnpNet.Struct s) { _s = s; }
        public int corge
        {
          get { return _s.ReadInt32(0); }
          set { _s.WriteInt32(0, value); }
        }

        public long grault
        {
          get { return _s.ReadInt64(1); }
          set { _s.WriteInt64(1, value); }
        }

        public global::CapnpNet.Text garply
        {
          get { return _s.DereferenceText(0); }
          set { _s.WritePointer(0, value); }
        }
      }

      public barGroup bar => new barGroup(_s);
      public struct barGroup
      {
        private readonly global::CapnpNet.Struct _s;
        public barGroup(global::CapnpNet.Struct s) { _s = s; }
        public int corge
        {
          get { return _s.ReadInt32(0); }
          set { _s.WriteInt32(0, value); }
        }

        public global::CapnpNet.Text grault
        {
          get { return _s.DereferenceText(0); }
          set { _s.WritePointer(0, value); }
        }

        public long garply
        {
          get { return _s.ReadInt64(1); }
          set { _s.WriteInt64(1, value); }
        }
      }

      public bazGroup baz => new bazGroup(_s);
      public struct bazGroup
      {
        private readonly global::CapnpNet.Struct _s;
        public bazGroup(global::CapnpNet.Struct s) { _s = s; }
        public int corge
        {
          get { return _s.ReadInt32(0); }
          set { _s.WriteInt32(0, value); }
        }

        public global::CapnpNet.Text grault
        {
          get { return _s.DereferenceText(0); }
          set { _s.WritePointer(0, value); }
        }

        public global::CapnpNet.Text garply
        {
          get { return _s.DereferenceText(1); }
          set { _s.WritePointer(1, value); }
        }
      }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestInterleavedGroups : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 6;
    public const int KNOWN_POINTER_WORDS = 6;
    private global::CapnpNet.Struct _s;
    public TestInterleavedGroups(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestInterleavedGroups(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestInterleavedGroups(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestInterleavedGroups(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public group1Group group1 => new group1Group(_s);
    public struct group1Group
    {
      private readonly global::CapnpNet.Struct _s;
      public group1Group(global::CapnpNet.Struct s) { _s = s; }
      public enum Union : ushort
      {
        qux = 0,
        corge = 1,
        fred = 2,
      }

      public bool Is(out group1Group.corgeGroup corge)
      {
        var ret = this.which == Union.corge;
        corge = new group1Group.corgeGroup(ret ? _s : default (global::CapnpNet.Struct));
        return ret;
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(14); }
        set { _s.WriteUInt16(14, (ushort)value); }
      }

      public uint foo
      {
        get { return _s.ReadUInt32(0); }
        set { _s.WriteUInt32(0, value); }
      }

      public ulong bar
      {
        get { return _s.ReadUInt64(1); }
        set { _s.WriteUInt64(1, value); }
      }

      public ushort qux
      {
        get { return _s.ReadUInt16(12); }
        set { _s.WriteUInt16(12, value); }
      }

      public corgeGroup corge => new corgeGroup(_s);
      public struct corgeGroup
      {
        private readonly global::CapnpNet.Struct _s;
        public corgeGroup(global::CapnpNet.Struct s) { _s = s; }
        public ulong grault
        {
          get { return _s.ReadUInt64(4); }
          set { _s.WriteUInt64(4, value); }
        }

        public ushort garply
        {
          get { return _s.ReadUInt16(12); }
          set { _s.WriteUInt16(12, value); }
        }

        public global::CapnpNet.Text plugh
        {
          get { return _s.DereferenceText(2); }
          set { _s.WritePointer(2, value); }
        }

        public global::CapnpNet.Text xyzzy
        {
          get { return _s.DereferenceText(4); }
          set { _s.WritePointer(4, value); }
        }
      }

      public global::CapnpNet.Text fred
      {
        get { return _s.DereferenceText(2); }
        set { _s.WritePointer(2, value); }
      }

      public global::CapnpNet.Text waldo
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    public group2Group group2 => new group2Group(_s);
    public struct group2Group
    {
      private readonly global::CapnpNet.Struct _s;
      public group2Group(global::CapnpNet.Struct s) { _s = s; }
      public enum Union : ushort
      {
        qux = 0,
        corge = 1,
        fred = 2,
      }

      public bool Is(out group2Group.corgeGroup corge)
      {
        var ret = this.which == Union.corge;
        corge = new group2Group.corgeGroup(ret ? _s : default (global::CapnpNet.Struct));
        return ret;
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(15); }
        set { _s.WriteUInt16(15, (ushort)value); }
      }

      public uint foo
      {
        get { return _s.ReadUInt32(1); }
        set { _s.WriteUInt32(1, value); }
      }

      public ulong bar
      {
        get { return _s.ReadUInt64(2); }
        set { _s.WriteUInt64(2, value); }
      }

      public ushort qux
      {
        get { return _s.ReadUInt16(13); }
        set { _s.WriteUInt16(13, value); }
      }

      public corgeGroup corge => new corgeGroup(_s);
      public struct corgeGroup
      {
        private readonly global::CapnpNet.Struct _s;
        public corgeGroup(global::CapnpNet.Struct s) { _s = s; }
        public ulong grault
        {
          get { return _s.ReadUInt64(5); }
          set { _s.WriteUInt64(5, value); }
        }

        public ushort garply
        {
          get { return _s.ReadUInt16(13); }
          set { _s.WriteUInt16(13, value); }
        }

        public global::CapnpNet.Text plugh
        {
          get { return _s.DereferenceText(3); }
          set { _s.WritePointer(3, value); }
        }

        public global::CapnpNet.Text xyzzy
        {
          get { return _s.DereferenceText(5); }
          set { _s.WritePointer(5, value); }
        }
      }

      public global::CapnpNet.Text fred
      {
        get { return _s.DereferenceText(3); }
        set { _s.WritePointer(3, value); }
      }

      public global::CapnpNet.Text waldo
      {
        get { return _s.DereferenceText(1); }
        set { _s.WritePointer(1, value); }
      }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestUnionDefaults : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 4;
    private global::CapnpNet.Struct _s;
    public TestUnionDefaults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestUnionDefaults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestUnionDefaults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestUnionDefaults(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public TestUnion s16s8s64s8Set
    {
      get { return _s.DereferenceStruct<TestUnion>(0); }
      set { _s.WritePointer(0, value); }
    }

    public TestUnion s0sps1s32Set
    {
      get { return _s.DereferenceStruct<TestUnion>(1); }
      set { _s.WritePointer(1, value); }
    }

    public TestUnnamedUnion unnamed1
    {
      get { return _s.DereferenceStruct<TestUnnamedUnion>(2); }
      set { _s.WritePointer(2, value); }
    }

    public TestUnnamedUnion unnamed2
    {
      get { return _s.DereferenceStruct<TestUnnamedUnion>(3); }
      set { _s.WritePointer(3, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestNestedTypes : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public TestNestedTypes(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestNestedTypes(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestNestedTypes(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestNestedTypes(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public NestedStruct nestedStruct
    {
      get { return _s.DereferenceStruct<NestedStruct>(0); }
      set { _s.WritePointer(0, value); }
    }

    public NestedEnum outerNestedEnum
    {
      get { return (NestedEnum)_s.ReadUInt16(0, 1); }
      set { _s.WriteUInt16(0, (ushort)value, 1); }
    }

    public NestedStruct.NestedEnum innerNestedEnum
    {
      get { return (NestedStruct.NestedEnum)_s.ReadUInt16(1, 2); }
      set { _s.WriteUInt16(1, (ushort)value, 2); }
    }

    public enum NestedEnum : ushort
    {
      foo = 0,
      bar = 1
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct NestedStruct : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 0;
      private global::CapnpNet.Struct _s;
      public NestedStruct(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public NestedStruct(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public NestedStruct(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public NestedStruct(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public NestedEnum outerNestedEnum
      {
        get { return (NestedEnum)_s.ReadUInt16(0, 1); }
        set { _s.WriteUInt16(0, (ushort)value, 1); }
      }

      public NestedEnum innerNestedEnum
      {
        get { return (NestedEnum)_s.ReadUInt16(1, 2); }
        set { _s.WriteUInt16(1, (ushort)value, 2); }
      }

      public enum NestedEnum : ushort
      {
        baz = 0,
        qux = 1,
        quux = 2
      }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestUsing : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public TestUsing(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestUsing(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestUsing(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestUsing(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public TestNestedTypes.NestedEnum outerNestedEnum
    {
      get { return (TestNestedTypes.NestedEnum)_s.ReadUInt16(1, 1); }
      set { _s.WriteUInt16(1, (ushort)value, 1); }
    }

    public TestNestedTypes.NestedStruct.NestedEnum innerNestedEnum
    {
      get { return (TestNestedTypes.NestedStruct.NestedEnum)_s.ReadUInt16(0, 2); }
      set { _s.WriteUInt16(0, (ushort)value, 2); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestLists : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 10;
    private global::CapnpNet.Struct _s;
    public TestLists(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestLists(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestLists(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestLists(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public global::CapnpNet.FlatArray<Struct0> list0
    {
      get { return new global::CapnpNet.FlatArray<Struct0>(_s.DereferenceAbsPointer(0)); }
      set { _s.WritePointer(0, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<Struct1> list1
    {
      get { return new global::CapnpNet.FlatArray<Struct1>(_s.DereferenceAbsPointer(1)); }
      set { _s.WritePointer(1, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<Struct8> list8
    {
      get { return new global::CapnpNet.FlatArray<Struct8>(_s.DereferenceAbsPointer(2)); }
      set { _s.WritePointer(2, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<Struct16> list16
    {
      get { return new global::CapnpNet.FlatArray<Struct16>(_s.DereferenceAbsPointer(3)); }
      set { _s.WritePointer(3, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<Struct32> list32
    {
      get { return new global::CapnpNet.FlatArray<Struct32>(_s.DereferenceAbsPointer(4)); }
      set { _s.WritePointer(4, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<Struct64> list64
    {
      get { return new global::CapnpNet.FlatArray<Struct64>(_s.DereferenceAbsPointer(5)); }
      set { _s.WritePointer(5, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<StructP> listP
    {
      get { return new global::CapnpNet.FlatArray<StructP>(_s.DereferenceAbsPointer(6)); }
      set { _s.WritePointer(6, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<global::CapnpNet.FlatArray<int>> int32ListList
    {
      get { return new global::CapnpNet.FlatArray<global::CapnpNet.FlatArray<int>>(_s.DereferenceAbsPointer(7)); }
      set { _s.WritePointer(7, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<global::CapnpNet.FlatArray<global::CapnpNet.Text>> textListList
    {
      get { return new global::CapnpNet.FlatArray<global::CapnpNet.FlatArray<global::CapnpNet.Text>>(_s.DereferenceAbsPointer(8)); }
      set { _s.WritePointer(8, value.Pointer); }
    }

    public global::CapnpNet.FlatArray<global::CapnpNet.FlatArray<TestAllTypes>> structListList
    {
      get { return new global::CapnpNet.FlatArray<global::CapnpNet.FlatArray<TestAllTypes>>(_s.DereferenceAbsPointer(9)); }
      set { _s.WritePointer(9, value.Pointer); }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Struct0 : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 0;
      private global::CapnpNet.Struct _s;
      public Struct0(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Struct0(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Struct0(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Struct0(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Struct1 : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 0;
      private global::CapnpNet.Struct _s;
      public Struct1(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Struct1(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Struct1(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Struct1(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public bool f
      {
        get { return _s.ReadBool(0); }
        set { _s.WriteBool(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Struct8 : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 0;
      private global::CapnpNet.Struct _s;
      public Struct8(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Struct8(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Struct8(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Struct8(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public byte f
      {
        get { return _s.ReadUInt8(0); }
        set { _s.WriteUInt8(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Struct16 : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 0;
      private global::CapnpNet.Struct _s;
      public Struct16(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Struct16(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Struct16(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Struct16(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ushort f
      {
        get { return _s.ReadUInt16(0); }
        set { _s.WriteUInt16(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Struct32 : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 0;
      private global::CapnpNet.Struct _s;
      public Struct32(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Struct32(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Struct32(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Struct32(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public uint f
      {
        get { return _s.ReadUInt32(0); }
        set { _s.WriteUInt32(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Struct64 : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 0;
      private global::CapnpNet.Struct _s;
      public Struct64(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Struct64(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Struct64(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Struct64(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ulong f
      {
        get { return _s.ReadUInt64(0); }
        set { _s.WriteUInt64(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct StructP : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public StructP(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public StructP(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public StructP(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public StructP(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public global::CapnpNet.Text f
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Struct0c : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public Struct0c(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Struct0c(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Struct0c(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Struct0c(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public global::CapnpNet.Text pad
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Struct1c : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public Struct1c(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Struct1c(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Struct1c(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Struct1c(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public bool f
      {
        get { return _s.ReadBool(0); }
        set { _s.WriteBool(0, value); }
      }

      public global::CapnpNet.Text pad
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Struct8c : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public Struct8c(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Struct8c(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Struct8c(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Struct8c(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public byte f
      {
        get { return _s.ReadUInt8(0); }
        set { _s.WriteUInt8(0, value); }
      }

      public global::CapnpNet.Text pad
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Struct16c : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public Struct16c(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Struct16c(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Struct16c(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Struct16c(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ushort f
      {
        get { return _s.ReadUInt16(0); }
        set { _s.WriteUInt16(0, value); }
      }

      public global::CapnpNet.Text pad
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Struct32c : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public Struct32c(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Struct32c(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Struct32c(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Struct32c(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public uint f
      {
        get { return _s.ReadUInt32(0); }
        set { _s.WriteUInt32(0, value); }
      }

      public global::CapnpNet.Text pad
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Struct64c : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public Struct64c(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Struct64c(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Struct64c(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Struct64c(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ulong f
      {
        get { return _s.ReadUInt64(0); }
        set { _s.WriteUInt64(0, value); }
      }

      public global::CapnpNet.Text pad
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct StructPc : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public StructPc(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public StructPc(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public StructPc(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public StructPc(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public global::CapnpNet.Text f
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }

      public ulong pad
      {
        get { return _s.ReadUInt64(0); }
        set { _s.WriteUInt64(0, value); }
      }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestFieldZeroIsBit : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public TestFieldZeroIsBit(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestFieldZeroIsBit(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestFieldZeroIsBit(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestFieldZeroIsBit(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public bool bit
    {
      get { return _s.ReadBool(0); }
      set { _s.WriteBool(0, value); }
    }

    public bool secondBit
    {
      get { return _s.ReadBool(1, true); }
      set { _s.WriteBool(1, value, true); }
    }

    public byte thirdField
    {
      get { return _s.ReadUInt8(1, 123); }
      set { _s.WriteUInt8(1, value, 123); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestListDefaults : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public TestListDefaults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestListDefaults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestListDefaults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestListDefaults(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public TestLists lists
    {
      get { return _s.DereferenceStruct<TestLists>(0); }
      set { _s.WritePointer(0, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestLateUnion : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 3;
    public const int KNOWN_POINTER_WORDS = 3;
    private global::CapnpNet.Struct _s;
    public TestLateUnion(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestLateUnion(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestLateUnion(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestLateUnion(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public int foo
    {
      get { return _s.ReadInt32(0); }
      set { _s.WriteInt32(0, value); }
    }

    public global::CapnpNet.Text bar
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }

    public short baz
    {
      get { return _s.ReadInt16(2); }
      set { _s.WriteInt16(2, value); }
    }

    public theUnionGroup theUnion => new theUnionGroup(_s);
    public struct theUnionGroup
    {
      private readonly global::CapnpNet.Struct _s;
      public theUnionGroup(global::CapnpNet.Struct s) { _s = s; }
      public enum Union : ushort
      {
        qux = 0,
        corge = 1,
        grault = 2,
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(3); }
        set { _s.WriteUInt16(3, (ushort)value); }
      }

      public global::CapnpNet.Text qux
      {
        get { return _s.DereferenceText(1); }
        set { _s.WritePointer(1, value); }
      }

      public global::CapnpNet.FlatArray<int> corge
      {
        get { return new global::CapnpNet.FlatArray<int>(_s.DereferenceAbsPointer(1)); }
        set { _s.WritePointer(1, value.Pointer); }
      }

      public float grault
      {
        get { return _s.ReadFloat32(2); }
        set { _s.WriteFloat32(2, value); }
      }
    }

    public anotherUnionGroup anotherUnion => new anotherUnionGroup(_s);
    public struct anotherUnionGroup
    {
      private readonly global::CapnpNet.Struct _s;
      public anotherUnionGroup(global::CapnpNet.Struct s) { _s = s; }
      public enum Union : ushort
      {
        qux = 0,
        corge = 1,
        grault = 2,
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(6); }
        set { _s.WriteUInt16(6, (ushort)value); }
      }

      public global::CapnpNet.Text qux
      {
        get { return _s.DereferenceText(2); }
        set { _s.WritePointer(2, value); }
      }

      public global::CapnpNet.FlatArray<int> corge
      {
        get { return new global::CapnpNet.FlatArray<int>(_s.DereferenceAbsPointer(2)); }
        set { _s.WritePointer(2, value.Pointer); }
      }

      public float grault
      {
        get { return _s.ReadFloat32(4); }
        set { _s.WriteFloat32(4, value); }
      }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestOldVersion : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 2;
    private global::CapnpNet.Struct _s;
    public TestOldVersion(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestOldVersion(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestOldVersion(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestOldVersion(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public long old1
    {
      get { return _s.ReadInt64(0); }
      set { _s.WriteInt64(0, value); }
    }

    public global::CapnpNet.Text old2
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }

    public TestOldVersion old3
    {
      get { return _s.DereferenceStruct<TestOldVersion>(1); }
      set { _s.WritePointer(1, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestNewVersion : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 2;
    public const int KNOWN_POINTER_WORDS = 3;
    private global::CapnpNet.Struct _s;
    public TestNewVersion(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestNewVersion(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestNewVersion(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestNewVersion(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public long old1
    {
      get { return _s.ReadInt64(0); }
      set { _s.WriteInt64(0, value); }
    }

    public global::CapnpNet.Text old2
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }

    public TestNewVersion old3
    {
      get { return _s.DereferenceStruct<TestNewVersion>(1); }
      set { _s.WritePointer(1, value); }
    }

    public long new1
    {
      get { return _s.ReadInt64(1, 987L); }
      set { _s.WriteInt64(1, value, 987L); }
    }

    public global::CapnpNet.Text new2
    {
      get { return _s.DereferenceText(2); }
      set { _s.WritePointer(2, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestOldUnionVersion : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 2;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public TestOldUnionVersion(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestOldUnionVersion(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestOldUnionVersion(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestOldUnionVersion(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public enum Union : ushort
    {
      a = 0,
      b = 1,
    }

    public Union which
    {
      get { return (Union)_s.ReadUInt16(0); }
      set { _s.WriteUInt16(0, (ushort)value); }
    }

    public ulong b
    {
      get { return _s.ReadUInt64(1); }
      set { _s.WriteUInt64(1, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestNewUnionVersion : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 3;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public TestNewUnionVersion(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestNewUnionVersion(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestNewUnionVersion(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestNewUnionVersion(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public enum Union : ushort
    {
      a = 0,
      b = 1,
    }

    public bool Is(out aGroup a)
    {
      var ret = this.which == Union.a;
      a = new aGroup(ret ? _s : default (global::CapnpNet.Struct));
      return ret;
    }

    public Union which
    {
      get { return (Union)_s.ReadUInt16(0); }
      set { _s.WriteUInt16(0, (ushort)value); }
    }

    public aGroup a => new aGroup(_s);
    public struct aGroup
    {
      private readonly global::CapnpNet.Struct _s;
      public aGroup(global::CapnpNet.Struct s) { _s = s; }
      public enum Union : ushort
      {
        a0 = 0,
        a1 = 1,
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(4); }
        set { _s.WriteUInt16(4, (ushort)value); }
      }

      public ulong a1
      {
        get { return _s.ReadUInt64(2); }
        set { _s.WriteUInt64(2, value); }
      }
    }

    public ulong b
    {
      get { return _s.ReadUInt64(1); }
      set { _s.WriteUInt64(1, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestStructUnion : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public TestStructUnion(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestStructUnion(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestStructUnion(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestStructUnion(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public unGroup un => new unGroup(_s);
    public struct unGroup
    {
      private readonly global::CapnpNet.Struct _s;
      public unGroup(global::CapnpNet.Struct s) { _s = s; }
      public enum Union : ushort
      {
        @struct = 0,
        @object = 1,
      }

      public bool Is(out SomeStruct @struct)
      {
        var ret = this.which == Union.@struct;
        @struct = ret ? this.@struct : default (SomeStruct);
        return ret;
      }

      public bool Is(out TestAnyPointer @object)
      {
        var ret = this.which == Union.@object;
        @object = ret ? this.@object : default (TestAnyPointer);
        return ret;
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(0); }
        set { _s.WriteUInt16(0, (ushort)value); }
      }

      public SomeStruct @struct
      {
        get { return _s.DereferenceStruct<SomeStruct>(0); }
        set { _s.WritePointer(0, value); }
      }

      public TestAnyPointer @object
      {
        get { return _s.DereferenceStruct<TestAnyPointer>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct SomeStruct : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 2;
      private global::CapnpNet.Struct _s;
      public SomeStruct(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public SomeStruct(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public SomeStruct(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public SomeStruct(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public global::CapnpNet.Text someText
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }

      public global::CapnpNet.Text moreText
      {
        get { return _s.DereferenceText(1); }
        set { _s.WritePointer(1, value); }
      }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestPrintInlineStructs : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 2;
    private global::CapnpNet.Struct _s;
    public TestPrintInlineStructs(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestPrintInlineStructs(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestPrintInlineStructs(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestPrintInlineStructs(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public global::CapnpNet.Text someText
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }

    public global::CapnpNet.FlatArray<InlineStruct> structList
    {
      get { return new global::CapnpNet.FlatArray<InlineStruct>(_s.DereferenceAbsPointer(1)); }
      set { _s.WritePointer(1, value.Pointer); }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct InlineStruct : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public InlineStruct(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public InlineStruct(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public InlineStruct(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public InlineStruct(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public int int32Field
      {
        get { return _s.ReadInt32(0); }
        set { _s.WriteInt32(0, value); }
      }

      public global::CapnpNet.Text textField
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestWholeFloatDefault : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public TestWholeFloatDefault(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestWholeFloatDefault(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestWholeFloatDefault(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestWholeFloatDefault(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public float field
    {
      get { return _s.ReadFloat32(0, 123F); }
      set { _s.WriteFloat32(0, value, 123F); }
    }

    public float bigField
    {
      get { return _s.ReadFloat32(1, 2E+30F); }
      set { _s.WriteFloat32(1, value, 2E+30F); }
    }

    public const float constant = 456F;
    public const float bigConstant = 4E+30F;
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestGenerics<Foo, Bar> : global::CapnpNet.IStruct where Foo : global::CapnpNet.IAbsPointer where Bar : global::CapnpNet.IAbsPointer
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 2;
    private global::CapnpNet.Struct _s;
    public TestGenerics(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestGenerics(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestGenerics(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestGenerics(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public enum Union : ushort
    {
      uv = 0,
      ug = 1,
    }

    public bool Is(out ugGroup ug)
    {
      var ret = this.which == Union.ug;
      ug = new ugGroup(ret ? _s : default (global::CapnpNet.Struct));
      return ret;
    }

    public Union which
    {
      get { return (Union)_s.ReadUInt16(0); }
      set { _s.WriteUInt16(0, (ushort)value); }
    }

    public Foo foo
    {
      get { return _s.DereferencePointer<Foo>(0); }
      set { _s.WritePointer(0, value); }
    }

    public TestGenerics<Bar, Foo> rev
    {
      get { return _s.DereferenceStruct<TestGenerics<Bar, Foo>>(1); }
      set { _s.WritePointer(1, value); }
    }

    public ugGroup ug => new ugGroup(_s);
    public struct ugGroup
    {
      private readonly global::CapnpNet.Struct _s;
      public ugGroup(global::CapnpNet.Struct s) { _s = s; }
      public int ugfoo
      {
        get { return _s.ReadInt32(1); }
        set { _s.WriteInt32(1, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Inner : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 2;
      private global::CapnpNet.Struct _s;
      public Inner(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Inner(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Inner(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Inner(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public Foo foo
      {
        get { return _s.DereferencePointer<Foo>(0); }
        set { _s.WritePointer(0, value); }
      }

      public Bar bar
      {
        get { return _s.DereferencePointer<Bar>(1); }
        set { _s.WritePointer(1, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Inner2<Baz> : global::CapnpNet.IStruct where Baz : global::CapnpNet.IAbsPointer
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 4;
      private global::CapnpNet.Struct _s;
      public Inner2(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Inner2(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Inner2(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Inner2(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public Bar bar
      {
        get { return _s.DereferencePointer<Bar>(0); }
        set { _s.WritePointer(0, value); }
      }

      public Baz baz
      {
        get { return _s.DereferencePointer<Baz>(1); }
        set { _s.WritePointer(1, value); }
      }

      public Inner innerBound
      {
        get { return _s.DereferenceStruct<Inner>(2); }
        set { _s.WritePointer(2, value); }
      }

      public Inner innerUnbound
      {
        get { return _s.DereferenceStruct<Inner>(3); }
        set { _s.WritePointer(3, value); }
      }

      [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
      public struct DeepNest<Qux> : global::CapnpNet.IStruct where Qux : global::CapnpNet.IAbsPointer
      {
        public const int KNOWN_DATA_WORDS = 0;
        public const int KNOWN_POINTER_WORDS = 4;
        private global::CapnpNet.Struct _s;
        public DeepNest(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
        {
        }

        public DeepNest(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
        {
        }

        public DeepNest(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
        {
        }

        public DeepNest(global::CapnpNet.Struct s) { _s = s; }
        global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
        {
          get { return _s; }
        }

        global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
        {
          get { return _s.Pointer; }
        }

        public Foo foo
        {
          get { return _s.DereferencePointer<Foo>(0); }
          set { _s.WritePointer(0, value); }
        }

        public Bar bar
        {
          get { return _s.DereferencePointer<Bar>(1); }
          set { _s.WritePointer(1, value); }
        }

        public Baz baz
        {
          get { return _s.DereferencePointer<Baz>(2); }
          set { _s.WritePointer(2, value); }
        }

        public Qux qux
        {
          get { return _s.DereferencePointer<Qux>(3); }
          set { _s.WritePointer(3, value); }
        }

        public interface IDeepNestInterface<Quux> : global::CapnpNet.Rpc.ICapability where Quux : global::CapnpNet.IAbsPointer
        {
          [global::CapnpNet.Ordinal(0)]
          void call();
        }
      }
    }

    public interface IInterface<Qux> : global::CapnpNet.Rpc.ICapability where Qux : global::CapnpNet.IAbsPointer
    {
      [global::CapnpNet.Ordinal(0)]
      Interface<Qux>.callResults call(Inner2<global::CapnpNet.Text> parameters);
    }

    public static class Interface<Qux>
      where Qux : global::CapnpNet.IAbsPointer
    {
      [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
      public struct callResults : global::CapnpNet.IStruct
      {
        public const int KNOWN_DATA_WORDS = 0;
        public const int KNOWN_POINTER_WORDS = 2;
        private global::CapnpNet.Struct _s;
        public callResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
        {
        }

        public callResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
        {
        }

        public callResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
        {
        }

        public callResults(global::CapnpNet.Struct s) { _s = s; }
        global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
        {
          get { return _s; }
        }

        global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
        {
          get { return _s.Pointer; }
        }

        public Qux qux
        {
          get { return _s.DereferencePointer<Qux>(0); }
          set { _s.WritePointer(0, value); }
        }

        public TestGenerics<TestAllTypes, TestAnyPointer> gen
        {
          get { return _s.DereferenceStruct<TestGenerics<TestAllTypes, TestAnyPointer>>(1); }
          set { _s.WritePointer(1, value); }
        }
      }
    }

    /*TODO*/
    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct UseAliases : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 6;
      private global::CapnpNet.Struct _s;
      public UseAliases(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public UseAliases(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public UseAliases(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public UseAliases(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public Foo foo
      {
        get { return _s.DereferencePointer<Foo>(0); }
        set { _s.WritePointer(0, value); }
      }

      public Inner inner
      {
        get { return _s.DereferenceStruct<Inner>(1); }
        set { _s.WritePointer(1, value); }
      }

      public Inner2<global::CapnpNet.AbsPointer> inner2
      {
        get { return _s.DereferenceStruct<Inner2<global::CapnpNet.AbsPointer>>(2); }
        set { _s.WritePointer(2, value); }
      }

      public Inner2<global::CapnpNet.Text> inner2Bind
      {
        get { return _s.DereferenceStruct<Inner2<global::CapnpNet.Text>>(3); }
        set { _s.WritePointer(3, value); }
      }

      public Inner2<global::CapnpNet.Text> inner2Text
      {
        get { return _s.DereferenceStruct<Inner2<global::CapnpNet.Text>>(4); }
        set { _s.WritePointer(4, value); }
      }

      public Bar revFoo
      {
        get { return _s.DereferencePointer<Bar>(5); }
        set { _s.WritePointer(5, value); }
      }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestGenericsWrapper<Foo, Bar> : global::CapnpNet.IStruct where Foo : global::CapnpNet.IAbsPointer where Bar : global::CapnpNet.IAbsPointer
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public TestGenericsWrapper(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestGenericsWrapper(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestGenericsWrapper(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestGenericsWrapper(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public TestGenerics<Foo, Bar> value
    {
      get { return _s.DereferenceStruct<TestGenerics<Foo, Bar>>(0); }
      set { _s.WritePointer(0, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestGenericsWrapper2 : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public TestGenericsWrapper2(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestGenericsWrapper2(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestGenericsWrapper2(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestGenericsWrapper2(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public TestGenericsWrapper<global::CapnpNet.Text, TestAllTypes> value
    {
      get { return _s.DereferenceStruct<TestGenericsWrapper<global::CapnpNet.Text, TestAllTypes>>(0); }
      set { _s.WritePointer(0, value); }
    }
  }

  public interface ITestImplicitMethodParams : global::CapnpNet.Rpc.ICapability
  {
    [global::CapnpNet.Ordinal(0)]
    TestGenerics<T, U> call<T, U>(TestImplicitMethodParams.callParams<global::CapnpNet.AbsPointer, global::CapnpNet.AbsPointer> parameters)where T : global::CapnpNet.IAbsPointer where U : global::CapnpNet.IAbsPointer;
  }

  public static class TestImplicitMethodParams
  {
    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct callParams<T, U> : global::CapnpNet.IStruct where T : global::CapnpNet.IAbsPointer where U : global::CapnpNet.IAbsPointer
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 2;
      private global::CapnpNet.Struct _s;
      public callParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public callParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public callParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public callParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public T foo
      {
        get { return _s.DereferencePointer<T>(0); }
        set { _s.WritePointer(0, value); }
      }

      public U bar
      {
        get { return _s.DereferencePointer<U>(1); }
        set { _s.WritePointer(1, value); }
      }
    }
  }

  public interface ITestImplicitMethodParamsInGeneric<V> : global::CapnpNet.Rpc.ICapability where V : global::CapnpNet.IAbsPointer
  {
    [global::CapnpNet.Ordinal(0)]
    TestGenerics<T, U> call<T, U>(TestImplicitMethodParamsInGeneric<V>.callParams<global::CapnpNet.AbsPointer, global::CapnpNet.AbsPointer> parameters)where T : global::CapnpNet.IAbsPointer where U : global::CapnpNet.IAbsPointer;
  }

  public static class TestImplicitMethodParamsInGeneric<V>
    where V : global::CapnpNet.IAbsPointer
  {
    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct callParams<T, U> : global::CapnpNet.IStruct where T : global::CapnpNet.IAbsPointer where U : global::CapnpNet.IAbsPointer
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 2;
      private global::CapnpNet.Struct _s;
      public callParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public callParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public callParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public callParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public T foo
      {
        get { return _s.DereferencePointer<T>(0); }
        set { _s.WritePointer(0, value); }
      }

      public U bar
      {
        get { return _s.DereferencePointer<U>(1); }
        set { _s.WritePointer(1, value); }
      }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestGenericsUnion<Foo, Bar> : global::CapnpNet.IStruct where Foo : global::CapnpNet.IAbsPointer where Bar : global::CapnpNet.IAbsPointer
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public TestGenericsUnion(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestGenericsUnion(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestGenericsUnion(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestGenericsUnion(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public enum Union : ushort
    {
      foo = 0,
      bar = 1,
    }

    public Union which
    {
      get { return (Union)_s.ReadUInt16(0); }
      set { _s.WriteUInt16(0, (ushort)value); }
    }

    public Foo foo
    {
      get { return _s.DereferencePointer<Foo>(0); }
      set { _s.WritePointer(0, value); }
    }

    public Bar bar
    {
      get { return _s.DereferencePointer<Bar>(0); }
      set { _s.WritePointer(0, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestUseGenerics : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 20;
    private global::CapnpNet.Struct _s;
    public TestUseGenerics(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestUseGenerics(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestUseGenerics(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestUseGenerics(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public TestGenerics<TestAllTypes, TestAnyPointer> basic
    {
      get { return _s.DereferenceStruct<TestGenerics<TestAllTypes, TestAnyPointer>>(0); }
      set { _s.WritePointer(0, value); }
    }

    public TestGenerics<TestAllTypes, TestAnyPointer>.Inner inner
    {
      get { return _s.DereferenceStruct<TestGenerics<TestAllTypes, TestAnyPointer>.Inner>(1); }
      set { _s.WritePointer(1, value); }
    }

    public TestGenerics<TestAllTypes, TestAnyPointer>.Inner2<global::CapnpNet.Text> inner2
    {
      get { return _s.DereferenceStruct<TestGenerics<TestAllTypes, TestAnyPointer>.Inner2<global::CapnpNet.Text>>(2); }
      set { _s.WritePointer(2, value); }
    }

    public TestGenerics<global::CapnpNet.AbsPointer, global::CapnpNet.AbsPointer> unspecified
    {
      get { return _s.DereferenceStruct<TestGenerics<global::CapnpNet.AbsPointer, global::CapnpNet.AbsPointer>>(3); }
      set { _s.WritePointer(3, value); }
    }

    public TestGenerics<global::CapnpNet.AbsPointer, global::CapnpNet.AbsPointer>.Inner2<global::CapnpNet.Text> unspecifiedInner
    {
      get { return _s.DereferenceStruct<TestGenerics<global::CapnpNet.AbsPointer, global::CapnpNet.AbsPointer>.Inner2<global::CapnpNet.Text>>(4); }
      set { _s.WritePointer(4, value); }
    }

    public TestGenericsWrapper<TestAllTypes, TestAnyPointer> wrapper
    {
      get { return _s.DereferenceStruct<TestGenericsWrapper<TestAllTypes, TestAnyPointer>>(8); }
      set { _s.WritePointer(8, value); }
    }

    public TestGenerics<ITestInterface, global::CapnpNet.Text> cap
    {
      get { return _s.DereferenceStruct<TestGenerics<ITestInterface, global::CapnpNet.Text>>(18); }
      set { _s.WritePointer(18, value); }
    }

    public TestGenerics<TestAllTypes, global::CapnpNet.FlatArray<uint>>.IInterface<global::CapnpNet.FlatArray<byte>> genericCap
    {
      get { return _s.DereferencePointer<TestGenerics<TestAllTypes, global::CapnpNet.FlatArray<uint>>.IInterface<global::CapnpNet.FlatArray<byte>>>(19); }
      set { _s.WritePointer(19, value); }
    }

    public TestGenerics<TestAllTypes, global::CapnpNet.Text> @default
    {
      get { return _s.DereferenceStruct<TestGenerics<TestAllTypes, global::CapnpNet.Text>>(5); }
      set { _s.WritePointer(5, value); }
    }

    public TestGenerics<TestAllTypes, global::CapnpNet.Text>.Inner defaultInner
    {
      get { return _s.DereferenceStruct<TestGenerics<TestAllTypes, global::CapnpNet.Text>.Inner>(6); }
      set { _s.WritePointer(6, value); }
    }

    public TestUseGenerics defaultUser
    {
      get { return _s.DereferenceStruct<TestUseGenerics>(7); }
      set { _s.WritePointer(7, value); }
    }

    public TestGenericsWrapper<global::CapnpNet.Text, TestAllTypes> defaultWrapper
    {
      get { return _s.DereferenceStruct<TestGenericsWrapper<global::CapnpNet.Text, TestAllTypes>>(9); }
      set { _s.WritePointer(9, value); }
    }

    public TestGenericsWrapper2 defaultWrapper2
    {
      get { return _s.DereferenceStruct<TestGenericsWrapper2>(10); }
      set { _s.WritePointer(10, value); }
    }

    public TestAllTypes aliasFoo
    {
      get { return _s.DereferenceStruct<TestAllTypes>(11); }
      set { _s.WritePointer(11, value); }
    }

    public TestGenerics<TestAllTypes, TestAnyPointer>.Inner aliasInner
    {
      get { return _s.DereferenceStruct<TestGenerics<TestAllTypes, TestAnyPointer>.Inner>(12); }
      set { _s.WritePointer(12, value); }
    }

    public TestGenerics<TestAllTypes, TestAnyPointer>.Inner2<global::CapnpNet.AbsPointer> aliasInner2
    {
      get { return _s.DereferenceStruct<TestGenerics<TestAllTypes, TestAnyPointer>.Inner2<global::CapnpNet.AbsPointer>>(13); }
      set { _s.WritePointer(13, value); }
    }

    public TestGenerics<TestAllTypes, TestAnyPointer>.Inner2<global::CapnpNet.FlatArray<uint>> aliasInner2Bind
    {
      get { return _s.DereferenceStruct<TestGenerics<TestAllTypes, TestAnyPointer>.Inner2<global::CapnpNet.FlatArray<uint>>>(14); }
      set { _s.WritePointer(14, value); }
    }

    public TestGenerics<TestAllTypes, TestAnyPointer>.Inner2<global::CapnpNet.Text> aliasInner2Text
    {
      get { return _s.DereferenceStruct<TestGenerics<TestAllTypes, TestAnyPointer>.Inner2<global::CapnpNet.Text>>(15); }
      set { _s.WritePointer(15, value); }
    }

    public global::CapnpNet.Text aliasRev
    {
      get { return _s.DereferenceText(16); }
      set { _s.WritePointer(16, value); }
    }

    public TestGenerics<TestAllTypes, global::CapnpNet.FlatArray<uint>>.UseAliases useAliases
    {
      get { return _s.DereferenceStruct<TestGenerics<TestAllTypes, global::CapnpNet.FlatArray<uint>>.UseAliases>(17); }
      set { _s.WritePointer(17, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestEmptyStruct : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public TestEmptyStruct(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestEmptyStruct(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestEmptyStruct(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestEmptyStruct(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestConstants : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public TestConstants(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestConstants(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestConstants(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestConstants(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public static readonly global::CapnpNet.Void voidConst = default (global::CapnpNet.Void) /*not yet supported*/;
    public const bool boolConst = true;
    public const sbyte int8Const = -123;
    public const short int16Const = -12345;
    public const int int32Const = -12345678;
    public const long int64Const = -123456789012345L;
    public const byte uint8Const = 234;
    public const ushort uint16Const = 45678;
    public const uint uint32Const = 3456789012U;
    public const ulong uint64Const = 12345678901234567890UL;
    public const float float32Const = 1234.5F;
    public const double float64Const = -1.23E+47;
    public static readonly global::CapnpNet.Text textConst = default (global::CapnpNet.Text) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<byte> dataConst = default (global::CapnpNet.FlatArray<byte>) /*not yet supported*/;
    public static readonly TestAllTypes structConst = default (TestAllTypes) /*not yet supported*/;
    public const TestEnum enumConst = (TestEnum)5;
    public static readonly global::CapnpNet.FlatArray<global::CapnpNet.Void> voidListConst = default (global::CapnpNet.FlatArray<global::CapnpNet.Void>) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<bool> boolListConst = default (global::CapnpNet.FlatArray<bool>) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<sbyte> int8ListConst = default (global::CapnpNet.FlatArray<sbyte>) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<short> int16ListConst = default (global::CapnpNet.FlatArray<short>) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<int> int32ListConst = default (global::CapnpNet.FlatArray<int>) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<long> int64ListConst = default (global::CapnpNet.FlatArray<long>) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<byte> uint8ListConst = default (global::CapnpNet.FlatArray<byte>) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<ushort> uint16ListConst = default (global::CapnpNet.FlatArray<ushort>) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<uint> uint32ListConst = default (global::CapnpNet.FlatArray<uint>) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<ulong> uint64ListConst = default (global::CapnpNet.FlatArray<ulong>) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<float> float32ListConst = default (global::CapnpNet.FlatArray<float>) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<double> float64ListConst = default (global::CapnpNet.FlatArray<double>) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<global::CapnpNet.Text> textListConst = default (global::CapnpNet.FlatArray<global::CapnpNet.Text>) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<global::CapnpNet.FlatArray<byte>> dataListConst = default (global::CapnpNet.FlatArray<global::CapnpNet.FlatArray<byte>>) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<TestAllTypes> structListConst = default (global::CapnpNet.FlatArray<TestAllTypes>) /*not yet supported*/;
    public static readonly global::CapnpNet.FlatArray<TestEnum> enumListConst = default (global::CapnpNet.FlatArray<TestEnum>) /*not yet supported*/;
  }

  public static partial class GlobalConstants
  {
    public const uint globalInt = 12345U;
  }

  public static partial class GlobalConstants
  {
    public static readonly global::CapnpNet.Text globalText = default (global::CapnpNet.Text) /*not yet supported*/;
  }

  public static partial class GlobalConstants
  {
    public static readonly TestAllTypes globalStruct = default (TestAllTypes) /*not yet supported*/;
  }

  public static partial class GlobalConstants
  {
    public static readonly TestPrintInlineStructs globalPrintableStruct = default (TestPrintInlineStructs) /*not yet supported*/;
  }

  public static partial class GlobalConstants
  {
    public static readonly TestAllTypes derivedConstant = default (TestAllTypes) /*not yet supported*/;
  }

  public static partial class GlobalConstants
  {
    public static readonly TestGenerics<TestAllTypes, global::CapnpNet.Text> genericConstant = default (TestGenerics<TestAllTypes, global::CapnpNet.Text>) /*not yet supported*/;
  }

  public static partial class GlobalConstants
  {
    public static readonly global::CapnpNet.FlatArray<byte> embeddedData = default (global::CapnpNet.FlatArray<byte>) /*not yet supported*/;
  }

  public static partial class GlobalConstants
  {
    public static readonly global::CapnpNet.Text embeddedText = default (global::CapnpNet.Text) /*not yet supported*/;
  }

  public static partial class GlobalConstants
  {
    public static readonly TestAllTypes embeddedStruct = default (TestAllTypes) /*not yet supported*/;
  }

  public static partial class GlobalConstants
  {
    public static readonly global::CapnpNet.Text nonAsciiText = default (global::CapnpNet.Text) /*not yet supported*/;
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestAnyPointerConstants : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 4;
    private global::CapnpNet.Struct _s;
    public TestAnyPointerConstants(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestAnyPointerConstants(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestAnyPointerConstants(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestAnyPointerConstants(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public global::CapnpNet.AbsPointer anyKindAsStruct
    {
      get { return _s.DereferenceAbsPointer(0); }
      set { _s.WritePointer(0, value); }
    }

    public global::CapnpNet.Struct anyStructAsStruct
    {
      get { return _s.DereferenceRawStruct(1); }
      set { _s.WritePointer(1, value); }
    }

    public global::CapnpNet.AbsPointer anyKindAsList
    {
      get { return _s.DereferenceAbsPointer(2); }
      set { _s.WritePointer(2, value); }
    }

    public global::CapnpNet.AbsPointer anyListAsList
    {
      get { return _s.DereferenceAbsPointer(3); }
      set { _s.WritePointer(3, value); }
    }
  }

  public static partial class GlobalConstants
  {
    public static readonly TestAnyPointerConstants anyPointerConstants = default (TestAnyPointerConstants) /*not yet supported*/;
  }

  public interface ITestInterface : global::CapnpNet.Rpc.ICapability
  {
    [global::CapnpNet.Ordinal(0)]
    TestInterface.fooResults foo(TestInterface.fooParams parameters);
    [global::CapnpNet.Ordinal(1)]
    void bar();
    [global::CapnpNet.Ordinal(2)]
    void baz(TestInterface.bazParams parameters);
  }

  public static class TestInterface
  {
    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct fooParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 0;
      private global::CapnpNet.Struct _s;
      public fooParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public fooParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public fooParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public fooParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public uint i
      {
        get { return _s.ReadUInt32(0); }
        set { _s.WriteUInt32(0, value); }
      }

      public bool j
      {
        get { return _s.ReadBool(32); }
        set { _s.WriteBool(32, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct fooResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public fooResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public fooResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public fooResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public fooResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public global::CapnpNet.Text x
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct bazParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public bazParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public bazParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public bazParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public bazParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public TestAllTypes s
      {
        get { return _s.DereferenceStruct<TestAllTypes>(0); }
        set { _s.WritePointer(0, value); }
      }
    }
  }

  public interface ITestExtends : global::CapnpNet.Rpc.ICapability, ITestInterface
  {
    [global::CapnpNet.Ordinal(0)]
    void qux();
    [global::CapnpNet.Ordinal(1)]
    void corge(TestAllTypes parameters);
    [global::CapnpNet.Ordinal(2)]
    TestAllTypes grault();
  }

  public interface ITestExtends2 : global::CapnpNet.Rpc.ICapability, ITestExtends
  {
  }

  public interface ITestPipeline : global::CapnpNet.Rpc.ICapability
  {
    [global::CapnpNet.Ordinal(0)]
    TestPipeline.getCapResults getCap(TestPipeline.getCapParams parameters);
    [global::CapnpNet.Ordinal(1)]
    void testPointers(TestPipeline.testPointersParams parameters);
  }

  public static class TestPipeline
  {
    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct getCapParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public getCapParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public getCapParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public getCapParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public getCapParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public uint n
      {
        get { return _s.ReadUInt32(0); }
        set { _s.WriteUInt32(0, value); }
      }

      public ITestInterface inCap
      {
        get { return _s.DereferencePointer<ITestInterface>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct getCapResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 2;
      private global::CapnpNet.Struct _s;
      public getCapResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public getCapResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public getCapResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public getCapResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public global::CapnpNet.Text s
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }

      public Box outBox
      {
        get { return _s.DereferenceStruct<Box>(1); }
        set { _s.WritePointer(1, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct testPointersParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 3;
      private global::CapnpNet.Struct _s;
      public testPointersParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public testPointersParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public testPointersParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public testPointersParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ITestInterface cap
      {
        get { return _s.DereferencePointer<ITestInterface>(0); }
        set { _s.WritePointer(0, value); }
      }

      public global::CapnpNet.AbsPointer obj
      {
        get { return _s.DereferenceAbsPointer(1); }
        set { _s.WritePointer(1, value); }
      }

      public global::CapnpNet.FlatArray<ITestInterface> list
      {
        get { return new global::CapnpNet.FlatArray<ITestInterface>(_s.DereferenceAbsPointer(2)); }
        set { _s.WritePointer(2, value.Pointer); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Box : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public Box(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Box(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Box(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Box(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ITestInterface cap
      {
        get { return _s.DereferencePointer<ITestInterface>(0); }
        set { _s.WritePointer(0, value); }
      }
    }
  }

  public interface ITestCallOrder : global::CapnpNet.Rpc.ICapability
  {
    [global::CapnpNet.Ordinal(0)]
    TestCallOrder.getCallSequenceResults getCallSequence(TestCallOrder.getCallSequenceParams parameters);
  }

  public static class TestCallOrder
  {
    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct getCallSequenceParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 0;
      private global::CapnpNet.Struct _s;
      public getCallSequenceParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public getCallSequenceParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public getCallSequenceParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public getCallSequenceParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public uint expected
      {
        get { return _s.ReadUInt32(0); }
        set { _s.WriteUInt32(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct getCallSequenceResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 0;
      private global::CapnpNet.Struct _s;
      public getCallSequenceResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public getCallSequenceResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public getCallSequenceResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public getCallSequenceResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public uint n
      {
        get { return _s.ReadUInt32(0); }
        set { _s.WriteUInt32(0, value); }
      }
    }
  }

  public interface ITestTailCallee : global::CapnpNet.Rpc.ICapability
  {
    [global::CapnpNet.Ordinal(0)]
    TestTailCallee.TailResult foo(TestTailCallee.fooParams parameters);
  }

  public static class TestTailCallee
  {
    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct fooParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public fooParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public fooParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public fooParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public fooParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public int i
      {
        get { return _s.ReadInt32(0); }
        set { _s.WriteInt32(0, value); }
      }

      public global::CapnpNet.Text t
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct TailResult : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 2;
      private global::CapnpNet.Struct _s;
      public TailResult(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public TailResult(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public TailResult(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public TailResult(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public uint i
      {
        get { return _s.ReadUInt32(0); }
        set { _s.WriteUInt32(0, value); }
      }

      public global::CapnpNet.Text t
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }

      public ITestCallOrder c
      {
        get { return _s.DereferencePointer<ITestCallOrder>(1); }
        set { _s.WritePointer(1, value); }
      }
    }
  }

  public interface ITestTailCaller : global::CapnpNet.Rpc.ICapability
  {
    [global::CapnpNet.Ordinal(0)]
    TestTailCallee.TailResult foo(TestTailCaller.fooParams parameters);
  }

  public static class TestTailCaller
  {
    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct fooParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public fooParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public fooParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public fooParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public fooParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public int i
      {
        get { return _s.ReadInt32(0); }
        set { _s.WriteInt32(0, value); }
      }

      public ITestTailCallee callee
      {
        get { return _s.DereferencePointer<ITestTailCallee>(0); }
        set { _s.WritePointer(0, value); }
      }
    }
  }

  public interface ITestHandle : global::CapnpNet.Rpc.ICapability
  {
  }

  public interface ITestMoreStuff : global::CapnpNet.Rpc.ICapability, ITestCallOrder
  {
    [global::CapnpNet.Ordinal(0)]
    TestMoreStuff.callFooResults callFoo(TestMoreStuff.callFooParams parameters);
    [global::CapnpNet.Ordinal(1)]
    TestMoreStuff.callFooWhenResolvedResults callFooWhenResolved(TestMoreStuff.callFooWhenResolvedParams parameters);
    [global::CapnpNet.Ordinal(2)]
    TestMoreStuff.neverReturnResults neverReturn(TestMoreStuff.neverReturnParams parameters);
    [global::CapnpNet.Ordinal(3)]
    void hold(TestMoreStuff.holdParams parameters);
    [global::CapnpNet.Ordinal(4)]
    TestMoreStuff.callHeldResults callHeld();
    [global::CapnpNet.Ordinal(5)]
    TestMoreStuff.getHeldResults getHeld();
    [global::CapnpNet.Ordinal(6)]
    TestMoreStuff.echoResults echo(TestMoreStuff.echoParams parameters);
    [global::CapnpNet.Ordinal(7)]
    void expectCancel(TestMoreStuff.expectCancelParams parameters);
    [global::CapnpNet.Ordinal(8)]
    TestMoreStuff.methodWithDefaultsResults methodWithDefaults(TestMoreStuff.methodWithDefaultsParams parameters);
    [global::CapnpNet.Ordinal(9)]
    TestMoreStuff.getHandleResults getHandle();
    [global::CapnpNet.Ordinal(10)]
    TestMoreStuff.getNullResults getNull();
    [global::CapnpNet.Ordinal(11)]
    TestMoreStuff.getEnormousStringResults getEnormousString();
  }

  public static class TestMoreStuff
  {
    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct callFooParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public callFooParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public callFooParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public callFooParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public callFooParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ITestInterface cap
      {
        get { return _s.DereferencePointer<ITestInterface>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct callFooResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public callFooResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public callFooResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public callFooResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public callFooResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public global::CapnpNet.Text s
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct callFooWhenResolvedParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public callFooWhenResolvedParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public callFooWhenResolvedParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public callFooWhenResolvedParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public callFooWhenResolvedParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ITestInterface cap
      {
        get { return _s.DereferencePointer<ITestInterface>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct callFooWhenResolvedResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public callFooWhenResolvedResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public callFooWhenResolvedResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public callFooWhenResolvedResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public callFooWhenResolvedResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public global::CapnpNet.Text s
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct neverReturnParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public neverReturnParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public neverReturnParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public neverReturnParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public neverReturnParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ITestInterface cap
      {
        get { return _s.DereferencePointer<ITestInterface>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct neverReturnResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public neverReturnResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public neverReturnResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public neverReturnResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public neverReturnResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ITestInterface capCopy
      {
        get { return _s.DereferencePointer<ITestInterface>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct holdParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public holdParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public holdParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public holdParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public holdParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ITestInterface cap
      {
        get { return _s.DereferencePointer<ITestInterface>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct callHeldResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public callHeldResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public callHeldResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public callHeldResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public callHeldResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public global::CapnpNet.Text s
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct getHeldResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public getHeldResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public getHeldResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public getHeldResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public getHeldResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ITestInterface cap
      {
        get { return _s.DereferencePointer<ITestInterface>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct echoParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public echoParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public echoParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public echoParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public echoParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ITestCallOrder cap
      {
        get { return _s.DereferencePointer<ITestCallOrder>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct echoResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public echoResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public echoResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public echoResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public echoResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ITestCallOrder cap
      {
        get { return _s.DereferencePointer<ITestCallOrder>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct expectCancelParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public expectCancelParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public expectCancelParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public expectCancelParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public expectCancelParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ITestInterface cap
      {
        get { return _s.DereferencePointer<ITestInterface>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct methodWithDefaultsParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 2;
      private global::CapnpNet.Struct _s;
      public methodWithDefaultsParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public methodWithDefaultsParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public methodWithDefaultsParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public methodWithDefaultsParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public global::CapnpNet.Text a
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }

      public uint b
      {
        get { return _s.ReadUInt32(0, 123U); }
        set { _s.WriteUInt32(0, value, 123U); }
      }

      public global::CapnpNet.Text c
      {
        get { return _s.DereferenceText(1); }
        set { _s.WritePointer(1, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct methodWithDefaultsResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 2;
      private global::CapnpNet.Struct _s;
      public methodWithDefaultsResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public methodWithDefaultsResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public methodWithDefaultsResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public methodWithDefaultsResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public global::CapnpNet.Text d
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }

      public global::CapnpNet.Text e
      {
        get { return _s.DereferenceText(1); }
        set { _s.WritePointer(1, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct getHandleResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public getHandleResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public getHandleResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public getHandleResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public getHandleResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ITestHandle handle
      {
        get { return _s.DereferencePointer<ITestHandle>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct getNullResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public getNullResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public getNullResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public getNullResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public getNullResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public ITestMoreStuff nullCap
      {
        get { return _s.DereferencePointer<ITestMoreStuff>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct getEnormousStringResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public getEnormousStringResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public getEnormousStringResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public getEnormousStringResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public getEnormousStringResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public global::CapnpNet.Text str
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }
  }

  public interface ITestMembrane : global::CapnpNet.Rpc.ICapability
  {
    [global::CapnpNet.Ordinal(0)]
    TestMembrane.makeThingResults makeThing();
    [global::CapnpNet.Ordinal(1)]
    TestMembrane.Result callPassThrough(TestMembrane.callPassThroughParams parameters);
    [global::CapnpNet.Ordinal(2)]
    TestMembrane.Result callIntercept(TestMembrane.callInterceptParams parameters);
    [global::CapnpNet.Ordinal(3)]
    TestMembrane.loopbackResults loopback(TestMembrane.loopbackParams parameters);
  }

  public static class TestMembrane
  {
    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct makeThingResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public makeThingResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public makeThingResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public makeThingResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public makeThingResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public IThing thing
      {
        get { return _s.DereferencePointer<IThing>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct callPassThroughParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public callPassThroughParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public callPassThroughParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public callPassThroughParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public callPassThroughParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public IThing thing
      {
        get { return _s.DereferencePointer<IThing>(0); }
        set { _s.WritePointer(0, value); }
      }

      public bool tailCall
      {
        get { return _s.ReadBool(0); }
        set { _s.WriteBool(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct callInterceptParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public callInterceptParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public callInterceptParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public callInterceptParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public callInterceptParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public IThing thing
      {
        get { return _s.DereferencePointer<IThing>(0); }
        set { _s.WritePointer(0, value); }
      }

      public bool tailCall
      {
        get { return _s.ReadBool(0); }
        set { _s.WriteBool(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct loopbackParams : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public loopbackParams(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public loopbackParams(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public loopbackParams(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public loopbackParams(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public IThing thing
      {
        get { return _s.DereferencePointer<IThing>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct loopbackResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public loopbackResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public loopbackResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public loopbackResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public loopbackResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public IThing thing
      {
        get { return _s.DereferencePointer<IThing>(0); }
        set { _s.WritePointer(0, value); }
      }
    }

    public interface IThing : global::CapnpNet.Rpc.ICapability
    {
      [global::CapnpNet.Ordinal(0)]
      Result passThrough();
      [global::CapnpNet.Ordinal(1)]
      Result intercept();
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Result : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public Result(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Result(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Result(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Result(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public global::CapnpNet.Text text
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestContainMembrane : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 2;
    private global::CapnpNet.Struct _s;
    public TestContainMembrane(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestContainMembrane(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestContainMembrane(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestContainMembrane(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public TestMembrane.IThing cap
    {
      get { return _s.DereferencePointer<TestMembrane.IThing>(0); }
      set { _s.WritePointer(0, value); }
    }

    public global::CapnpNet.FlatArray<TestMembrane.IThing> list
    {
      get { return new global::CapnpNet.FlatArray<TestMembrane.IThing>(_s.DereferenceAbsPointer(1)); }
      set { _s.WritePointer(1, value.Pointer); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestTransferCap : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public TestTransferCap(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestTransferCap(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestTransferCap(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestTransferCap(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public global::CapnpNet.FlatArray<Element> list
    {
      get { return new global::CapnpNet.FlatArray<Element>(_s.DereferenceAbsPointer(0)); }
      set { _s.WritePointer(0, value.Pointer); }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Element : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 2;
      private global::CapnpNet.Struct _s;
      public Element(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Element(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Element(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Element(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public global::CapnpNet.Text text
      {
        get { return _s.DereferenceText(0); }
        set { _s.WritePointer(0, value); }
      }

      public ITestInterface cap
      {
        get { return _s.DereferencePointer<ITestInterface>(1); }
        set { _s.WritePointer(1, value); }
      }
    }
  }

  public interface ITestKeywordMethods : global::CapnpNet.Rpc.ICapability
  {
    [global::CapnpNet.Ordinal(0)]
    void delete();
    [global::CapnpNet.Ordinal(1)]
    void @class();
    [global::CapnpNet.Ordinal(2)]
    void @void();
    [global::CapnpNet.Ordinal(3)]
    void @return();
  }

  public interface ITestAuthenticatedBootstrap<VatId> : global::CapnpNet.Rpc.ICapability where VatId : global::CapnpNet.IAbsPointer
  {
    [global::CapnpNet.Ordinal(0)]
    TestAuthenticatedBootstrap<VatId>.getCallerIdResults getCallerId();
  }

  public static class TestAuthenticatedBootstrap<VatId>
    where VatId : global::CapnpNet.IAbsPointer
  {
    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct getCallerIdResults : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 0;
      public const int KNOWN_POINTER_WORDS = 1;
      private global::CapnpNet.Struct _s;
      public getCallerIdResults(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public getCallerIdResults(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public getCallerIdResults(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public getCallerIdResults(global::CapnpNet.Struct s) { _s = s; }
      global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
      {
        get { return _s; }
      }

      global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
      {
        get { return _s.Pointer; }
      }

      public VatId caller
      {
        get { return _s.DereferencePointer<VatId>(0); }
        set { _s.WritePointer(0, value); }
      }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestSturdyRef : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 2;
    private global::CapnpNet.Struct _s;
    public TestSturdyRef(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestSturdyRef(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestSturdyRef(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestSturdyRef(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public TestSturdyRefHostId hostId
    {
      get { return _s.DereferenceStruct<TestSturdyRefHostId>(0); }
      set { _s.WritePointer(0, value); }
    }

    public global::CapnpNet.AbsPointer objectId
    {
      get { return _s.DereferenceAbsPointer(1); }
      set { _s.WritePointer(1, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestSturdyRefHostId : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public TestSturdyRefHostId(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestSturdyRefHostId(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestSturdyRefHostId(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestSturdyRefHostId(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public global::CapnpNet.Text host
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestSturdyRefObjectId : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public TestSturdyRefObjectId(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestSturdyRefObjectId(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestSturdyRefObjectId(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestSturdyRefObjectId(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public Tag tag
    {
      get { return (Tag)_s.ReadUInt16(0); }
      set { _s.WriteUInt16(0, (ushort)value); }
    }

    public enum Tag : ushort
    {
      testInterface = 0,
      testExtends = 1,
      testPipeline = 2,
      testTailCallee = 3,
      testTailCaller = 4,
      testMoreStuff = 5
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestProvisionId : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public TestProvisionId(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestProvisionId(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestProvisionId(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestProvisionId(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestRecipientId : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public TestRecipientId(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestRecipientId(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestRecipientId(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestRecipientId(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestThirdPartyCapId : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public TestThirdPartyCapId(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestThirdPartyCapId(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestThirdPartyCapId(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestThirdPartyCapId(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct TestJoinResult : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public TestJoinResult(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public TestJoinResult(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public TestJoinResult(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public TestJoinResult(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }
  }
}