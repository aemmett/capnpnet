using System;
using System.Runtime.InteropServices;

namespace CapnpNet
{
  public enum PointerType
  {
    Struct = 0,
    List = 1,
    Far = 2,
    Other = 3,
  }

  public enum ElementSize
  {
    Zero = 0,
    OneBit = 1,
    OneByte = 2,
    TwoBytes = 3,
    FourBytes = 4,
    EightBytesNonPointer = 5,
    EightBytesPointer = 6,
    Composite = 7,
  }

  public enum OtherPointerType
  {
    Capability = 0,
  }

  [StructLayout(LayoutKind.Explicit)]
  public struct Pointer : IEquatable<Pointer>
  {
    [FieldOffset(0)]
    private ulong _raw;

    // TODO: little-endian compiler/runtime checks/conversions?
    // maybe I should always just twiddle a ulong field
    [FieldOffset(0)]
    private byte _b0;

    [FieldOffset(0)]
    private int _si0;

    [FieldOffset(0)]
    private uint _ui0;

    [FieldOffset(4)]
    private ushort _us4;

    [FieldOffset(6)]
    private ushort _us6;

    [FieldOffset(4)]
    private uint _ui4;

    public ulong RawValue => _raw;

    public PointerType Type
    {
      get { return (PointerType)(_b0 & 3); }
      set { _si0 = (this.WordOffset << 2) | ((int)value & 3); }
    }

    public void AssertType(PointerType type)
    {
      if (this.Type != type)
      {
        throw new InvalidOperationException($"Expected pointer type of {type} but found {this.Type}");
      }
    }

    public bool Equals(Pointer other)
    {
      return this._raw == other._raw;
    }

    public override bool Equals(object obj)
    {
      return obj is Pointer && this.Equals((Pointer)obj);
    }

    public override int GetHashCode()
    {
      return _raw.GetHashCode();
    }

    public static bool operator ==(Pointer a, Pointer b)
    {
      return a.Equals(b);
    }
    public static bool operator !=(Pointer a, Pointer b)
    {
      return !(a == b);
    }

    public bool Is(out StructPointer structPtr)
    {
      if (this.Type == PointerType.Struct)
      {
        structPtr = (StructPointer)this;
        return true;
      }
      else
      {
        structPtr = default(StructPointer);
        return false;
      }
    }
    
    public bool Is(out ListPointer listPtr)
    {
      if (this.Type == PointerType.List)
      {
        listPtr = (ListPointer)this;
        return true;
      }
      else
      {
        listPtr = default(ListPointer);
        return false;
      }
    }

    public bool Is(out FarPointer farPtr)
    {
      if (this.Type == PointerType.Far)
      {
        farPtr = (FarPointer)this;
        return true;
      }
      else
      {
        farPtr = default(FarPointer);
        return false;
      }
    }
    
    public bool Is(out OtherPointer otherPtr) // or specifically a capability pointer?
    {
      if (this.Type == PointerType.Other)
      {
        otherPtr = (OtherPointer)this;
        return true;
      }
      else
      {
        otherPtr = default(OtherPointer);
        return false;
      }
    }

    /// <summary>
    /// Offset to content, relative to the word after this pointer.
    /// </summary>
    public int WordOffset
    {
      get { return _si0 >> 2; }
      set { _si0 = (value << 2) | (int)this.Type; }
    }

    #region Struct

    public ushort DataWords
    {
      get { return _us4; }
      set { _us4 = value; }
    }

    public ushort PointerWords
    {
      get { return _us6; }
      set { _us6 = value; }
    }

    #endregion Struct

    #region List

    public ElementSize ElementSize
    {
      get { return (ElementSize)(_us4 & 7); }
      set { _us4 = (ushort)((_us4 & ~7U) | ((uint)value & 7U)); }
    }

    /// <summary>
    /// Number of elements, except when ElementSize == Composite, then EC is number of words (after tag)
    /// </summary>
    public uint ElementCount
    {
      get { return _ui4 >> 3; }
      set { _ui4 = (value << 3) | (_ui4 & 7); }
    }

    #endregion List

    #region Far

    public bool IsDoubleFar
    {
      get { return (_b0 & 4) > 0; }
      set { if (value) _b0 |= 4; else _b0 &= 0xFB; }
    }

    /// <summary>
    /// Offset to landing pad from beginning of target segment.
    /// </summary>
    public uint LandingPadOffset
    {
      get { return _ui0 >> 3; }
      set { _ui0 = (value << 3) | (_ui0 & 7); }
    }

