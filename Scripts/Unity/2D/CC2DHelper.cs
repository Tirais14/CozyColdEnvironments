using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.TwoD
{
    public static class CC2DHelper
    {
        public static Direction2D ToDirection(Vector2 vector2)
        {
            float x = vector2.normalized.x;
            float y = vector2.normalized.y;
            if (x.NotNearlyEquals(0) && y.NotNearlyEquals(0))
            {
                if (x > 0 && y > 0) { return Direction2D.RightUp; }
                else if (x < 0 && y > 0) { return Direction2D.LeftUp; }
                else if (x > 0 && y < 0) { return Direction2D.RightDown; }
                else if (x < 0 && y < 0) { return Direction2D.LeftDown; }
            }
            else if (x.NotNearlyEquals(0) && y.NearlyEquals(0))
            {
                if (x > 0) { return Direction2D.Right; }
                else if (x < 0) { return Direction2D.Left; }
            }
            else if (x.NearlyEquals(0) && y.NotNearlyEquals(0))
            {
                if (y > 0) { return Direction2D.Up; }
                else if (y < 0) { return Direction2D.Down; }
            }

            return Direction2D.None;
        }

        public static Direction2D ToDirection(Vector2Int vector2Int) =>
            ToDirection(new Vector2(vector2Int.x, vector2Int.y));
    }
}
