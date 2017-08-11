using System;
using System.IO;

namespace CapnpNet.Schema.Compiler
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Message msg;
      using (var stdin = Console.OpenStandardInput())
      {
        msg = Message.DecodeAsync(stdin).Result;
      }

      var defaultNamespace = "Schema";
      if (args.Length > 0)
      {
        defaultNamespace = args[0];
      }

      var codeGenerator = new CSharpCodeGenerator(msg.GetRoot<CodeGeneratorRequest>(), defaultNamespace);
      var outputs = codeGenerator.GenerateSources();
      foreach (var kvp in outputs)
      {
        File.WriteAllText(kvp.Key + ".g.cs", kvp.Value);
      }
    }
  }
}