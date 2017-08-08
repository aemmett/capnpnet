using CapnpNet.Rpc;
using System;
using System.Runtime.CompilerServices;

namespace CapnpNet
{
  /// <summary>
  /// Implementors must be a struct whose only field is <see cref="Struct"/>.
  /// </summary>
  public interface IStruct // TODO: rename to IPureStruct
  {
    Struct Struct { get; }
  }

  public static class StructExtensions
  {
    public static T As<T>(this Struct s) where T : struct, IStruct => Unsafe.As<Struct, T>(ref s);

    public static Struct GetStruct<T>(this T structObj) where T : struct, IStruct => structObj.Struct;

    public static bool Compact<T>(this T structObj, bool dataOnly = true) where T : struct, IStruct
    {
      var s = structObj.Struct;
      if (s.IsPartialStruct)
      {
        return false;
      }

      var originalEnd = s.StructWordOffset + s.DataWords + s.PointerWords;
      int savedWords = 0;

      var dw = s.DataWords;
      while (dw > 0 && s.ReadInt64(dw - 1) == 0)
      {
        dw--;
        savedWords++;
      }

      var pw = s.PointerWords;
      if (dataOnly == false)
      {
        while (pw > 0 && s.ReadRawPointer(pw - 1).RawValue == 0)
        {
          pw--;
          savedWords++;
        }
      }

      if (savedWords == 0) return false;

      var newStruct = new Struct(s.Segment, s.StructWordOffset, dw, pw);

      var pointerShift = s.DataWords - dw;
      if (pointerShift > 0)
      {
        // need to shift pointers over
        for (int i = 0; i < pw; i++)
        {
          var ptr = s.ReadRawPointer(i);
          if (ptr.Type == PointerType.Struct || ptr.Type == PointerType.List) ptr.WordOffset += pointerShift;

          newStruct.WriteRawPointer(i, ptr);
        }
      }

      var seg = s.Segment;
      for (int i = 0; i < savedWords; i++)
      {
        seg[originalEnd - i - 1 | Word.unit] = 0;
      }

      Unsafe.As<T, Struct>(ref structObj) = newStruct;

      seg.TryReclaim(originalEnd, savedWords);

      return true;
    }

    public static T CopyTo<T>(this T structObj, Message dest) where T : struct, IStruct
    {
      var s = structObj.Struct.CopyTo(dest);
      return Unsafe.As<Struct, T>(ref s);
    }
  }
  
  public struct Struct
  {
    private readonly Segment _segment;
    private readonly int _structWordOffset; // in the encoding spec, technically this is a uint, but currently I bottom out at an API that uses int :(
    private readonly ushort _dataWords, _pointerWords;
    private readonly sbyte _upgradedListElementByteOffset;

    public static bool operator ==(Struct a, Struct b)
    {
      return a._segment == b._segment
        && a._structWordOffset == b._structWordOffset
        && a._dataWords == b._dataWords
        && a._pointerWords == b._pointerWords
        && a._upgradedListElementByteOffset == b._upgradedListElementByteOffset;
    }

    public static bool operator !=(Struct a, Struct b) => !(a == b);
    
    public override bool Equals(object obj) => obj is Struct s && this == s;
    
    public override int GetHashCode()
    {
      return _segment.GetHashCode() * 17 + _structWordOffset;
    }

    public Struct(Segment segment, int pointerOffset, StructPointer pointer)
      : this(
          segment,
          pointerOffset + pointer.WordOffset,
          pointer.DataWords,
          pointer.PointerWords)
    {
    }

    public Struct(Segment segment, int structWordOffset, ushort dataWords, ushort pointerWords, sbyte upgradedListElementSize = -1)
    {
      _segment = segment;
      _structWordOffset = structWordOffset;
      _dataWords = dataWords;
      _pointerWords = pointerWords;
      _upgradedListElementByteOffset = upgradedListElementSize;
    }

