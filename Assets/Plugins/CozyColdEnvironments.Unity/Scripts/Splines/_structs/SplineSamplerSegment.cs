using CCEnvs.Reflection.Caching;
using System;
using Unity.Mathematics;

#nullable enable
namespace CCEnvs.Unity.Splines
{
    public readonly struct SplineSamplerSegment : IEquatable<SplineSamplerSegment>
    {
        public readonly float3 Position { get; }
        public readonly float3 Tangent { get; }
        public readonly float3 UpVector { get; }

        public SplineSamplerSegment(float3 position, float3 tangent, float3 upVector)
        {
            Position = position;
            Tangent = tangent;
            UpVector = upVector;
        }

        public static bool operator ==(SplineSamplerSegment left, SplineSamplerSegment right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SplineSamplerSegment left, SplineSamplerSegment right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is SplineSamplerSegment segment && Equals(segment);
        }

        public bool Equals(SplineSamplerSegment other)
        {
            return Position.Equals(other.Position)
                   &&
                   Tangent.Equals(other.Tangent)
                   &&
                   UpVector.Equals(other.UpVector);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position, Tangent, UpVector);
        }

        public override string ToString()
        {
            if (this == default)
                return TypeCache<SplineSamplerSegment>.FullName;

            return new ToStringBuilder(null)
                .Add(nameof(Position), Position)
                .Add(nameof(Tangent), Tangent)
                .Add(nameof(UpVector), UpVector)
                .ToStringAndDispose();
        }
    }
}
