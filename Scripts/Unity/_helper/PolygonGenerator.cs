using UnityEngine;

namespace UTIRLib.Utils
{
    public static class PolygonGenerator
    {
        public static Vector2[] GetFourVerticePolygonCoordinates(Vector2 position, float nearEdgeWidth, float farEdgeWidth,
            float range)
        {
            float halfNearEdgeWidth = nearEdgeWidth / 2;
            float halfFarEdgeWidth = farEdgeWidth / 2;
            return new Vector2[] {
                new(position.x - halfNearEdgeWidth, position.y),
                new(position.x + halfFarEdgeWidth, position.y),
                new(position.x + farEdgeWidth, position.y + range),
                new(position.x - farEdgeWidth, position.y + range)
            };
        }

        public static Vector2[] GetFourVerticePolygonCoordinates(float nearEdgeWidth, float farEdgeWidth,
            float range) => GetFourVerticePolygonCoordinates(Vector2.zero, nearEdgeWidth, farEdgeWidth, range);
    }
}