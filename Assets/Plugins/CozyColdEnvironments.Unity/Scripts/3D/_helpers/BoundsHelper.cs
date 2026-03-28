using CommunityToolkit.Diagnostics;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public static class BoundsHelper
    {
        public static BoundsPoints GetBoundsPoints(Bounds bounds, Vector3 offset = default)
        {
            Guard.IsNotDefault(bounds);

            Vector3 c = bounds.center + offset;
            Vector3 e = bounds.extents;

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
    }
}
