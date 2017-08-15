namespace CapnpNet.Rpc
{
  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct Message : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public Message(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public Message(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public Message(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public Message(global::CapnpNet.Struct s) { _s = s; }
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
      unimplemented = 0,
      abort = 1,
      bootstrap = 8,
      call = 2,
      @return = 3,
      finish = 4,
      resolve = 5,
      release = 6,
      disembargo = 13,
      obsoleteSave = 7,
      obsoleteDelete = 9,
      provide = 10,
      accept = 11,
      join = 12,
    }

    public bool Is(out Message unimplemented)
    {
      var ret = this.which == Union.unimplemented;
      unimplemented = ret ? this.unimplemented : default (Message);
      return ret;
    }

    public bool Is(out Exception abort)
    {
      var ret = this.which == Union.abort;
      abort = ret ? this.abort : default (Exception);
      return ret;
    }

    public bool Is(out Bootstrap bootstrap)
    {
      var ret = this.which == Union.bootstrap;
      bootstrap = ret ? this.bootstrap : default (Bootstrap);
      return ret;
    }

    public bool Is(out Call call)
    {
      var ret = this.which == Union.call;
      call = ret ? this.call : default (Call);
      return ret;
    }

    public bool Is(out Return @return)
    {
      var ret = this.which == Union.@return;
      @return = ret ? this.@return : default (Return);
      return ret;
    }

    public bool Is(out Finish finish)
    {
      var ret = this.which == Union.finish;
      finish = ret ? this.finish : default (Finish);
      return ret;
    }

    public bool Is(out Resolve resolve)
    {
      var ret = this.which == Union.resolve;
      resolve = ret ? this.resolve : default (Resolve);
      return ret;
    }

    public bool Is(out Release release)
    {
      var ret = this.which == Union.release;
      release = ret ? this.release : default (Release);
      return ret;
    }

    public bool Is(out Disembargo disembargo)
    {
      var ret = this.which == Union.disembargo;
      disembargo = ret ? this.disembargo : default (Disembargo);
      return ret;
    }

    public bool Is(out Provide provide)
    {
      var ret = this.which == Union.provide;
      provide = ret ? this.provide : default (Provide);
      return ret;
    }

    public bool Is(out Accept accept)
    {
      var ret = this.which == Union.accept;
      accept = ret ? this.accept : default (Accept);
      return ret;
    }

    public bool Is(out Join join)
    {
      var ret = this.which == Union.join;
      join = ret ? this.join : default (Join);
      return ret;
    }

    public Union which
    {
      get { return (Union)_s.ReadUInt16(0); }
      set { _s.WriteUInt16(0, (ushort)value); }
    }

    public Message unimplemented
    {
      get { return _s.DereferenceStruct<Message>(0); }
      set { _s.WritePointer(0, value); }
    }

    public Exception abort
    {
      get { return _s.DereferenceStruct<Exception>(0); }
      set { _s.WritePointer(0, value); }
    }

    public Bootstrap bootstrap
    {
      get { return _s.DereferenceStruct<Bootstrap>(0); }
      set { _s.WritePointer(0, value); }
    }

    public Call call
    {
      get { return _s.DereferenceStruct<Call>(0); }
      set { _s.WritePointer(0, value); }
    }

    public Return @return
    {
      get { return _s.DereferenceStruct<Return>(0); }
      set { _s.WritePointer(0, value); }
    }

    public Finish finish
    {
      get { return _s.DereferenceStruct<Finish>(0); }
      set { _s.WritePointer(0, value); }
    }

    public Resolve resolve
    {
      get { return _s.DereferenceStruct<Resolve>(0); }
      set { _s.WritePointer(0, value); }
    }

    public Release release
    {
      get { return _s.DereferenceStruct<Release>(0); }
      set { _s.WritePointer(0, value); }
    }

    public Disembargo disembargo
    {
      get { return _s.DereferenceStruct<Disembargo>(0); }
      set { _s.WritePointer(0, value); }
    }

    public global::CapnpNet.AbsPointer obsoleteSave
    {
      get { return _s.DereferenceAbsPointer(0); }
      set { _s.WritePointer(0, value); }
    }

    public global::CapnpNet.AbsPointer obsoleteDelete
    {
      get { return _s.DereferenceAbsPointer(0); }
      set { _s.WritePointer(0, value); }
    }

    public Provide provide
    {
      get { return _s.DereferenceStruct<Provide>(0); }
      set { _s.WritePointer(0, value); }
    }

    public Accept accept
    {
      get { return _s.DereferenceStruct<Accept>(0); }
      set { _s.WritePointer(0, value); }
    }

    public Join join
    {
      get { return _s.DereferenceStruct<Join>(0); }
      set { _s.WritePointer(0, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct Bootstrap : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public Bootstrap(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public Bootstrap(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public Bootstrap(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public Bootstrap(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public uint questionId
    {
      get { return _s.ReadUInt32(0); }
      set { _s.WriteUInt32(0, value); }
    }

    public global::CapnpNet.AbsPointer deprecatedObjectId
    {
      get { return _s.DereferenceAbsPointer(0); }
      set { _s.WritePointer(0, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct Call : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 3;
    public const int KNOWN_POINTER_WORDS = 3;
    private global::CapnpNet.Struct _s;
    public Call(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public Call(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public Call(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public Call(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public uint questionId
    {
      get { return _s.ReadUInt32(0); }
      set { _s.WriteUInt32(0, value); }
    }

    public MessageTarget target
    {
      get { return _s.DereferenceStruct<MessageTarget>(0); }
      set { _s.WritePointer(0, value); }
    }

    public ulong interfaceId
    {
      get { return _s.ReadUInt64(1); }
      set { _s.WriteUInt64(1, value); }
    }

    public ushort methodId
    {
      get { return _s.ReadUInt16(2); }
      set { _s.WriteUInt16(2, value); }
    }

    public bool allowThirdPartyTailCall
    {
      get { return _s.ReadBool(128); }
      set { _s.WriteBool(128, value); }
    }

    public Payload @params
    {
      get { return _s.DereferenceStruct<Payload>(1); }
      set { _s.WritePointer(1, value); }
    }

    public sendResultsToGroup sendResultsTo => new sendResultsToGroup(_s);
    public struct sendResultsToGroup
    {
      private readonly global::CapnpNet.Struct _s;
      public sendResultsToGroup(global::CapnpNet.Struct s) { _s = s; }
      public enum Union : ushort
      {
        caller = 0,
        yourself = 1,
        thirdParty = 2,
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(3); }
        set { _s.WriteUInt16(3, (ushort)value); }
      }

      public global::CapnpNet.AbsPointer thirdParty
      {
        get { return _s.DereferenceAbsPointer(2); }
        set { _s.WritePointer(2, value); }
      }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct Return : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 2;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public Return(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public Return(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public Return(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public Return(global::CapnpNet.Struct s) { _s = s; }
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
      results = 0,
      exception = 1,
      canceled = 2,
      resultsSentElsewhere = 3,
      takeFromOtherQuestion = 4,
      acceptFromThirdParty = 5,
    }

    public bool Is(out Payload results)
    {
      var ret = this.which == Union.results;
      results = ret ? this.results : default (Payload);
      return ret;
    }

    public bool Is(out Exception exception)
    {
      var ret = this.which == Union.exception;
      exception = ret ? this.exception : default (Exception);
      return ret;
    }

    public Union which
    {
      get { return (Union)_s.ReadUInt16(3); }
      set { _s.WriteUInt16(3, (ushort)value); }
    }

    public uint answerId
    {
      get { return _s.ReadUInt32(0); }
      set { _s.WriteUInt32(0, value); }
    }

    public bool releaseParamCaps
    {
      get { return _s.ReadBool(32, true); }
      set { _s.WriteBool(32, value, true); }
    }

    public Payload results
    {
      get { return _s.DereferenceStruct<Payload>(0); }
      set { _s.WritePointer(0, value); }
    }

    public Exception exception
    {
      get { return _s.DereferenceStruct<Exception>(0); }
      set { _s.WritePointer(0, value); }
    }

    public uint takeFromOtherQuestion
    {
      get { return _s.ReadUInt32(2); }
      set { _s.WriteUInt32(2, value); }
    }

    public global::CapnpNet.AbsPointer acceptFromThirdParty
    {
      get { return _s.DereferenceAbsPointer(0); }
      set { _s.WritePointer(0, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct Finish : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public Finish(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public Finish(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public Finish(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public Finish(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public uint questionId
    {
      get { return _s.ReadUInt32(0); }
      set { _s.WriteUInt32(0, value); }
    }

    public bool releaseResultCaps
    {
      get { return _s.ReadBool(32, true); }
      set { _s.WriteBool(32, value, true); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct Resolve : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public Resolve(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public Resolve(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public Resolve(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public Resolve(global::CapnpNet.Struct s) { _s = s; }
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
      cap = 0,
      exception = 1,
    }

    public bool Is(out CapDescriptor cap)
    {
      var ret = this.which == Union.cap;
      cap = ret ? this.cap : default (CapDescriptor);
      return ret;
    }

    public bool Is(out Exception exception)
    {
      var ret = this.which == Union.exception;
      exception = ret ? this.exception : default (Exception);
      return ret;
    }

    public Union which
    {
      get { return (Union)_s.ReadUInt16(2); }
      set { _s.WriteUInt16(2, (ushort)value); }
    }

    public uint promiseId
    {
      get { return _s.ReadUInt32(0); }
      set { _s.WriteUInt32(0, value); }
    }

    public CapDescriptor cap
    {
      get { return _s.DereferenceStruct<CapDescriptor>(0); }
      set { _s.WritePointer(0, value); }
    }

    public Exception exception
    {
      get { return _s.DereferenceStruct<Exception>(0); }
      set { _s.WritePointer(0, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct Release : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 0;
    private global::CapnpNet.Struct _s;
    public Release(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public Release(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public Release(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public Release(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public uint id
    {
      get { return _s.ReadUInt32(0); }
      set { _s.WriteUInt32(0, value); }
    }

    public uint referenceCount
    {
      get { return _s.ReadUInt32(1); }
      set { _s.WriteUInt32(1, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct Disembargo : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public Disembargo(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public Disembargo(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public Disembargo(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public Disembargo(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public MessageTarget target
    {
      get { return _s.DereferenceStruct<MessageTarget>(0); }
      set { _s.WritePointer(0, value); }
    }

    public contextGroup context => new contextGroup(_s);
    public struct contextGroup
    {
      private readonly global::CapnpNet.Struct _s;
      public contextGroup(global::CapnpNet.Struct s) { _s = s; }
      public enum Union : ushort
      {
        senderLoopback = 0,
        receiverLoopback = 1,
        accept = 2,
        provide = 3,
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(2); }
        set { _s.WriteUInt16(2, (ushort)value); }
      }

      public uint senderLoopback
      {
        get { return _s.ReadUInt32(0); }
        set { _s.WriteUInt32(0, value); }
      }

      public uint receiverLoopback
      {
        get { return _s.ReadUInt32(0); }
        set { _s.WriteUInt32(0, value); }
      }

      public uint provide
      {
        get { return _s.ReadUInt32(0); }
        set { _s.WriteUInt32(0, value); }
      }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct Provide : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 2;
    private global::CapnpNet.Struct _s;
    public Provide(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public Provide(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public Provide(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public Provide(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public uint questionId
    {
      get { return _s.ReadUInt32(0); }
      set { _s.WriteUInt32(0, value); }
    }

    public MessageTarget target
    {
      get { return _s.DereferenceStruct<MessageTarget>(0); }
      set { _s.WritePointer(0, value); }
    }

    public global::CapnpNet.AbsPointer recipient
    {
      get { return _s.DereferenceAbsPointer(1); }
      set { _s.WritePointer(1, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct Accept : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public Accept(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public Accept(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public Accept(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public Accept(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public uint questionId
    {
      get { return _s.ReadUInt32(0); }
      set { _s.WriteUInt32(0, value); }
    }

    public global::CapnpNet.AbsPointer provision
    {
      get { return _s.DereferenceAbsPointer(0); }
      set { _s.WritePointer(0, value); }
    }

    public bool embargo
    {
      get { return _s.ReadBool(32); }
      set { _s.WriteBool(32, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct Join : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 2;
    private global::CapnpNet.Struct _s;
    public Join(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public Join(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public Join(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public Join(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public uint questionId
    {
      get { return _s.ReadUInt32(0); }
      set { _s.WriteUInt32(0, value); }
    }

    public MessageTarget target
    {
      get { return _s.DereferenceStruct<MessageTarget>(0); }
      set { _s.WritePointer(0, value); }
    }

    public global::CapnpNet.AbsPointer keyPart
    {
      get { return _s.DereferenceAbsPointer(1); }
      set { _s.WritePointer(1, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct MessageTarget : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public MessageTarget(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public MessageTarget(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public MessageTarget(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public MessageTarget(global::CapnpNet.Struct s) { _s = s; }
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
      importedCap = 0,
      promisedAnswer = 1,
    }

    public bool Is(out PromisedAnswer promisedAnswer)
    {
      var ret = this.which == Union.promisedAnswer;
      promisedAnswer = ret ? this.promisedAnswer : default (PromisedAnswer);
      return ret;
    }

    public Union which
    {
      get { return (Union)_s.ReadUInt16(2); }
      set { _s.WriteUInt16(2, (ushort)value); }
    }

    public uint importedCap
    {
      get { return _s.ReadUInt32(0); }
      set { _s.WriteUInt32(0, value); }
    }

    public PromisedAnswer promisedAnswer
    {
      get { return _s.DereferenceStruct<PromisedAnswer>(0); }
      set { _s.WritePointer(0, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct Payload : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 0;
    public const int KNOWN_POINTER_WORDS = 2;
    private global::CapnpNet.Struct _s;
    public Payload(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public Payload(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public Payload(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public Payload(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public global::CapnpNet.AbsPointer content
    {
      get { return _s.DereferenceAbsPointer(0); }
      set { _s.WritePointer(0, value); }
    }

    public global::CapnpNet.FlatArray<CapDescriptor> capTable
    {
      get { return new global::CapnpNet.FlatArray<CapDescriptor>(_s.DereferenceAbsPointer(1)); }
      set { _s.WritePointer(1, value.Pointer); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct CapDescriptor : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public CapDescriptor(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public CapDescriptor(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public CapDescriptor(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public CapDescriptor(global::CapnpNet.Struct s) { _s = s; }
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
      none = 0,
      senderHosted = 1,
      senderPromise = 2,
      receiverHosted = 3,
      receiverAnswer = 4,
      thirdPartyHosted = 5,
    }

    public bool Is(out PromisedAnswer receiverAnswer)
    {
      var ret = this.which == Union.receiverAnswer;
      receiverAnswer = ret ? this.receiverAnswer : default (PromisedAnswer);
      return ret;
    }

    public bool Is(out ThirdPartyCapDescriptor thirdPartyHosted)
    {
      var ret = this.which == Union.thirdPartyHosted;
      thirdPartyHosted = ret ? this.thirdPartyHosted : default (ThirdPartyCapDescriptor);
      return ret;
    }

    public Union which
    {
      get { return (Union)_s.ReadUInt16(0); }
      set { _s.WriteUInt16(0, (ushort)value); }
    }

    public uint senderHosted
    {
      get { return _s.ReadUInt32(1); }
      set { _s.WriteUInt32(1, value); }
    }

    public uint senderPromise
    {
      get { return _s.ReadUInt32(1); }
      set { _s.WriteUInt32(1, value); }
    }

    public uint receiverHosted
    {
      get { return _s.ReadUInt32(1); }
      set { _s.WriteUInt32(1, value); }
    }

    public PromisedAnswer receiverAnswer
    {
      get { return _s.DereferenceStruct<PromisedAnswer>(0); }
      set { _s.WritePointer(0, value); }
    }

    public ThirdPartyCapDescriptor thirdPartyHosted
    {
      get { return _s.DereferenceStruct<ThirdPartyCapDescriptor>(0); }
      set { _s.WritePointer(0, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct PromisedAnswer : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public PromisedAnswer(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public PromisedAnswer(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public PromisedAnswer(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public PromisedAnswer(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public uint questionId
    {
      get { return _s.ReadUInt32(0); }
      set { _s.WriteUInt32(0, value); }
    }

    public global::CapnpNet.FlatArray<Op> transform
    {
      get { return new global::CapnpNet.FlatArray<Op>(_s.DereferenceAbsPointer(0)); }
      set { _s.WritePointer(0, value.Pointer); }
    }

    [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
    public struct Op : global::CapnpNet.IStruct
    {
      public const int KNOWN_DATA_WORDS = 1;
      public const int KNOWN_POINTER_WORDS = 0;
      private global::CapnpNet.Struct _s;
      public Op(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
      {
      }

      public Op(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
      {
      }

      public Op(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
      {
      }

      public Op(global::CapnpNet.Struct s) { _s = s; }
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
        noop = 0,
        getPointerField = 1,
      }

      public Union which
      {
        get { return (Union)_s.ReadUInt16(0); }
        set { _s.WriteUInt16(0, (ushort)value); }
      }

      public ushort getPointerField
      {
        get { return _s.ReadUInt16(1); }
        set { _s.WriteUInt16(1, value); }
      }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct ThirdPartyCapDescriptor : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public ThirdPartyCapDescriptor(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public ThirdPartyCapDescriptor(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public ThirdPartyCapDescriptor(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public ThirdPartyCapDescriptor(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public global::CapnpNet.AbsPointer id
    {
      get { return _s.DereferenceAbsPointer(0); }
      set { _s.WritePointer(0, value); }
    }

    public uint vineId
    {
      get { return _s.ReadUInt32(0); }
      set { _s.WriteUInt32(0, value); }
    }
  }

  [global::CapnpNet.PreferredListEncoding(global::CapnpNet.ElementSize.Composite)]
  public struct Exception : global::CapnpNet.IStruct
  {
    public const int KNOWN_DATA_WORDS = 1;
    public const int KNOWN_POINTER_WORDS = 1;
    private global::CapnpNet.Struct _s;
    public Exception(ref global::CapnpNet.AllocationContext allocContext) : this (allocContext.Allocate(KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS))
    {
    }

    public Exception(global::CapnpNet.Message m) : this (m, KNOWN_DATA_WORDS, KNOWN_POINTER_WORDS)
    {
    }

    public Exception(global::CapnpNet.Message m, ushort dataWords, ushort pointers) : this (m.Allocate(dataWords, pointers))
    {
    }

    public Exception(global::CapnpNet.Struct s) { _s = s; }
    global::CapnpNet.Struct global::CapnpNet.IStruct.Struct
    {
      get { return _s; }
    }

    global::CapnpNet.AbsPointer global::CapnpNet.IAbsPointer.Pointer
    {
      get { return _s.Pointer; }
    }

    public global::CapnpNet.Text reason
    {
      get { return _s.DereferenceText(0); }
      set { _s.WritePointer(0, value); }
    }

    public Type type
    {
      get { return (Type)_s.ReadUInt16(2); }
      set { _s.WriteUInt16(2, (ushort)value); }
    }

    public bool obsoleteIsCallersFault
    {
      get { return _s.ReadBool(0); }
      set { _s.WriteBool(0, value); }
    }

    public ushort obsoleteDurability
    {
      get { return _s.ReadUInt16(1); }
      set { _s.WriteUInt16(1, value); }
    }

    public enum Type : ushort
    {
      failed = 0,
      overloaded = 1,
      disconnected = 2,
      unimplemented = 3
    }
  }
}