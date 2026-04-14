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

            GetVertexDistances(left, leftMeshVertexDistances.Value);
            GetVertexDistances(right, rightMeshVertexDistances.Value);

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

        public static void GetVertexDistances(Mesh mesh, IList<float> distances)
        {
            CC.Guard.IsNotNull(mesh, nameof(mesh));
            Guard.IsNotNull(distances);

            if (distances.IsReadOnly)
                throw CC.ThrowHelper.ReadOnlyCollection(distances);

            using var vertices = ListPool<Vector3>.Shared.Get();

            mesh.GetVertices(vertices.Value);

            Vector3 distance;

            for (int i = 0; i < vertices.Value.Count; i++)
            {
                if (i < vertices.Value.Count - 1)
                    distance = vertices.Value[i + 1] - vertices.Value[i];
                else
                    distance = Vector3.zero;

                distances.Add(distance.magnitude);
            }
        }
    }
}
