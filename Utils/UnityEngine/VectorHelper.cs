using UnityEngine;

namespace UTIRLib.Utils
{
    public static class VectorHelper
    {
        public static Vector3 GetDirection(Vector3 startPosition, Vector3 targetPosition)
        { return new Vector3(targetPosition.x - startPosition.x, targetPosition.y - startPosition.y, targetPosition.z - startPosition.z).normalized; }

        public static Vector2 GetDirection(Vector2 startPosition, Vector2 targetPosition)
        { return new Vector2(targetPosition.x - startPosition.x, targetPosition.y - startPosition.y).normalized; }

        public static float SqrDistance(Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;
            return dx * dx + dy * dy + dz * dz;
        }

        public static int SqrDistance(Vector3Int a, Vector3Int b)
        {
            int dx = a.x - b.x;
            int dy = a.y - b.y;
            int dz = a.z - b.z;
            return dx * dx + dy * dy + dz * dz;
        }

        public static long SqrDistanceLong(Vector3Int a, Vector3Int b)
        {
            int dx = a.x - b.x;
            int dy = a.y - b.y;
            int dz = a.z - b.z;
            return dx * dx + dy * dy + dz * dz;
        }

        public static float SqrDistance(Vector2 a, Vector2 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            return dx * dx + dy * dy;
        }

        public static int SqrDistance(Vector2Int a, Vector2Int b)
        {
            int dx = a.x - b.x;
            int dy = a.y - b.y;
            return dx * dx + dy * dy;
        }
    }
}