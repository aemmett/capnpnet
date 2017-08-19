using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CapnpNet
{
  public static class TypeHelpers
  {
    public static bool IsNumericPrimitive<T>()
    {
      var t = typeof(T);
      return t == typeof(byte)
        || t == typeof(sbyte)
        || t == typeof(short)
        || t == typeof(ushort)
        || t == typeof(int)
        || t == typeof(uint)
        || t == typeof(long)
        || t == typeof(ulong)
        || t == typeof(float)
        || t == typeof(double);
    }

    public static ElementSize ToElementSize<T>()
    {
      switch (Unsafe.SizeOf<T>())
      {
        case 1: return ElementSize.OneByte;
        case 2: return ElementSize.TwoBytes;
        case 4: return ElementSize.FourBytes;
        case 8: return ElementSize.EightBytesNonPointer;
        default: throw new InvalidOperationException("Not a valid size");
      }
    }

    public static int SizeOf(this ElementSize size)
    {
      switch (size)
      {
        case ElementSize.Zero: return 0;
        case ElementSize.OneBit: throw new ArgumentException("size is OneBit", nameof(size));
        case ElementSize.OneByte: return 1;
        case ElementSize.TwoBytes: return 2;
        case ElementSize.FourBytes: return 4;
        case ElementSize.EightBytesNonPointer:
        case ElementSize.EightBytesPointer:
          return 8;
        case ElementSize.Composite: throw new ArgumentException("size is Composite", nameof(size));
        default: throw new ArgumentException($"Invalid ElementSize: {size}");
      }
    }

    public static void AssertSize<T>(ElementSize elementSize)
    {
      if (TypeHelpers.ToElementSize<T>() != elementSize)
      {
        throw new InvalidOperationException($"Element size mismatch, T is {Unsafe.SizeOf<T>()}, pointer is {elementSize}");
      }
    }

    // TODO: look into using unsafe
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Xor(float a, float b)
    {
      if (a == default) return b;
      if (b == default) return a;

      var val = new Float32Helper();
      var def = new Float32Helper();
      val.floatVal = a;
      def.floatVal = b;
      val.intVal ^= def.intVal;
      return val.floatVal;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Xor(double a, double b)
    {
      if (a == default) return b;
      if (b == default) return a;

      var val = new Float64Helper();
      var def = new Float64Helper();
      val.doubleVal = a;
      def.doubleVal = b;
      val.longVal ^= def.longVal;
      return val.doubleVal;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct Float32Helper
    {
      [FieldOffset(0)]
      public int intVal;

      [FieldOffset(0)]
      public float floatVal;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct Float64Helper
    {
      [FieldOffset(0)]
      public long longVal;

      [FieldOffset(0)]
      public double doubleVal;
    }
  }
}