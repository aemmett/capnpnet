using System.Linq;

namespace CapnpNet
{
  internal class ReflectionCache<T> where T : struct
  {
    public static readonly bool ImplementsIStruct;
    public static readonly ushort KnownDataWords;
    public static readonly ushort KnownPointerWords;

    static ReflectionCache()
    {
      ImplementsIStruct = typeof(T).GetInterfaces().Contains(typeof(IStruct));
      KnownDataWords = (ushort)(int)(typeof(T).GetField("KNOWN_DATA_WORDS")?.GetValue(null) ?? 0);
      KnownPointerWords = (ushort)(int)(typeof(T).GetField("KNOWN_POINTER_WORDS")?.GetValue(null) ?? 0);
    }
  }
}