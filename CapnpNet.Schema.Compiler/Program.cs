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

      var codeGenerator = new CSharpCodeGenerator(msg.GetRoot<CodeGeneratorRequest>());
      var outputs = codeGenerator.GenerateSources();
      foreach (var kvp in outputs)
      {
        File.WriteAllText(kvp.Key + ".cs", kvp.Value);
      }
    }
  }
}