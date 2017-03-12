using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace CapnpNet
{
#if SPAN
  public static class SpanExtensions
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetOrDefault<T>(this Span<ulong> span, int index)
      where T : struct
    {
      var typedSpan = span.Cast<ulong, T>();
      return index < typedSpan.Length ? typedSpan[index] : default(T);
    }

    public static string DecodeUTF8(this Span<byte> span)
    {
      // exclude \0 terminator at the end
      return Encoding.UTF8.GetString(span.Slice(0, span.Length - 1).ToArray());
    }
  } 
#endif
}