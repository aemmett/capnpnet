namespace CapnpNet
{
  public struct AnyStruct : IStruct
  {
    public Struct Struct { get; set; }

    public AbsPointer Pointer => this.Struct.Pointer;
  }
}