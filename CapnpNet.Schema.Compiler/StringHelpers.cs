using System.Collections.Generic;

namespace CapnpNet.Schema.Compiler
{
  internal static class StringHelpers
  {
    public static string StringJoin(this IEnumerable<string> parts, string separator)
    {
      return string.Join(separator, parts);
    }
  }
}