    public uint TargetSegmentId
    {
      get { return _ui4; }
      set { _ui4 = value; }
    }

    #endregion Far

    #region Other

    public OtherPointerType OtherPointerType
    {
      get { return (OtherPointerType)(_ui0 >> 2); }
      set { _ui0 = ((uint)value << 2) | (_ui0 & 3); }
    }

    public uint CapabilityId
    {
      get { return _ui4; }
      set { _ui4 = value; }
    }

    #endregion Other
  }

  public struct StructPointer
  {
    private Pointer _p;

    public static implicit operator Pointer(StructPointer p)
    {
      return p._p;
    }

    public static explicit operator StructPointer(Pointer p)
    {
      return new StructPointer(p);
    }

    public StructPointer(Pointer p)
    {
      if (p.Type != PointerType.Struct) throw new ArgumentException($"Expected pointer type of Struct ({p.RawValue.ToString("X16")})");

      _p = p;
    }

    public ulong RawValue => _p.RawValue;

    public PointerType Type
    {
      get { return _p.Type; }
      set { _p.Type = value; }
    }

    public int WordOffset
    {
      get { return _p.WordOffset; }
      set { _p.WordOffset = value; }
    }

    public ushort DataWords
    {
      get { return _p.DataWords; }
      set { _p.DataWords = value; }
    }

    public ushort PointerWords
    {
      get { return _p.PointerWords; }
      set { _p.PointerWords = value; }
    }
  }

  public struct ListPointer
  {
    private Pointer _p;

    public static implicit operator Pointer(ListPointer p)
    {
      return p._p;
    }

    public static explicit operator ListPointer(Pointer p)
    {
      return new ListPointer(p);
    }

    public ListPointer(Pointer p)
    {
      if (p.Type != PointerType.List) throw new ArgumentException("Expected pointer type of List", "p");

      _p = p;
    }

    public PointerType Type
    {
      get { return _p.Type; }
      set { _p.Type = value; }
    }

    public int WordOffset
    {
      get { return _p.WordOffset; }
      set { _p.WordOffset = value; }
    }

    public ElementSize ElementSize
    {
      get { return _p.ElementSize; }
      set { _p.ElementSize = value; }
    }

    public uint ElementCount
    {
      get { return _p.ElementCount; }
      set { _p.ElementCount = value; }
    }
  }

  public struct FarPointer
  {
    private Pointer _p;

    public static implicit operator Pointer(FarPointer p)
    {
      return p._p;
    }

    public static explicit operator FarPointer(Pointer p)
    {
      return new FarPointer(p);
    }

    public FarPointer(Pointer p)
    {
      if (p.Type != PointerType.Far) throw new ArgumentException("Expected pointer type of Far", "p");

      _p = p;
    }
    
    public ulong RawValue => _p.RawValue;

    public PointerType Type
    {
      get { return _p.Type; }
      set { _p.Type = value; }
    }

    public bool IsDoubleFar
    {
      get { return _p.IsDoubleFar; }
      set { _p.IsDoubleFar = value; }
    }

    /// <summary>
    /// Offset to landing pad from beginning of target segment.
    /// </summary>
    public uint LandingPadOffset
    {
      get { return _p.LandingPadOffset; }
      set { _p.LandingPadOffset = value; }
    }

    public uint TargetSegmentId
    {
      get { return _p.TargetSegmentId; }
      set { _p.TargetSegmentId = value; }
    }
  }

  public struct OtherPointer
  {
    private Pointer _p;

    public static implicit operator Pointer(OtherPointer p)
    {
      return p._p;
    }

    public static explicit operator OtherPointer(Pointer p)
    {
      return new OtherPointer(p);
    }

    public OtherPointer(Pointer p)
    {
      if (p.Type != PointerType.Other) throw new ArgumentException("Expected pointer type of Other", nameof(p));

      _p = p;
    }
    
    public ulong RawValue => _p.RawValue;

    public PointerType Type
    {
      get { return _p.Type; }
      set { _p.Type = value; }
    }

    public OtherPointerType OtherPointerType
    {
      get { return _p.OtherPointerType; }
      set { _p.OtherPointerType = value; }
    }

    public uint CapabilityId
    {
      get { return _p.CapabilityId; }
      set { _p.CapabilityId = value; }
    }
  }
}