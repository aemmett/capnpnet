using System.Threading.Tasks;

namespace CapnpNet.Rpc
{
  public interface ICapability
  {
    Task DispatchCall(ulong interfaceId, ushort methodId, CallContext callContext);
  }
}
