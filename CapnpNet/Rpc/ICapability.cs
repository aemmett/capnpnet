using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpNet.Rpc
{
  using Msg = CapnpNet.Message;

  public sealed class InterfaceIdAttribute : Attribute
  {
    public uint InterfaceId { get; set; }
  }

  public interface ICapability : IAbsPointer
  {
  }
  
  public sealed class RemoteProxy<TCap> : Proxy<TCap>
    where TCap : ICapability
  {
    private readonly RpcConnection _con;
    private readonly uint _id;
    private readonly bool _isPromise; // if true, _id is questionId

    // TODO: conversion to TCap so we can forward capabilities by proxy (and three-party handoff eventually)
    // TODO: make awaitable. On await, flush pipelined calls
    
    public override CapnpNet.Message CreateMessage() // TODO: purpose?
    {
      // consider: reservation mechanism? (allow call metadata to be put at front of message)
      // also consider 
      return _con.CreateMessage();
    }

    public override uint DispatchCall(ushort methodId, Msg request)
    {
      var parameters = request.Root;
      ref RpcConnection.Question question = ref _con.AllocateQuestion(out uint questionId);
      MessageTarget target;
      request.SetRoot(new Call(request)
      {
        // TODO: cache
        interfaceId = typeof(TCap).GetTypeInfo().GetCustomAttribute<InterfaceIdAttribute>().InterfaceId,
        methodId = methodId,
        target = (target = new MessageTarget(request)),
        questionId = questionId,
        @params = new Payload(request)
        {
          content = parameters.Pointer,
          capTable = _con.CreateCapDescList(request)
        }
      });

      if (_isPromise)
      {
        target.which = MessageTarget.Union.importedCap;
        target.promisedAnswer = new PromisedAnswer(request)
        {
          questionId = _id
        };
      }
      else
      { 
        target.which = MessageTarget.Union.importedCap;
        target.importedCap = _id;
      }

      _con.SendAndDispose(request);
      return questionId;
    }
  }

  // consider: refine this as a generic (non-Cap'n Proto-specific) abstraction?
  public abstract class Proxy<TCap> where TCap : ICapability
  {
    public abstract CapnpNet.Message CreateMessage();

    public abstract uint DispatchCall(ushort methodId, Msg request);
  }

  public interface IStrawmanService : ICapability
  {
  }

  public static class StrawmanService
  {
    public struct addParams : IStruct
    {
      private Struct _s;
      
      public addParams(Msg m)
        : this(m.Allocate(1, 0))
      {
      }

      public addParams(Struct s)
      {
        _s = s;
      }

      Struct IStruct.Struct => _s;

      AbsPointer IAbsPointer.Pointer => _s.Pointer;

      public int a
      {
        get => _s.ReadInt32(0);
        set => _s.WriteInt32(0, value);
      }
      public int b
      {
        get => _s.ReadInt32(1);
        set => _s.WriteInt32(1, value);
      }
    }

    public static RpcTask<int> Add(this Proxy<IStrawmanService> cap, int a, int b)
    {
      var msg = cap.CreateMessage();
      msg.SetRoot(new addParams(msg)
      {
        a = 1,
        b = 2
      });
      var questionId = cap.DispatchCall(0x123, msg);
      return new RpcTask<int>(); // TODO
    }
  }

  /*
  public abstract class RpcBase : ICapability
  {
    public Task DispatchCall(ulong interfaceId, ushort methodId, CallContext callContext, CancellationToken ct)
    {
      throw new NotImplementedException();
    }
  }*/
  
  // TODO: result-less version
  public struct RpcTask<T>
  {
    private readonly RpcConnection _con;
    private readonly uint _questionId;

    public RpcTask(RpcConnection con, uint questionId)
    {
      _con = con;
      _questionId = questionId;
    }

    public Awaiter GetAwaiter() => new Awaiter(this);

    public struct Awaiter : INotifyCompletion
    {
      private RpcTask<T> _rpcTask;

      public Awaiter(RpcTask<T> rpcTask) : this()
      {
        _rpcTask = rpcTask;
      }

      public Awaiter GetAwaiter() => this; // ?

      public bool IsCompleted { get; private set; }

      public T GetResult()
      {
        // TODO: mark question as ready to release?
        // - it's not clear if primitive values can be pipelined?
        // - if they can, 
        return default(T);
      }

      public void OnCompleted(Action continuation)
      {
        throw new NotImplementedException();
      }
    }
  }
}
