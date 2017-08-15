using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CapnpNet.Rpc
{
  public class RpcConnection : IDisposable
  {
    // question/answer/export/import tables
    private MessageStream _msgStream;
    private ArrayPoolSegmentFactory _segFactory;
    private CancellationTokenSource _ctSource;
    private AsyncCountdownEvent _inFlightCounter;
    private ConcurrentQueue<System.Exception> _exceptions;

    [StructLayout(LayoutKind.Auto)]
    private struct Question
    {
      public List<uint> paramExports;
      public bool isAwaitingReturn;
      public bool isTailCall;
    }
    [StructLayout(LayoutKind.Auto)]
    private struct Answer
    {
      public bool active;
      public Task<CapnpNet.Message> task;
      //public RpcCallContext callContext;
      public List<uint> resultExports;
    }
    [StructLayout(LayoutKind.Auto)]
    private struct Export
    {
      public uint refCount;
      public ICapability capability;
      public Task resolveOp;
    }
    [StructLayout(LayoutKind.Auto)]
    private struct Import
    {
      //public ImportClient importClient;
      //public RpcClient appClient;
      //public TaskCompletionSource<ClientHook> promiseFulfiller;
    }

    private ExportTable<Export> _exports;
    private ExportTable<Question> _questions;
    private ImportTable<Answer> _answers;
    private ImportTable<Import> _imports;
    private ICapability _bootstrapCap;
    private Dictionary<ICapability, uint> _capToExportId;

    public void Init(Stream readStream, Stream writeStream, ICapability bootstrapCapability)
    {
      _msgStream = new MessageStream();
      _segFactory = new ArrayPoolSegmentFactory();
      _msgStream.Init(readStream, writeStream, _segFactory);
      _ctSource = new CancellationTokenSource();
      _exceptions = new ConcurrentQueue<System.Exception>();
      _exports = new ExportTable<Export>();
      _bootstrapCap = bootstrapCapability;
      _capToExportId = new Dictionary<ICapability, uint>();
    }

    public async Task StartReadLoop()
    {
      _inFlightCounter = new AsyncCountdownEvent(1);

      var ct = _ctSource.Token;
      while (ct.IsCancellationRequested == false)
      {
        var msg = await _msgStream.ReceiveAsync();
        this.Process(msg);
      }

      _inFlightCounter.Signal();
      await _inFlightCounter.WaitAsync();

      if (_exceptions.Count > 0)
      {
        throw new AggregateException(_exceptions);
      }
    }

    private void Consume<T>(T state, Func<T, Task> task) => this.Consume(task(state));
    private void Consume(Func<Task> task) => this.Consume(task());

    private void Consume(Task task)
    {
      if (task.IsCompleted)
      {
        if (task.IsFaulted)
        {
          _exceptions.Enqueue(task.Exception);
          _ctSource.Cancel();
        }
      }
      else
      {
        _inFlightCounter.AddCount();
        task.ContinueWith(t =>
        {
          this.Consume(t);
          _inFlightCounter.Signal();
        }, TaskContinuationOptions.ExecuteSynchronously);
      }
    }

    internal void Process(CapnpNet.Message rpcMessage)
    {
      try
      {
        var message = rpcMessage.GetRoot<Message>();
        if (message.Is(out Message unimplemented))
        {
          // TODO: release resources
          throw new NotImplementedException();
        }
        else if (message.Is(out Exception abort))
        {
          throw new InvalidOperationException($"Connection aborted by remote: {abort.type.ToString()}: {abort.reason.ToString()}");
        }
        else if (message.Is(out Bootstrap bootstrap))
        {
          var reply = new CapnpNet.Message().Init(_segFactory);
          //await reply.PreAllocateAsync(senderMsg.CalculateSize() + 3);
          reply.Allocate(0, 1).WritePointer(0, new Return(reply)
          {
            answerId = bootstrap.questionId,
            which = Return.Union.results,
            results = new Payload
            {
              content = new OtherPointer
              {
                Type = PointerType.Other,
                OtherPointerType = OtherPointerType.Capability,
                CapabilityId = 0
              },
              capTable = new FlatArray<CapDescriptor>(reply, 1, out AllocationContext allocContext)
              {
                new CapDescriptor(ref allocContext)
                {
                  which = CapDescriptor.Union.senderHosted,
                  senderHosted = this.ExportCap(_bootstrapCap)
                }
              }
            }
          });
          rpcMessage.Dispose();
          rpcMessage = null;
          this.Consume((msgStream: _msgStream, reply: reply), async state =>
          {
            await state.msgStream.WriteAsync(state.reply);
            state.reply.Dispose();
          });
        }
        else if (message.Is(out Call call))
        {
          bool valid;
          ref Answer ans = ref _answers.TryGet(call.questionId, out valid);
          if (!valid)
          {
            // TODO: return exception message
          }

          if (call.target.which == MessageTarget.Union.importedCap)
          {
            ref Export export = ref _exports.TryGet(call.target.importedCap, out valid);
            if (!valid)
            {
              // TODO: return exception message
            }

            // TODO: read cap descriptors, look up corresponding imports, await any still pending

            if (call.@params.content.IsStruct(out var @struct) == false)
            {
              // TODO: return exception
            }

            switch (call.sendResultsTo.which)
            {
              case Call.sendResultsToGroup.Union.caller:
                // set up return message
                break;
              case Call.sendResultsToGroup.Union.yourself:
                // set aside memory to hold result
                break;
              case Call.sendResultsToGroup.Union.thirdParty:
              default:
                throw new NotImplementedException();
            }

            //ans.task = export.capability.DispatchCall(
            //  call.interfaceId,
            //  call.methodId,
            //  new CallContext(call, this),
            //  CancellationToken.None);

            var callTask = ans.task;
            this.Consume(async () =>
            {
              await callTask;
              if (call.sendResultsTo.which == Call.sendResultsToGroup.Union.caller)
              {
                
              }
            });
          }
          else if (call.target.Is(out PromisedAnswer promisedAnswer))
          {
            throw new NotImplementedException();
          }
          else
          {
            throw new NotSupportedException("Unknown call target");
          }
        }
        //else if (msg.Is(out Return ret))
        //{

        //}
        //else if (msg.Is(out Finish finish))
        //{

        //}
        else
        {
          // level -1 RPC
          this.Consume((message, this.CreateMessage()), async state =>
          {
            var (msg, reply) = state;
            var senderMsg = msg.GetStruct().Segment.Message;
            // TODO: copy data, or just move segments?
            
            await reply.PreAllocateAsync(senderMsg.CalculateSize() + 3);
            reply.Allocate(0, 1).WritePointer(0, new Message(reply)
            {
              which = Message.Union.unimplemented,
              unimplemented = msg.CopyTo(reply),
            });
            rpcMessage.Dispose();
            rpcMessage = null;
            await _msgStream.WriteAsync(reply);
            reply.Dispose();
          });
        }
      }
      finally
      {
        if (rpcMessage != null) rpcMessage.Dispose();
      }
    }

    internal FlatArray<CapDescriptor> CreateCapDescList(CapnpNet.Message msg)
    {
      var caps = msg.LocalCaps;
      var count = 0;
      for (uint i = 0; i < caps.Count; i++)
      {
        count += caps[i].RefCount > 0 ? 1 : 0;
      }

      var list = new FlatArray<CapDescriptor>(msg, count, out var allocContext);
      
      for (uint i = 0; i < caps.Count; i++)
      {
        var cap = caps[i];
        var desc = new CapDescriptor(ref allocContext);

        if (cap.RefCount > 0)
        {
          desc.which = CapDescriptor.Union.senderHosted;
          desc.senderHosted = this.ExportCap(cap.Capability);
        }
        else
        {
          desc.which = CapDescriptor.Union.none;
        };

        list.Add(desc);
      }

      return list;
    }

    public void SendAndDispose(uint answerId, CapnpNet.Message msg)
    {
      ref Answer answer = ref _answers.TryGet(answerId, out var valid);
      if (!valid)
      {
        // TODO: exception
      }
      
      async Task Run(CapnpNet.Message m)
      {
        await _msgStream.WriteAsync(m);
        m.Dispose();
      }

      this.Consume(Run(msg));
    }

    private uint ExportCap(ICapability cap)
    {
      if (_capToExportId.TryGetValue(cap, out uint id) == false)
      {
        ref Export export = ref _exports.Next(out id);
        export.capability = cap;
        export.refCount = 1;
        _capToExportId[cap] = id;
      }
      else
      {
        ref Export export = ref _exports.TryGet(id, out bool valid);
        Debug.Assert(valid);
        Debug.Assert(export.capability == cap);
        export.refCount++;
      }

      return id;
    }
    
    public void Dispose()
    {
      _msgStream.Dispose();
      _msgStream = null;
      _segFactory = null;
    }

    internal CapnpNet.Message CreateMessage() => new CapnpNet.Message().Init(_segFactory);
  }
  
  public struct CallContext
  {
    private readonly uint _questionId;
    private Call _call;
    private readonly RpcConnection _rpc;
    private readonly Call.sendResultsToGroup.Union _sendResultsTo;

    public CallContext(Call call, RpcConnection rpc)
    {
      _questionId = call.questionId;
      _sendResultsTo = call.sendResultsTo.which;
      _call = call;
      _rpc = rpc;
    }

    public Struct Parameters => _call.@params.content.IsStruct(out var @struct) ? @struct : throw new InvalidOperationException("Call parameters not a struct");
    
    public void DisposeParameters()
    {
      _call.GetStruct().Segment.Message.Dispose();
      _call = default(Call);
    }
    
    public CapnpNet.Message CreateReply(int? sizeHint = null)
    {
      if (_sendResultsTo == Call.sendResultsToGroup.Union.caller)
      {
        var msg = _rpc.CreateMessage();

        msg.PreAllocate(3 + sizeHint ?? 5);

        msg.Allocate(0, 1).WritePointer(0, new Message(msg)
        {
          @return = new Return(msg)
          {
            answerId = _questionId
          }
        });

        return msg;
      }
      else
      {
        // TODO: allocate from scratch memory?
        throw new NotImplementedException();
      }
    }
    
    public async ValueTask<CapnpNet.Message> CreateReplyAsync(int? sizeHint = null)
    {
      var msg = _rpc.CreateMessage();
      
      await msg.PreAllocateAsync(3 + sizeHint ?? 5); // TODO: tune

      msg.Allocate(0, 1).WritePointer(0, new Message(msg)
      {
        @return = new Return(msg)
        {
          answerId = _questionId
        }
      });

      return msg;
    }

    public Task TailCall(Pointer requestPayload)
    {
      throw new NotImplementedException();
    }
  }

  public struct ReplyContext
  {
    private readonly RpcConnection _rpc;
    private readonly uint _answerId;

    public ReplyContext(RpcConnection rpc, uint answerId, CapnpNet.Message msg)
    {
      _rpc = rpc;
      _answerId = answerId;
      this.Message = msg;
    }

    public CapnpNet.Message Message { get; }

    public Payload PrepareReturn()
    {
      var ret = this.Message.GetRoot<Message>().@return;
      ret.which = Rpc.Return.Union.results;
      return (ret.results = new Payload(this.Message));
    }

    public void Return(AbsPointer returnPayload)
    {
      var ret = this.Message.GetRoot<Message>().@return;
      ret.which = Rpc.Return.Union.results;
      ret.results = new Payload(this.Message)
      {
        content = returnPayload,
        capTable = _rpc.CreateCapDescList(this.Message)
      };

      this.Send();
    }

    public void Throw(string reason)
    {
      var ret = this.Message.GetRoot<Message>().@return;
      ret.which = Rpc.Return.Union.exception;
      ret.exception = new Exception(this.Message)
      {
        type = Exception.Type.failed,
        reason = new Text(this.Message, reason)
      };

      this.Send();
    }

    public void Send() => _rpc.SendAndDispose(_answerId, this.Message);
  }
}
