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
        public static bool EqualsByNormals(this Mesh leftMesh, Mesh rightMesh, float? epsilon = null)
        {
            if (leftMesh == rightMesh)
                return true;

            CC.Guard.IsNotNull(leftMesh, nameof(leftMesh));
            CC.Guard.IsNotNull(rightMesh, nameof(rightMesh));

            using var leftVertexNormals = PooledList<Vector3>.Create();
            using var rightVertexNormals = PooledList<Vector3>.Create();

            leftMesh.GetNormals(leftVertexNormals);
            rightMesh.GetNormals(rightVertexNormals);

            if (leftVertexNormals.Count != rightVertexNormals.Count)
                return false;

            Vector3 leftVertexNormal;
            Vector3 rightVertexNormal;

            for (int i = 0; i < leftVertexNormals.Count; i++)
            {
                leftVertexNormal = leftVertexNormals[i];
                rightVertexNormal = rightVertexNormals[i];

                if (leftVertexNormal.NotNearlyEquals(rightVertexNormal, epsilon))
                    return false;
            }

            return true;
        }

        public static bool EqualsByVertexDistance(this Mesh leftMesh, Mesh rightMesh, float? epsilon = null)
        {
            if (leftMesh == rightMesh)
                return true;

            CC.Guard.IsNotNull(leftMesh, nameof(leftMesh));
            CC.Guard.IsNotNull(rightMesh, nameof(rightMesh));

            using var leftMeshVertexDistances = PooledList<float>.Create();
            using var rightMeshVertexDistances = PooledList<float>.Create();

            using var leftMeshVertices = PooledList<Vector3>.Create();
            using var rightMeshVertices = PooledList<Vector3>.Create();

            GetVertexDistances(
                leftMesh,
                leftMeshVertexDistances,
                leftMeshVertices
                );

            GetVertexDistances(
                rightMesh,
                rightMeshVertexDistances,
                rightMeshVertices
                );

            if (leftMeshVertexDistances.Count != rightMeshVertexDistances.Count
                ||
                leftMeshVertexDistances.Count == 0
                ||
                rightMeshVertexDistances.Count == 0)
            {
                return false;
            }

            float leftMeshVertexDistance;
            float rightMeshVertexDistance;

            for (int i = 0; i < leftMeshVertexDistances.Count; i++)
            {
                leftMeshVertexDistance = leftMeshVertexDistances[i];
                rightMeshVertexDistance = rightMeshVertexDistances[i];

                if (leftMeshVertexDistance.NotNearlyEquals(rightMeshVertexDistance, epsilon))
                    return false;
            }

            return true;
        }

        public static bool EqualsByGeometry(this Mesh leftMesh, Mesh rightMesh, float? epsilon = null)
        {
            return leftMesh.EqualsByNormals(rightMesh, epsilon)
                   &&
                   leftMesh.EqualsByVertexDistance(rightMesh, epsilon);
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
