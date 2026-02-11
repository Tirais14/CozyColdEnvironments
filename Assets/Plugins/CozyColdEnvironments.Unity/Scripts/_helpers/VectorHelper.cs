using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class VectorHelper
    {
        public static Vector2 ClampInRect(this Vector2 source, Rect rect)
        {
            var rectSize = rect.size;

            var rectLeftEdge = -rectSize.x / 2;
            var rectRightEdge = rectSize.x / 2;
            var rectBottomEdge = -rectSize.y / 2;
            var rectTopEdge = rectSize.y / 2;

            source.x = Mathf.Clamp(source.x, rectLeftEdge, rectRightEdge);
            source.y = Mathf.Clamp(source.y, rectBottomEdge, rectTopEdge);

            return source;
        }

        public static Vector3 ClampInRect(this Vector3 source, Rect rect)
        {
            var clamped2 = source.ToVector2().ClampInRect(rect);

            return clamped2.ToVector3(source.z);
        }

        public static Vector2 ResolveRectRelativePosition(this Vector2 source, Rect rect)
        {
            var rectSize = rect.size;

            var rectRightEdge = rectSize.x / 2;
            var rectTopEdge = rectSize.y / 2;

            return new Vector2(
                x: Mathf.Clamp01(source.x / rectRightEdge),
                y: Mathf.Clamp01(source.y / rectTopEdge)
                );
        }

        public static Vector3 ResolveRectRelativePosition(this Vector3 source, Rect rect)
        {
            var rectPos2 = source.ToVector2().ResolveRectRelativePosition(rect);

            return rectPos2.ToVector3(source.z);
        }

        public static Vector2 TransformRelativeRectPostition(this Vector2 source, Rect rect)
        {
            var rectSize = rect.size;

            var rectRightEdge = rectSize.x / 2;
            var rectTopEdge = rectSize.y / 2;

            return new Vector2(
                x: source.x * rectRightEdge,
                y: source.y * rectTopEdge
                );
        }

        public static Vector3 TransformRelativeRectPostition(this Vector3 source, Rect rect)
        {
            var rectPos2 = source.ToVector2().TransformRelativeRectPostition(rect);

            return rectPos2.ToVector3(source.z);
        }

        public static float SqrDistance(Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;

            return dx * dx + dy * dy + dz * dz;
        }

        public static long SqrDistance(Vector3Int a, Vector3Int b)
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

        public static long SqrDistance(Vector2Int a, Vector2Int b)
        {
            int dx = a.x - b.x;
            int dy = a.y - b.y;

            return dx * dx + dy * dy;
        }

        public static Vector3 GetDirection(Vector3 startPosition, Vector3 targetPosition)
        {
            return new Vector3(targetPosition.x - startPosition.x,
                               targetPosition.y - startPosition.y,
                               targetPosition.z - startPosition.z).normalized;
        }

        public static Vector2 GetDirection(Vector2 startPosition, Vector2 targetPosition)
        {
            return new Vector2(targetPosition.x - startPosition.x,
                               targetPosition.y - startPosition.y).normalized;
        }
    }
}
