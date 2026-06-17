#nullable enable
using System;

namespace CCEnvs
{
    public struct CombinedInt : IEquatable<CombinedInt>
    {
        public long Value;

        public int Part1 {
#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
            readonly get => (int)Value;
#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
            set => Value |= (long)value;
        }
        public int Part2 {
#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
            readonly get => (int)Value << 16;
#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
            set => Value |= (long)value >> 16;
        }

#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
        public CombinedInt(long value)
        {
            Value = value;
        }

#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
        public CombinedInt(int part1)
        {
            Value = default;
            Part1 = part1;
        }

#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
        public CombinedInt(int part1, int part2)
        {
            Value = default;
            Part1 = part1;
            Part2 = part2;
        }

#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
        public readonly void Deconstruct(out int part1, out int part2)
        {
            part1 = Part1;
            part2 = Part2;
        }

#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
        public static implicit operator long(CombinedInt instance)
        {
            return instance.Value;
        }

#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
        public static implicit operator CombinedInt(long value)
        {
            return new CombinedInt(value);
        }

        public static implicit operator CombinedInt((int Part1, int Part2) args)
        {
            return new CombinedInt(args.Part1, args.Part2);
        }

#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
        public static bool operator ==(CombinedInt left, CombinedInt right)
        {
            return left.Equals(right);
        }

#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
        public static bool operator !=(CombinedInt left, CombinedInt right)
        {
            return !(left == right);
        }

#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
        public CombinedInt SetPart1(int value)
        {
            Part1 = value;
            return this;
        }

#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
        public CombinedInt SetPart21(int value)
        {
            Part2 = value;
            return this;
        }

#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
        public CombinedInt Set(int part1, int part2)
        {
            Part1 = part1;
            Part2 = part2;
            return this;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is CombinedInt @int && Equals(@int);
        }

#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
        public readonly bool Equals(CombinedInt other)
        {
            return Value == other.Value;
        }

#if UNITY_BURST
        [Unity.Burst.BurstCompile]
#endif
        public readonly override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public readonly override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Part1), Part1)
                .AddProperty(nameof(Part2), Part2)
                .ToStringAndDispose();
        }
    }
}
