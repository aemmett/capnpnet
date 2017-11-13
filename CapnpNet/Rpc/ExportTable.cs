using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CapnpNet.Rpc
{
  // TODO: threading concerns (currently assume RpcConnection is 
  internal sealed class ExportTable<T>
  {
    private static T _empty;

    private T[] _slots = new T[16]; // TODO: pool

    // TODO: find a priority queue implementation
    //private SortedList<uint, uint> _freeList;
    private List<uint> _freeList;

    private uint _nextId;
    
    public ref T TryGet(uint key, out bool valid)
    {
      valid = key < _nextId; // && _freeList?.ContainsKey(key) == false;
      if (!valid) return ref _empty;

      return ref _slots[key];
    }

    private ref T GetOrAdd(uint key)
    {
      if (key < _slots.Length) return ref _slots[key];

      Array.Resize(ref _slots, _slots.Length * 2);
      return ref _slots[key];
    }

    public T Remove(uint key, ref T entry)
    {
      ref T val = ref this.TryGet(key, out bool valid);
      if (!valid || !Unsafe.AreSame(ref val, ref entry))
      {
        throw new InvalidOperationException($"Entry not present at key ${key}");
      }

      T copy = val;
      val = default;
      //if (_freeList == null) _freeList = new SortedList<uint, uint>();
      if (_freeList == null) _freeList = new List<uint>();

      _freeList.Add(key);
      return copy;
    }

    public ref T Next(out uint id)
    {
      if (_freeList?.Count > 0)
      {
        id = _freeList[0];
        _freeList.RemoveAt(0);
      }
      else
      {
        id = _nextId;
        _nextId++;
      }

      return ref this.GetOrAdd(id);
    }
  }

  //public struct ReplyContext
  //{
  //  private readonly RpcConnection _rpc;
  //  private readonly uint _answerId;

  //  public ReplyContext(RpcConnection rpc, uint answerId, CapnpNet.Message msg)
  //  {
  //    _rpc = rpc;
  //    _answerId = answerId;
  //    this.Message = msg;
  //  }

  //  public CapnpNet.Message Message { get; }
    
  //  public Payload PrepareReturn()
  //  {
  //    var ret = this.Message.GetRoot<Message>().@return;
  //    ret.which = Rpc.Return.Union.results;
  //    return (ret.results = new Payload(this.Message));
  //  }

  //  public void Return(AbsPointer returnPayload)
  //  {
  //    var ret = this.Message.GetRoot<Message>().@return;
  //    ret.which = Rpc.Return.Union.results;
  //    ret.results = new Payload(this.Message)
  //    {
  //      content = returnPayload,
  //      capTable = _rpc.CreateCapDescList(this.Message)
  //    };

  //    this.Send();
  //  }

  //  public void Throw(string reason)
  //  {
  //    var ret = this.Message.GetRoot<Message>().@return;
  //    ret.which = Rpc.Return.Union.exception;
  //    ret.exception = new Exception(this.Message)
  //    {
  //      type = Exception.Type.failed,
  //      reason = new Text(this.Message, reason)
  //    };

  //    this.Send();
  //  }

  //  public void Send() => _rpc.SendAndDispose(_answerId, this.Message);
  //}
}
