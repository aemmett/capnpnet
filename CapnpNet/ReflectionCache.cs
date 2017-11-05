using System;
using System.Linq;
using System.Reflection;
using CapnpNet.Rpc;

namespace CapnpNet
{
  internal class ReflectionCache<T>
  {
    private static ElementSize? preferredElementSize;

    public static readonly bool ImplementsIStruct;
    public static readonly ushort KnownDataWords;
    public static readonly ushort KnownPointerWords;
    public static readonly bool ImplementsIPureAbsPointer;
    public static readonly bool ImplementsIAbsPointer;
    public static readonly bool ImplementsICapability;

    public static ElementSize PreferredElementSize
    {
      get
      {
        if (preferredElementSize.HasValue == false)
        {
          preferredElementSize = typeof(T)
            .GetTypeInfo()
            .GetCustomAttribute<PreferredListEncodingAttribute>()
            ?.ElementSize;
        }

        return preferredElementSize ?? throw new InvalidOperationException($"Type {typeof(T).Name} does not have PreferredListEncodingAttribute");
      }
    }

    static ReflectionCache()
    {
      var interfaces = typeof(T).GetTypeInfo().ImplementedInterfaces.ToList();
      ImplementsIStruct = interfaces.Contains(typeof(IStruct));
      // TODO: validate type meets IPure* contracts
      ImplementsIPureAbsPointer = interfaces.Contains(typeof(IPureAbsPointer));
      ImplementsIAbsPointer = ImplementsIPureAbsPointer || interfaces.Contains(typeof(IAbsPointer));
      ImplementsICapability = interfaces.Contains(typeof(ICapability));
      KnownDataWords = (ushort)(int)(typeof(T).GetTypeInfo().GetDeclaredField("KNOWN_DATA_WORDS")?.GetValue(null) ?? 0);
      KnownPointerWords = (ushort)(int)(typeof(T).GetTypeInfo().GetDeclaredField("KNOWN_POINTER_WORDS")?.GetValue(null) ?? 0);
    }
  }
}