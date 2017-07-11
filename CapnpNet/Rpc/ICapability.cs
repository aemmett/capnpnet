﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CapnpNet.Rpc
{
  public interface ICapability
  {
    Task DispatchCall(ulong interfaceId, ushort methodId, CallContext callContext);
  }

  public sealed class RpcTask<T>
  {
    private RpcTask<object> _previous;

    public Awaiter GetAwaiter() => new Awaiter();

    public struct Awaiter : INotifyCompletion
    {
      public Awaiter GetAwaiter() => this;

      public bool IsCompleted { get; private set; }

      public T GetResult()
      {
        return default(T);
      }

      public void OnCompleted(Action continuation)
      {
        throw new NotImplementedException();
      }
    }
  }
}
