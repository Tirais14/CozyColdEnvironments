using CCEnvs.Patterns.Factories;
using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class MeshHelper
    {
        public static bool EqualsByVertexDistance(this Mesh left, Mesh right, float? epsilon = null)
        {
            if (left == right)
                return true;

            CC.Guard.IsNotNull(left, nameof(left));
            CC.Guard.IsNotNull(right, nameof(right));

            using var leftMeshVertexDistances = ListPool<float>.Shared.Get();
            using var rightMeshVertexDistances = ListPool<float>.Shared.Get();

            using var leftMeshVertices = ListPool<Vector3>.Shared.Get();
            using var rightMeshVertices = ListPool<Vector3>.Shared.Get();

            GetVertexDistances(
                left,
                leftMeshVertexDistances.Value,
                leftMeshVertices.Value
                );

            GetVertexDistances(
                right,
                rightMeshVertexDistances.Value,
                rightMeshVertices.Value
                );

            if (leftMeshVertexDistances.Value.Count != rightMeshVertexDistances.Value.Count
                ||
                leftMeshVertexDistances.Value.Count == 0
                ||
                rightMeshVertexDistances.Value.Count == 0)
            {
                return false;
            }

            float leftMeshVertexDistance;
            float rightMeshVertexDistance;

            for (int i = 0; i < leftMeshVertexDistances.Value.Count; i++)
            {
                leftMeshVertexDistance = leftMeshVertexDistances.Value[i];
                rightMeshVertexDistance = rightMeshVertexDistances.Value[i];

                if (leftMeshVertexDistance.NotNearlyEquals(rightMeshVertexDistance, epsilon))
                    return false;
            }

            return true;
        }

        public static void GetVertexDistances(
            Mesh mesh,
            List<float> distances,
            List<Vector3>? vertices
            )
        {
            CC.Guard.IsNotNull(mesh, nameof(mesh));
            Guard.IsNotNull(distances);
            Guard.IsNotNull(vertices);

            using var verticesHandle = ListPool<Vector3>.Shared.Get();

            if (vertices.Count == 0)
                mesh.GetVertices(vertices);

            Vector3 distance;

            for (int i = 0; i < vertices.Count; i++)
            {
                if (i < vertices.Count - 1)
                    distance = vertices[i + 1] - vertices[i];
                else
                    distance = Vector3.zero;

                distances.Add(distance.magnitude);
            }
        }
    }
}
