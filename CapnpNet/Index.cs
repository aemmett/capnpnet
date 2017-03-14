namespace CapnpNet
{
  public sealed class Word
  {
    public static readonly Index<Word> unit = new Index<Word>(1);

    private Word()
    {
    }
  }

  public sealed class Byte
  {
    public static readonly Index<Byte> unit = new Index<Byte>(1);

    private Byte()
    {
    }
  }

  public struct Index<T>
  {
    public int index;

    public Index(int i)
    {
      index = i;
    }

    public static implicit operator int(Index<T> i) => i.index;

    public static explicit operator Index<T>(int i) => new Index<T>(i);

    public static Index<T> operator |(int i, Index<T> unit) => new Index<T>(i);
  }
}