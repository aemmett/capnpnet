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

    public static void Range(int index, int count, int length)
    {
      Range(index, count);
      if (index + count > length) throw new ArgumentOutOfRangeException();
    }

    public static void NotNull(object obj, string name)
    {
      if (obj == null) throw new ArgumentNullException(name);
    }

    public static void IsTrue(bool condition)
    {
      if (condition == false) throw new InvalidOperationException();
    }
  }
}