using UnityEngine;
using UTIRLib.Extensions;

#nullable enable
namespace UTIRLib.TwoD
{
    public static class TirLibTwoDHelper
    {
        public static Direction2D ToDirection(Vector2 vector2)
        {
            float x = vector2.normalized.x;
            float y = vector2.normalized.y;
            if (x.NotEqualsWithEpsilon(0) && y.NotEqualsWithEpsilon(0))
            {
                if (x > 0 && y > 0) { return Direction2D.RightUp; }
                else if (x < 0 && y > 0) { return Direction2D.LeftUp; }
                else if (x > 0 && y < 0) { return Direction2D.RightDown; }
                else if (x < 0 && y < 0) { return Direction2D.LeftDown; }
            }
            else if (x.NotEqualsWithEpsilon(0) && y.EqualsX(0))
            {
                if (x > 0) { return Direction2D.Right; }
                else if (x < 0) { return Direction2D.Left; }
            }
            else if (x.EqualsX(0) && y.NotEqualsWithEpsilon(0))
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
