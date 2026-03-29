using Generator.Equals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    [Equatable]
    public readonly partial struct BoundsPoints : IReadOnlyList<Vector3>
    {
        public const int CENTER_POINT_OFFSET = 1;

        public Bounds Source { get; }

        [ReferenceEquality]
        public IReadOnlyList<Vector3> Corners { get; }

        [ReferenceEquality]
        public IReadOnlyList<Vector3> Faces { get; }

        [ReferenceEquality]
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
    }
}
