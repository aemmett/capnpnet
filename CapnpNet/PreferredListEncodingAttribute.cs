using System;

namespace CapnpNet
{
  [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
  public sealed class PreferredListEncodingAttribute : Attribute
  {
    public PreferredListEncodingAttribute(ElementSize elementSize)
    {
      this.ElementSize = elementSize;
    }

    public ElementSize ElementSize { get; }
  }
}
