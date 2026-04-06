using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public readonly struct BoundsPoints : IReadOnlyList<Vector3>, IEquatable<BoundsPoints>
    {
        public const int CENTER_POINT_OFFSET = 1;

        public Bounds Source { get; }

        public IReadOnlyList<Vector3> Corners { get; }
        public IReadOnlyList<Vector3> Faces { get; }
        public IReadOnlyList<Vector3> Edges { get; }

        public Vector3 this[int idx] {
            get
            {
                if (idx == 0)
                    return Source.center;

                idx--;

                if (idx < Corners.Count)
                    return Corners[idx];

                idx -= Corners.Count;

                if (idx < Faces.Count)
                    return Faces[idx];

                idx -= Faces.Count;

                if (idx < Edges.Count)
                    return Edges[idx];

                throw CC.ThrowHelper.IndexOutOfRangeException(idx);
            }
        }

        public static bool operator ==(BoundsPoints left, BoundsPoints right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BoundsPoints left, BoundsPoints right)
        {
            return !(left == right);
        }

        public int Count => Corners.Count + Faces.Count + Edges.Count + 1;

        public BoundsPoints(
            Bounds source,
            IReadOnlyList<Vector3> corners,
            IReadOnlyList<Vector3> faces,
            IReadOnlyList<Vector3> edges
            )
        {
            CC.Guard.IsNotNull(corners, nameof(corners));
            CC.Guard.IsNotNull(faces, nameof(faces));
            CC.Guard.IsNotNull(edges, nameof(edges));

            Source = source;
            Corners = corners;
            Faces = faces;
            Edges = edges;
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public readonly override bool Equals(object? obj)
        {
            return obj is BoundsPoints points && Equals(points);
        }

        public readonly bool Equals(BoundsPoints other)
        {
            return Source.Equals(other.Source) &&
                   EqualityComparer<IReadOnlyList<Vector3>>.Default.Equals(Corners, other.Corners) &&
                   EqualityComparer<IReadOnlyList<Vector3>>.Default.Equals(Faces, other.Faces) &&
                   EqualityComparer<IReadOnlyList<Vector3>>.Default.Equals(Edges, other.Edges) &&
                   Count == other.Count;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Source, Corners, Faces, Edges, Count);
        }
    }
}
