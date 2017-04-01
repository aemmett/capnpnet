using CapnpNet;

namespace CapnpNet.Rpc.TwoParty
{
  public struct JoinResult : IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public JoinResult(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public JoinResult(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public JoinResult(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct IStruct.Struct
    {
      get { return _s; }
      set { _s = value; }
    }

    public uint joinId
    {
      get { return _s.ReadUInt32(0); }
      set { _s.WriteUInt32(0, value); }
    }

    public bool succeeded
    {
      get { return _s.ReadBool(32); }
      set { _s.WriteBool(32, value); }
    }

    public Pointer cap
    {
      get { return _s.ReadRawPointer(0); }
      set { _s.WriteRawPointer(0, value); }
    }
  }

  public struct RecipientId : IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public RecipientId(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public RecipientId(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public RecipientId(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct IStruct.Struct
    {
      get { return _s; }
      set { _s = value; }
    }
  }

  public struct JoinKeyPart : IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public JoinKeyPart(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public JoinKeyPart(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public JoinKeyPart(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct IStruct.Struct
    {
      get { return _s; }
      set { _s = value; }
    }

    public uint joinId
    {
      get { return _s.ReadUInt32(0); }
      set { _s.WriteUInt32(0, value); }
    }

    public ushort partCount
    {
      get { return _s.ReadUInt16(2); }
      set { _s.WriteUInt16(2, value); }
    }

    public ushort partNum
    {
      get { return _s.ReadUInt16(3); }
      set { _s.WriteUInt16(3, value); }
    }
  }

  public struct VatId : IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public VatId(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public VatId(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public VatId(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct IStruct.Struct
    {
      get { return _s; }
      set { _s = value; }
    }

    public Side side
    {
      get { return (Side)_s.ReadUInt16(0); }
      set { _s.WriteUInt16(0, (ushort)value); }
    }
  }

  public enum Side : ushort
  {
    server = 0,
    client = 1
  }

  public struct ProvisionId : IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public ProvisionId(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public ProvisionId(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public ProvisionId(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct IStruct.Struct
    {
      get { return _s; }
      set { _s = value; }
    }

    public uint joinId
    {
      get { return _s.ReadUInt32(0); }
      set { _s.WriteUInt32(0, value); }
    }
  }

  public struct ThirdPartyCapId : IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public ThirdPartyCapId(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public ThirdPartyCapId(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public ThirdPartyCapId(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct IStruct.Struct
    {
      get { return _s; }
      set { _s = value; }
    }
  }
}