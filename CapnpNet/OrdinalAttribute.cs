using System;

namespace CapnpNet
{
  [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
  public sealed class OrdinalAttribute : Attribute
  {
    public OrdinalAttribute(ushort ordinal)
    {
      this.Ordinal = ordinal;
    }

    public ushort Ordinal { get; set; }
  }
}