    public Pointer[] PointersDebug
    {
      get
      {
        var ret = new Pointer[_pointerWords];
        ref var pointers = ref Unsafe.As<ulong, Pointer>(ref _segment[_structWordOffset + _dataWords | Word.unit]);
        for (int i = 0; i < _pointerWords; i++)
        {
          ret[i] = Unsafe.Add(ref pointers, i);
        }

        return ret;
      }
    }

#if SPAN
    // <summary>
    // Note, when _upgradedListElementOffset > 0, Span will be one word long, and may comprise of multiple structs.
    // </summary>
    public Span<ulong> Span => _segment.Span.Slice((int)_structWordOffset, _dataWords + _pointerWords);
#endif

    public int StructWordOffset => _structWordOffset;
    public Segment Segment => _segment;
    public ushort DataWords => _dataWords;
    public ushort PointerWords => _pointerWords;
    
    public bool IsEmpty => _dataWords == 0 && _pointerWords == 0;
    public bool IsPartialStruct => _upgradedListElementByteOffset != -1;

    public override string ToString() => $"Struct(S={_segment.SegmentIndex}, O={_structWordOffset}, DW={_dataWords}, PW={_pointerWords})";
    
    public int CalculateSize()
    {
      // TODO: traversal depth limit
      var size = this.DataWords + this.PointerWords;

      for (int i = 0; i < this.PointerWords; i++)
      {
        var ptr = this.ReadRawPointer(i);
        var dataSeg = _segment;
        var srcMsg = _segment.Message;
        // TODO: only dereference far; don't count against transversal limit
        srcMsg.Traverse(ref ptr, ref dataSeg, out int baseOffset);
        
        if (ptr.Type == PointerType.Struct)
        {
          size += this.DereferenceRawStruct(i).CalculateSize();
        }
        else if (ptr.Is(out ListPointer list))
        {
          int elementsPerWord;
          if (list.ElementSize == ElementSize.OneBit) elementsPerWord = 64;
          else if (list.ElementSize == ElementSize.OneByte) elementsPerWord = 8;
          else if (list.ElementSize == ElementSize.TwoBytes) elementsPerWord = 4;
          else if (list.ElementSize == ElementSize.FourBytes) elementsPerWord = 2;
          else if (list.ElementSize >= ElementSize.EightBytesNonPointer) elementsPerWord = 1;
          else throw new NotSupportedException();

          var words = ((int)list.ElementCount + elementsPerWord - 1) / elementsPerWord;
          if (list.ElementSize == ElementSize.Composite) words++;

          size += words;
        }
      }

      return size;
    }

    public AbsPointer ToAbsPointer() => new AbsPointer(_segment, 0, new StructPointer
    {
      Type = PointerType.Struct,
      WordOffset = _structWordOffset,
      DataWords = _dataWords,
      PointerWords = _pointerWords,
    });

    public Struct CopyTo(Message dest)
    {
      if (this.IsEmpty) return default(Struct);

      if (this.Segment.Message == dest) return this;

      var newS = dest.Allocate(this.DataWords, this.PointerWords);
      var destSeg = newS.Segment;
      var srcSeg = this.Segment;
      var srcMsg = srcSeg.Message;

      // copy data
      ref ulong src = ref srcSeg[this.StructWordOffset | Word.unit];
      ref ulong dst = ref destSeg[newS.StructWordOffset | Word.unit];
      for (int i = 0; i < this.DataWords; i++)
      {
        Unsafe.Add(ref dst, i) = Unsafe.Add(ref src, i);
      }

      int srcPointerBase = _structWordOffset + _dataWords + 1;
      int dstPointerBase = newS.StructWordOffset + newS.DataWords + 1;
      for (int i = 0; i < this.PointerWords; i++)
      {
        Unsafe.Add(ref dst, _dataWords + i) =
          new AbsPointer(_segment, srcPointerBase + i, this.Pointer(i))
            .CopyTo(dest)
            .ToPointer(destSeg, dstPointerBase + i)
            .RawValue;
      }

      return newS;
    }
    
    private ref Pointer Pointer(int index)
    {
      Check.Range(index, _pointerWords);
      return ref Unsafe.As<ulong, Pointer>(ref _segment[_structWordOffset + _dataWords + index | Word.unit]);
    }

#region Read methods
    public Pointer ReadRawPointer(int pointerIndex)
    {
      Check.Positive(pointerIndex);

      // TODO: how to handle default?
      if (pointerIndex >= this.PointerWords) return new Pointer();

      return this.Pointer(pointerIndex);
    }

