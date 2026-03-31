using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public static class BoundsHelper
    {
        /// <summary>
        /// For non alloc and faster using cached values - bounds must be with center == Vector3.zero (Local)
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="padding"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static BoundsPoints GetBoundsPoints(
            Bounds bounds,
            in Vector3 padding = default
            )
        {
            Guard.IsNotDefault(bounds);

            Vector3 c = bounds.center;
            Vector3 e = bounds.extents + padding;

            var corners = new Vector3[8];

            // 2. Углы (8 точек) — битовая маска
            for (int i = 0; i < 8; i++)
            {
                float x = (i & 1) != 0 ? e.x : -e.x;
                float y = (i & 2) != 0 ? e.y : -e.y;
                float z = (i & 4) != 0 ? e.z : -e.z;

                corners[i] = c + new Vector3(x, y, z);
            }

            // 3. Грани (6 точек) — одна ось на экстремуме, две в центре
            var faces = new Vector3[6]
            {
                c + new Vector3(-e.x,  0,    0),   // Left
                c + new Vector3( e.x,  0,    0),   // Right
                c + new Vector3( 0,   -e.y,  0),   // Bottom
                c + new Vector3( 0,    e.y,  0),   // Top
                c + new Vector3( 0,    0,   -e.z), // Back
                c + new Vector3( 0,    0,    e.z)  // Front
            };

            // 4. Ребра (12 точек) — две оси на экстремуме, одна в центре
            var edges = new Vector3[12];
            int idx = 0;

            // Ребра параллельные Z (X,Y фиксированы)
            for (int x = -1; x <= 1; x += 2)
                for (int y = -1; y <= 1; y += 2)
                    edges[idx++] = c + new Vector3(x * e.x, y * e.y, 0);

            // Ребра параллельные Y (X,Z фиксированы)
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                    edges[idx++] = c + new Vector3(x * e.x, 0, z * e.z);

            // Ребра параллельные X (Y,Z фиксированы)
            for (int y = -1; y <= 1; y += 2)
                for (int z = -1; z <= 1; z += 2)
                    edges[idx++] = c + new Vector3(0, y * e.y, z * e.z);

            var points = new BoundsPoints(bounds, corners, faces, edges);

            return points;
        }

        public static Bounds GetLocal(this Bounds source)
        {
            source.center = Vector3.zero;
            return source;
        }

        public static bool IsLocal(this Bounds source)
        {
            return source.center == Vector3.zero;
        }

        public static IReadOnlyList<Bounds> TransformBounds(in Bounds source, IReadOnlyList<Bounds> other)
        {
            if (source.center == Vector3.zero)
                return other;

            Bounds transformedItem;

            var results = new Bounds[other.Count];

            for (int i = 0; i < other.Count; i++)
            {
                transformedItem = other[i];
                transformedItem.center += source.center;
                results[i] = transformedItem;
            }

            return results;
        }

        public static void TransformBoundsNonAlloc(in Bounds source, Span<Bounds> other)
        {
            if (source.center == Vector3.zero)
                return;

            Bounds transformedItem;

            for (int i = 0; i < other.Length; i++)
            {
                transformedItem = other[i];
                transformedItem.center += source.center;
                other[i] = transformedItem;
            }
        }

        public static IReadOnlyList<Vector3> TransformPoints(Bounds source, IReadOnlyList<Vector3> other)
        {
            if (source.center == Vector3.zero)
                return other;

            var results = new Vector3[other.Count];

            for (int i = 0; i < other.Count; i++)
                results[i] = other[i] + source.center;

            return results;
        }

        public static void TransformPointsNonAlloc(in Bounds source, Span<Vector3> other)
        {
            if (source.center == Vector3.zero)
                return;

            for (int i = 0; i < other.Length; i++)
                other[i] += source.center;
        }

        public static IReadOnlyList<Bounds> InverseTransformBounds(in Bounds source, IReadOnlyList<Bounds> other)
        {
            if (source.center == Vector3.zero)
                return other;

            Bounds transformedItem;

            var results = new Bounds[other.Count];

            for (int i = 0; i < other.Count; i++)
            {
                transformedItem = other[i];
                transformedItem.center -= source.center;
                results[i] = transformedItem;
            }

            return results;
        }

        public static void InverseTransformBoundsNonAlloc(in Bounds source, Span<Bounds> other)
        {
            if (source.center == Vector3.zero)
                return;

            Bounds transformedItem;

            for (int i = 0; i < other.Length; i++)
            {
                transformedItem = other[i];
                transformedItem.center -= source.center;
                other[i] = transformedItem;
            }
        }

        public static IReadOnlyList<Vector3> InverseTransformPoints(Bounds source, IReadOnlyList<Vector3> other)
        {
            if (source.center == Vector3.zero)
                return other;

            var results = new Vector3[other.Count];

            for (int i = 0; i < other.Count; i++)
                results[i] = other[i] - source.center;

            return results;
        }

        public static void InverseTransformPointsNonAlloc(in Bounds source, Span<Vector3> other)
        {
            if (source.center == Vector3.zero)
                return;

            for (int i = 0; i < other.Length; i++)
                other[i] -= source.center;
        }
    }
}
