#if PSUEDO_SPAN

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CapnpNet
{
  [StructLayout(LayoutKind.Sequential)]
  internal sealed class Pinnable
  {
    public byte pin;
  }

  internal static class Helpers
  {
    public static readonly IntPtr byteArrayOffset = MeasureByteArrayOffset();

    private unsafe static IntPtr MeasureByteArrayOffset()
    {
      var arr = new byte[1];
      fixed (byte* elem0Ptr = &arr[0])
      {
        return (IntPtr)(elem0Ptr - (byte*)Unsafe.Read<IntPtr>(Unsafe.AsPointer(ref arr)).ToPointer());
      }
    }
  }

  internal struct Span<T>
    where T : struct
  {
    public Pinnable pinnable;
    public IntPtr offset;
    public int length;

    public Span(Pinnable p, IntPtr o, int l)
    {
      pinnable = p;
      offset = o;
      length = l;
    }

    public ref T Ref
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
        if (pinnable == null)
        {
          unsafe
          {
            return ref Unsafe.AsRef<T>(offset.ToPointer());
          }
        }
        else
        {
          return ref Unsafe.As<byte, T>(ref Unsafe.Add(ref pinnable.pin, offset.ToInt32()));
        }
      }
    }
  }
}

#endif