    public T DereferencePointer<T>(int pointerIndex)
      where T : struct, IAbsPointer
    {
      if (this.DereferenceCore(pointerIndex, out var pointer, out var baseOffset, out var targetSegment))
      {
        var p = new AbsPointer(targetSegment, baseOffset, pointer);
        if (ReflectionCache<T>.ImplementsIPureAbsPointer)
        {
          // TODO: grab expected pointer type(s) from reflection cache
          // - or, to put a Verify method on IAbsPointer; FlatArray's verifications is complicated.
          return Unsafe.As<AbsPointer, T>(ref p);
        }
        else if (ReflectionCache<T>.ImplementsIStruct) // pure struct
        {
          if (p.IsStruct(out var s) == false)
          {
            throw new InvalidOperationException($"Pointer type {p.Tag.Type} is not Struct");
          }

          return Unsafe.As<Struct, T>(ref s);
        }

        throw new InvalidOperationException($"{typeof(T).Name} expected to implement either IPureStruct or IPureAbsPointer");
      }

      // TODO: how to handle default?
      throw new NotImplementedException();
    }

    public AbsPointer DereferenceAbsPointer(int pointerIndex)
    {
      if (this.DereferenceCore(pointerIndex, out var pointer, out var baseOffset, out var targetSegment))
      {
        return new AbsPointer(targetSegment, baseOffset, pointer);
      }

      // TODO: how to handle default?
      throw new NotImplementedException();
    }

    public Struct DereferenceRawStruct(int pointerIndex)
    {
      if (this.DereferenceCore(pointerIndex, out var pointer, out var baseOffset, out var targetSegment))
      {
        return new Struct(targetSegment, baseOffset, (StructPointer)pointer);
      }

      // TODO: how to handle default?
      return new Struct();
    }

    public T DereferenceStruct<T>(int pointerIndex) where T : struct, IStruct
    {
      if (this.DereferenceCore(pointerIndex, out var pointer, out var baseOffset, out var targetSegment))
      {
        var s = new Struct(targetSegment, baseOffset, (StructPointer)pointer);
        return Unsafe.As<Struct, T>(ref s);
      }

      // TODO: how to handle default?
      return default(T);
    }

    public Text DereferenceText(int pointerIndex)
    {
      if (this.DereferenceCore(pointerIndex, out var pointer, out var baseOffset, out var targetSegment))
      {
        var listPointer = (ListPointer)pointer;
        TypeHelpers.AssertSize<byte>(listPointer.ElementSize);

        return new Text(targetSegment, baseOffset, listPointer);
      }

      // TODO: how to handle default?
      return new Text();
    }

    public PrimitiveList<T> DereferenceList<T>(int pointerIndex)
      where T : struct
    {
      if (this.DereferenceCore(pointerIndex, out var pointer, out var baseOffset, out var targetSegment))
      {
        var listPointer = (ListPointer)pointer;

        if (listPointer.ElementSize == ElementSize.Zero) return new PrimitiveList<T>(); // ummm...

        TypeHelpers.AssertSize<T>(listPointer.ElementSize);

        return new PrimitiveList<T>(targetSegment, baseOffset, listPointer);
      }

      // TODO: how to handle default?
      return new PrimitiveList<T>();
    }

    public BoolList DereferenceBoolList(int pointerIndex)
    {
      if (this.DereferenceCore(pointerIndex, out var pointer, out var baseOffset, out var targetSegment))
      {
        var listPointer = (ListPointer)pointer;

        if (listPointer.ElementSize != ElementSize.OneBit) throw new ArgumentException($"Expected list with element size of one bit, but found {pointer.ElementSize}");
      
        return new BoolList(
          targetSegment,
          baseOffset,
          listPointer);
      }

      // TODO: how to handle default?
      return new BoolList();
    }

