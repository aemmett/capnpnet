using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CapnpNet.Schema.Compiler
{
  public static class FormattableStringUtilties
  {
    private static Regex formatSplitter = new Regex(@"(?<!{){\d+(?::.*?)}");

    public static IEnumerable<string> S(FormattableString fs)
    {
      var parts = formatSplitter.Split(fs.Format);
      for (int i = 0; i < parts.Length - 1; i++)
      {
        yield return parts[i];
        var arg = fs.GetArgument(i);
        if (arg is IEnumerable<string> strings)
        {
          foreach (var str in strings)
          {
            yield return str;
          }
        }
        else
        {
          yield return arg.ToString();
        }
      }

      yield return parts[parts.Length - 1];
    }
  }
}
