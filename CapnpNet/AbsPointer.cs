using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CapnpNet
{
  [StructLayout(LayoutKind.Sequential)]
  public struct AbsPointer : IPureAbsPointer
  {
    private Pointer _tag;
    private Segment _segment;
    private int _baseOffset; // TODO: bake into _tag.WordOffset and remove?

    public AbsPointer(Segment segment, int offsetAfterPointer, Pointer tag)
    {
      _segment = segment;
      _baseOffset = offsetAfterPointer;
      _tag = tag;
    }

    public Segment Segment => _segment;
    public int BaseOffset => _baseOffset;
    public int DataOffset => _baseOffset + _tag.WordOffset;
    public Pointer Tag => _tag;
    public bool IsIndirect => _tag.Type == PointerType.Far;
    public bool IsNull => _tag.RawValue == 0;
    public bool IsEmpty
    {
      get
      {
        this.Dereference();
        if (_tag.Is(out StructPointer s)) return s.DataWords == 0 && s.PointerWords == 0;
        else if (_tag.Is(out ListPointer l)) return l.ElementCount == 0;
        else return false;
      }
    }

    internal ref ulong Data
    {
      get
      {
        Debug.Assert(this.IsIndirect == false);
        return ref _segment[this.DataOffset | Word.unit];
      }
    }

    AbsPointer IAbsPointer.Pointer => this;

    public static implicit operator AbsPointer(OtherPointer op) => new AbsPointer(null, 0, op);

    public bool Is<T>(out T @struct) where T : struct, IStruct
    {
      if (this.Tag.Type == PointerType.Struct)
      {
        @struct = default(T); // annoying :/
        Unsafe.As<T, Struct>(ref @struct) = new Struct(this.Segment, this.DataOffset, this.Tag.DataWords, this.Tag.PointerWords);
        return true;
      }

      @struct = default(T);
      return false;
    }
    
    public bool IsStruct(out Struct @struct)
    {
      if (this.Tag.Type == PointerType.Struct)
      {
        @struct = new Struct(this.Segment, this.DataOffset, this.Tag.DataWords, this.Tag.PointerWords);
        return true;
      }

      @struct = default(Struct);
      return false;
    }

    public Pointer ToPointer(Segment srcSegment, int baseOffset)
    {
      var tag = this.Tag;
      if (this.IsNull
        || tag.Type == PointerType.Other)
      {
        return tag;
      }

      if (this.Segment == srcSegment)
      {
        tag.WordOffset = this.DataOffset - baseOffset;
        return tag;
      }
      else
      {
        uint segmentIndex = (uint)this.Segment.SegmentIndex;
        if (this.Segment.TryAllocate(1, out int padOffset))
        {
          tag.WordOffset = this.DataOffset - (padOffset + 1);
          this.Segment[padOffset | Word.unit] = tag.RawValue;

          return new FarPointer
          {
            Type = PointerType.Far,
            LandingPadOffset = (uint)padOffset,
            TargetSegmentId = segmentIndex,
          };
        }
        else
        {
          this.Segment.Message.Allocate(2, out padOffset, out Segment segment);
          segment[padOffset | Word.unit] = new FarPointer
          {
            Type = PointerType.Far,
            LandingPadOffset = (uint)this.DataOffset,
            TargetSegmentId = segmentIndex
          }.RawValue;
          tag.WordOffset = 0;
          segment[padOffset + 1 | Word.unit] = tag.RawValue;

          return new FarPointer
          {
            Type = PointerType.Far,
            IsDoubleFar = true,
            LandingPadOffset = (uint)padOffset,
            TargetSegmentId = (uint)segment.SegmentIndex,
          };
        }
      }
    }

    public void Dereference()
    {
      if (this.IsIndirect)
      {
        this.Segment.Message.Traverse(ref _tag, ref _segment, out _baseOffset);
      }
    }

    public AbsPointer CopyTo(Message dest)
    {
      this.Dereference();

      var dataOffset = this.DataOffset;

      if (this.Tag.Is(out StructPointer sptr))
      {
        var srcStruct = new Struct(this.Segment, dataOffset, sptr.DataWords, sptr.PointerWords);
        var dstStruct = srcStruct.CopyTo(dest);
        return dstStruct.ToAbsPointer();
      }
      else if (this.Tag.Is(out ListPointer list))
      {
        ref ulong src = ref this.Segment[dataOffset | Word.unit];
        ref Pointer pointers = ref Unsafe.As<ulong, Pointer>(ref src);

        int elementsPerWord;
        if (list.ElementSize == ElementSize.OneBit) elementsPerWord = 64;
        else if (list.ElementSize == ElementSize.OneByte) elementsPerWord = 8;
        else if (list.ElementSize == ElementSize.TwoBytes) elementsPerWord = 4;
        else if (list.ElementSize == ElementSize.FourBytes) elementsPerWord = 2;
        else if (list.ElementSize >= ElementSize.EightBytesNonPointer) elementsPerWord = 1;
        else
        {
          throw new NotSupportedException(); // zero
        }

        var words = ((int)list.ElementCount + elementsPerWord - 1) / elementsPerWord;
        if (list.ElementSize == ElementSize.Composite) words++;

        dest.Allocate(words, out int offset, out Segment newSeg);
        ref ulong dst = ref newSeg[offset | Word.unit];

        int i, dataWords, pointerWords;
        if (list.ElementSize == ElementSize.EightBytesPointer)
        {
          i = 0;
          dataWords = 0;
          pointerWords = 1;
        }
        else if (list.ElementSize == ElementSize.Composite)
        {
          var tag = (StructPointer)pointers;
          dst = src; // copy tag

          i = 1;
          dataWords = tag.DataWords;
          pointerWords = tag.PointerWords;
        }
        else
        {
          i = 0;
          dataWords = 1;
          pointerWords = 0;
        }

        int totalWords = dataWords + pointerWords;

        for (; i < words; i += totalWords)
        {
          for (int j = i; j < i + dataWords; j++)
          {
            Unsafe.Add(ref dst, j) = Unsafe.Add(ref src, j);
          }

          for (int j = i; j < i + pointerWords; j++)
          {
            ref Pointer ptrElem = ref Unsafe.Add(ref pointers, j);
            var absPtr = new AbsPointer(this.Segment, dataOffset + j + 1, ptrElem);
            Unsafe.Add(ref dst, j) = absPtr
              .CopyTo(dest)
              .ToPointer(newSeg, offset + j + 1)
              .RawValue;
          }
        }

        return new AbsPointer(newSeg, offset, list);
      }
      else if (this.Tag.Type == PointerType.Other)
      {
        return new AbsPointer(null, 0, this.Tag);
      }
      else
      {
        throw new InvalidOperationException(); // should be unreachable
      }
    }
  }
}