    public CompositeList<T> DereferenceCompositeList<T>(int pointerIndex) where T : struct, IStruct
    {
      if (this.DereferenceCore(pointerIndex, out var pointer, out var baseOffset, out var targetSegment))
      {
        var listPointer = (ListPointer)pointer;

        if (listPointer.ElementSize != ElementSize.Composite) throw new ArgumentException($"Expected composite list, but found {listPointer.ElementSize} list");
      
        return new CompositeList<T>(
          targetSegment,
          baseOffset,
          listPointer);
      }

      // TODO: how to handle default?
      return new CompositeList<T>();
    }

    public T ReadInterface<T>(int pointerIndex) where T : ICapability
    {
      if (pointerIndex >= this.PointerWords)
      {
        return default(T);
      }

      ref Pointer otherPointer = ref this.Pointer(pointerIndex);

      return (T)this.Segment.Message.LocalCaps[otherPointer.CapabilityId].Capability;
    }

    // TODO: replace with AbsPointer
    private bool DereferenceCore(int pointerIndex, out Pointer pointer, out int baseOffset, out Segment targetSegment)
    {
      Check.Positive(pointerIndex);

      if (pointerIndex >= this.PointerWords)
      {
        pointer = default(Pointer);
        baseOffset = 0;
        targetSegment = null;
        return false;
      }
      
      pointer = this.Pointer(pointerIndex);
      
      if (pointer == default(Pointer))
      {
        baseOffset = 0;
        targetSegment = null;
        return false;
      }

      targetSegment = _segment;
      if (!_segment.Message.Traverse(ref pointer, ref targetSegment, out baseOffset))
      {
        baseOffset = _structWordOffset + _dataWords + pointerIndex + 1;
      }
      
      return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T GetOrDefault<T>(int index)
      where T : struct
    {
      Check.Positive(index);

      if (_upgradedListElementByteOffset >= 0)
      {
        // this is a list element upgraded to a struct; only the first field is present
        if (index > 0) return default(T);
        
        return Unsafe.As<byte, T>(ref _segment[_structWordOffset * sizeof(ulong) + _upgradedListElementByteOffset / Unsafe.SizeOf<T>() | Byte.unit]);
      }
      else
      {
        if (index * Unsafe.SizeOf<T>() >= _dataWords * sizeof(ulong)) return default(T);

        return Unsafe.As<byte, T>(ref _segment[_structWordOffset * sizeof(ulong) + index * Unsafe.SizeOf<T>() | Byte.unit]);
      }
    }

    // TODO: option which throws instead of return default for out of range?
    // TODO: aggressiveinlining attribute? Also, if inlined, does defaultValue = 0 get optimized away?
    // TODO: take a second look at these xor operations for the smaller integers
    public sbyte ReadInt8(int index, sbyte defaultValue = 0) => (sbyte)(this.GetOrDefault<sbyte>(index) ^ defaultValue);
    public byte ReadUInt8(int index, byte defaultValue = 0) => (byte)(this.GetOrDefault<byte>(index) ^ defaultValue);
    public short ReadInt16(int index, short defaultValue = 0) => (short)(this.GetOrDefault<short>(index) ^ defaultValue);
    public ushort ReadUInt16(int index, ushort defaultValue = 0) => (ushort)(this.GetOrDefault<ushort>(index) ^ defaultValue);
    public int ReadInt32(int index, int defaultValue = 0) => this.GetOrDefault<int>(index) ^ defaultValue;
    public uint ReadUInt32(int index, uint defaultValue = 0) => this.GetOrDefault<uint>(index) ^ defaultValue;
    public long ReadInt64(int index, long defaultValue = 0) => this.GetOrDefault<long>(index) ^ defaultValue;
    public ulong ReadUInt64(int index, ulong defaultValue = 0) => this.GetOrDefault<ulong>(index) ^ defaultValue;
    public float ReadFloat32(int index, float defaultValue = 0) => TypeHelpers.Xor(this.GetOrDefault<float>(index), defaultValue);
    public double ReadFloat64(int index, double defaultValue = 0) => TypeHelpers.Xor(this.GetOrDefault<double>(index), defaultValue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBool(int index, bool defaultValue = false)
    {
      // bool list elements can't be upgraded to struct
      if (_upgradedListElementByteOffset >= 0) return defaultValue;

      var wordIndex = index >> 6;
      if (wordIndex >= this.DataWords) return defaultValue;
      
      // TODO: byte-sized mask?
      var mask = 1UL << (index & 63);
      return (_segment[_structWordOffset + wordIndex | Word.unit] & mask) > 0 != defaultValue;
    }

#endregion Read methods

#region Write methods
    public void WriteRawPointer(int pointerIndex, Pointer pointer)
    {
      // TODO: disallow writing arbitrary capability pointers
      if (pointerIndex < 0 || pointerIndex >= this.PointerWords)
      {
        throw new ArgumentOutOfRangeException("pointerIndex", "Pointer index out of range");
      }

      this.Pointer(pointerIndex) = pointer;
    }

    public void WritePointer<T>(int pointerIndex, PrimitiveList<T> dest)
      where T : struct
    {
      this.WritePointerCore(
        pointerIndex,
        dest.Segment,
        dest.ListWordOffset,
        new ListPointer
        {
          Type = PointerType.List,
          ElementSize = TypeHelpers.ToElementSize<T>(),
          ElementCount = (uint)dest.Count,
        });
    }

    public void WritePointer(int pointerIndex, Text dest)
    {
      this.WritePointerCore(
        pointerIndex,
        dest.Segment,
        dest.ListWordOffset,
        new ListPointer
        {
          Type = PointerType.List,
          ElementSize = ElementSize.OneByte,
          ElementCount = (uint)dest.Count,
        });
    }

    public void WritePointer(int pointerIndex, BoolList dest)
    {
      this.WritePointerCore(
        pointerIndex,
        dest.Segment,
        dest.ListWordOffset,
        new ListPointer
        {
          Type = PointerType.List,
          ElementSize = ElementSize.OneBit,
          ElementCount = (uint)dest.Count,
        });
    }

    public void WritePointer<T>(int pointerIndex, CompositeList<T> dest) where T : struct, IStruct
    {
      this.WritePointerCore(
        pointerIndex,
        dest.Segment,
        dest.TagWordOffset,
        new ListPointer
        {
          Type = PointerType.List,
          ElementSize = ElementSize.Composite,
          ElementCount = (uint)dest.Count,
        });
    }
    
    public void WritePointer(int pointerIndex, Struct s)
    {
      this.WritePointerCore(
        pointerIndex,
        s.Segment,
        s.StructWordOffset,
        new StructPointer
        {
          Type = PointerType.Struct,
          DataWords = s.DataWords,
          PointerWords = s.PointerWords,
        });
    }
    
    public void WritePointer<T>(int pointerIndex, T dest) where T : struct, IStruct
    {
      var s = dest.Struct;
      this.WritePointerCore(
        pointerIndex,
        s.Segment,
        s.StructWordOffset,
        new StructPointer
        {
          Type = PointerType.Struct,
          DataWords = s.DataWords,
          PointerWords = s.PointerWords,
        });
    }

    public void WritePointer(int pointerIndex, ICapability cap)
    {
      Check.NotNull(cap, nameof(cap));
      
      var localCaps = this.Segment.Message.LocalCaps;
      uint? capId = null;
      for (uint i = 0; i < localCaps.Count; i++)
      {
        ref Message.CapEntry capEntry = ref localCaps[i];
        if (capEntry.Capability == cap)
        {
          capEntry.RefCount++;
          capId = i;
          break;
        }
      }
      
      if (capId == null)
      {
        ref Message.CapEntry capEntry = ref localCaps.Add(out var id);
        capEntry.Capability = cap;
        capEntry.RefCount = 1;
        capId = id;
      }

      var p = new Pointer()
      {
        Type = PointerType.Other,
        OtherPointerType = OtherPointerType.Capability,
        CapabilityId = capId.Value
      };

      _segment[_structWordOffset + this.DataWords + pointerIndex | Word.unit] = p.RawValue;
    }

    public void WritePointer(int pointerIndex, AbsPointer p)
    {
      this.WritePointerCore(
        pointerIndex,
        p.Segment,
        p.DataOffset,
        p.Tag);
    }

    private void WritePointerCore(int pointerIndex, Segment destSegment, int absOffset, Pointer tag)
    {
      if (pointerIndex < 0 || pointerIndex >= this.PointerWords)
      {
        throw new ArgumentOutOfRangeException("pointerIndex", "Pointer index out of range");
      }

      var msg = _segment.Message;
      if (destSegment.Message != msg)
      {
        throw new ArgumentException("Can't point to struct in different message");
      }

      Pointer resultPointer;
      if (this.Segment == destSegment)
      {
        int pointerWordOffset = this.StructWordOffset + this.DataWords + pointerIndex + 1;
        resultPointer = tag;
        resultPointer.WordOffset = absOffset - pointerWordOffset;
      }
      else
      {
        if (destSegment.TryAllocate(1, out int padOffset))
        {
          tag.WordOffset = absOffset - (padOffset + 1);
          destSegment[padOffset | Word.unit] = tag.RawValue;

          resultPointer = new FarPointer
          {
            Type = PointerType.Far,
            LandingPadOffset = (uint)padOffset,
            TargetSegmentId = (uint)destSegment.SegmentIndex,
          };
        }
        else
        {
          _segment.Message.Allocate(2, out padOffset, out Segment segment);
          segment[padOffset | Word.unit] = new FarPointer
          {
            Type = PointerType.Far,
            LandingPadOffset = (uint)absOffset,
            TargetSegmentId = (uint)destSegment.SegmentIndex
          }.RawValue;
          tag.WordOffset = 0;
          segment[padOffset + 1 | Word.unit] = tag.RawValue;

          resultPointer = new FarPointer
          {
            Type = PointerType.Far,
            IsDoubleFar = true,
            LandingPadOffset = (uint)padOffset,
            TargetSegmentId = (uint)segment.SegmentIndex,
          };
        }
      }
      
      _segment[_structWordOffset + this.DataWords + pointerIndex | Word.unit] = resultPointer.RawValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Write<T>(int index, T value)
      where T : struct
    {
      if (_upgradedListElementByteOffset >= 0)
      {
        if (index > 0) throw new InvalidOperationException("Cannot write to struct from a non-composite list");

        Unsafe.As<byte, T>(ref _segment[_structWordOffset * sizeof(ulong) + _upgradedListElementByteOffset | Byte.unit]) = value;
      }
      
      Unsafe.As<byte, T>(ref _segment[_structWordOffset * sizeof(ulong) + index * Unsafe.SizeOf<T>() | Byte.unit]) = value;
    }

    public void WriteInt8(int index, sbyte value, sbyte defaultValue = 0) => this.Write(index, value ^ defaultValue);
    public void WriteUInt8(int index, byte value, byte defaultValue = 0) => this.Write(index, value ^ defaultValue);
    public void WriteInt16(int index, short value, short defaultValue = 0) => this.Write(index, value ^ defaultValue);
    public void WriteUInt16(int index, ushort value, ushort defaultValue = 0) => this.Write(index, value ^ defaultValue);
    public void WriteInt32(int index, int value, int defaultValue = 0) => this.Write(index, value ^ defaultValue);
    public void WriteUInt32(int index, uint value, uint defaultValue = 0) => this.Write(index, value ^ defaultValue);
    public void WriteInt64(int index, long value, long defaultValue = 0) => this.Write(index, value ^ defaultValue);
    public void WriteUInt64(int index, ulong value, ulong defaultValue = 0) => this.Write(index, value ^ defaultValue);
    public void WriteFloat32(int index, float value, float defaultValue = 0) => this.Write(index, TypeHelpers.Xor(value, defaultValue));
    public void WriteFloat64(int index, double value, double defaultValue = 0) => this.Write(index, TypeHelpers.Xor(value, defaultValue));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBool(int index, bool value, bool defaultValue = false)
    {
      if (_upgradedListElementByteOffset >= 0 && index > 0)
      {
        throw new InvalidOperationException("Cannot write to struct from a non-composite list");
      }

      var wordIndex = index >> 6;
      if (wordIndex >= this.DataWords) throw new IndexOutOfRangeException();

      var mask = 1UL << (index & 63);
      if (value != defaultValue)
      {
        _segment[_structWordOffset + wordIndex | Word.unit] |= mask;
      }
      else
      {
        _segment[_structWordOffset + wordIndex | Word.unit] &= ~mask;
      }
    }

#endregion Write methods
  }
}