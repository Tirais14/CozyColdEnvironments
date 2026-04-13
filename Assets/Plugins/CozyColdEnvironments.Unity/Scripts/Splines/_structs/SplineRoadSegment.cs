using CCEnvs.Reflection.Caching;
using System;
using Unity.Mathematics;

#nullable enable
namespace CCEnvs.Unity.Splines
{
    public readonly struct SplineRoadSegment : IEquatable<SplineRoadSegment>
    {
        public float3 LeftTopPoint { get; }
        public float3 LeftBottomPoint { get; }
        public float3 RightTopPoint { get; }
        public float3 RightBottomPoint { get; }

        public SplineRoadSegment(
            float3 leftTopPoint,
            float3 leftBottomPoint,
            float3 rightTopPoint,
            float3 rightBottomPoint
            )
        {
            LeftTopPoint = leftTopPoint;
            LeftBottomPoint = leftBottomPoint;
            RightTopPoint = rightTopPoint;
            RightBottomPoint = rightBottomPoint;
        }

        public static bool operator ==(SplineRoadSegment left, SplineRoadSegment right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SplineRoadSegment left, SplineRoadSegment right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is SplineRoadSegment segment && Equals(segment);
        }

        public bool Equals(SplineRoadSegment other)
        {
            return LeftTopPoint.Equals(other.LeftTopPoint)
                   &&
                   LeftBottomPoint.Equals(other.LeftBottomPoint)
                   &&
                   RightTopPoint.Equals(other.RightTopPoint)
                   &&
                   RightBottomPoint.Equals(other.RightBottomPoint);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                LeftTopPoint,
                LeftBottomPoint,
                RightTopPoint,
                RightBottomPoint
                );
        }

        public override string ToString()
        {
            if (this == default)
                return TypeCache<SplineRoadSegment>.FullName;

            return new ToStringBuilder()
                .Add(nameof(LeftTopPoint), LeftTopPoint)
                .Add(nameof(LeftBottomPoint), LeftBottomPoint)
                .Add(nameof(RightTopPoint), RightTopPoint)
                .Add(nameof(RightBottomPoint), RightBottomPoint)
                .ToStringAndDispose();
        }
    }
}
