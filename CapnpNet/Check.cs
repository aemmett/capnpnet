using System;

namespace CapnpNet
{
  internal static class Check
  {
    public static void Positive(int num)
    {
      if (num < 0) throw new ArgumentException();
    }

    public static void Range(int index, int count)
    {
      if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
    }
  }
}