using CCEnvs.Reflection.Caching;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

#nullable enable
namespace CCEnvs.Unity.Splines
{
    public readonly struct SplineRoadSegment : IEquatable<SplineRoadSegment>, IEnumerable<float3>
    {
        public const int COUNT = 4;

        public float3 LeftTopPoint { get; }
        public float3 LeftBottomPoint { get; }
        public float3 RightTopPoint { get; }
        public float3 RightBottomPoint { get; }

        public readonly int Count => COUNT;

        public readonly float3 this[int idx] {
            get
            {
                return idx switch
                {
                    0 => LeftBottomPoint,
                    1 => LeftTopPoint,
                    2 => RightTopPoint,
                    3 => RightBottomPoint,
                    _ => throw CC.ThrowHelper.IndexOutOfRangeException(idx)
                };
            }
        }

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

            return new ToStringBuilder(null)
                .AddProperty(nameof(LeftTopPoint), LeftTopPoint)
                .AddProperty(nameof(LeftBottomPoint), LeftBottomPoint)
                .AddProperty(nameof(RightTopPoint), RightTopPoint)
                .AddProperty(nameof(RightBottomPoint), RightBottomPoint)
                .ToStringAndDispose();
        }

        public IEnumerator<float3> GetEnumerator() => new Enumeartor(this);
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumeartor : IEnumerator<float3>
        {
            private readonly SplineRoadSegment roadSegment;

            private int pointer;

            public float3 Current { get; private set; }

            readonly object IEnumerator.Current => Current;

            public Enumeartor(SplineRoadSegment roadSegment)
                :
                this()
            {
                this.roadSegment = roadSegment;

                pointer = -1;
            }

            public readonly void Dispose() { }

            public bool MoveNext()
            {
                if (++pointer >= roadSegment.Count)
                    return false;

                Current = pointer;
                return true;
            }

            public void Reset()
            {
                pointer = -1;
                Current = default;
            }
        }
    }
}
