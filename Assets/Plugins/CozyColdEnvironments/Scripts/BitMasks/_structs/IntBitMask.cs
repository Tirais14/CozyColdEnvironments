#nullable enable
using System;
using System.Runtime.CompilerServices;

namespace CCEnvs
{
    public struct IntBitMask : IEquatable<IntBitMask>, IEquatable<int>
    {
        public int Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntBitMask(int value)
        {
            Value = value; 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator IntBitMask(int value)
        {
            return new IntBitMask(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(IntBitMask mask)
        {
            return mask.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlag(int flag) => Value &= ~flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetFlag(int flag) => Value |= flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ContainsFlag(int flag)
        {
            return Value == flag || (Value & (1 << flag)) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => Value = default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(IntBitMask other)
        {
            return Value == other.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(int other)
        {
            return Value == other;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override bool Equals(object obj)
        {
            return (obj is IntBitMask bitMask && Equals(bitMask))
                   ||
                   (obj is int number && Equals(number));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override int GetHashCode() => Value.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override string ToString() => Value.ToString();
    }

    public static class IntBitMaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntBitMask ToBitMask(this int source)
        {
            return new IntBitMask(source);
        }
    }
}
