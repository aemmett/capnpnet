using System;

namespace CapnpNet.Rpc
{
  internal sealed class ImportTable<T>
  {
    private struct Entry
    {
      public uint key;
      public T value;
      public bool active;
    }
    
    private Entry[] _entries;
    
    private T _empty;
    private uint _count;
    
    public ref T TryGet(uint key, out bool valid)
    {
      for (int i = 0; i < _entries.Length; i++)
      {
        ref var entry = ref _entries[i];
        if (entry.key == key && entry.active)
        {
          valid = true;
          return ref entry.value;
        }
      }

      valid = false;
      return ref _empty;
    }

    private ref T GetOrAdd(uint key)
    {
      int? emptyIndex = null;
      for (int i = 0; i < _entries.Length; i++)
      {
        ref var entry = ref _entries[i];
        if (entry.key == key && entry.active)
        {
          return ref entry.value;
        }
        else if (entry.active == false && emptyIndex == null)
        {
          emptyIndex = i;
        }
      }

      if (emptyIndex == null)
      {
        emptyIndex = _entries.Length;
        Array.Resize(ref _entries, _entries.Length * 2);
      }

      ref var newEntry = ref _entries[emptyIndex.Value];
      newEntry.key = key;
      newEntry.active = true;
      _count++;
      return ref newEntry.value;
    }

    public T Remove(uint key)
    {
      for (int i = 0; i < _entries.Length; i++)
      {
        ref var entry = ref _entries[i];
        if (entry.key == key && entry.active)
        {
          var copy = entry.value;
          entry.key = 0;
          entry.value = default;
          entry.active = false;
          _count--;
          return copy;
        }
      }

      throw new InvalidOperationException($"Entry with key {key} not found.");
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
