using System.Runtime.CompilerServices;
using UnityEngine;
using UTIRLib.Extensions;

#nullable enable

namespace UTIRLib.TwoD
{
    public static class VectorExtensions
    {
        #region Vector2

        public static Direction2D ToDirection2D(this Vector2 value)
        {
            if (!value.x.EqualsWithEpsilon(0) && !value.y.EqualsWithEpsilon(0))
            {
                if (value.x > 0 && value.y > 0)
                {
                    return Direction2D.RightUp;
                }
                else if (value.x < 0 && value.y > 0)
                {
                    return Direction2D.LeftUp;
                }
                else if (value.x > 0 && value.y < 0)
                {
                    return Direction2D.RightDown;
                }
                else if (value.x < 0 && value.y < 0)
                {
                    return Direction2D.LeftDown;
                }
            }
            else if (!value.x.EqualsWithEpsilon(0) && value.y.EqualsWithEpsilon(0))
            {
                if (value.x > 0)
                {
                    return Direction2D.Right;
                }
                else if (value.x < 0)
                {
                    return Direction2D.Left;
                }
            }
            else if (!value.y.EqualsWithEpsilon(0) && value.x.EqualsWithEpsilon(0))
            {
                if (value.y > 0)
                {
                    return Direction2D.Up;
                }
                else if (value.y < 0)
                {
                    return Direction2D.Down;
                }
            }

            return Direction2D.None;
        }

        #endregion Vector2

        #region Vector2Int

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction2D ToDirection2D(this Vector2Int value)
        {
            return new Vector2(value.x, value.y).ToDirection2D();
        }

        #endregion Vector2Int



        #region Vector3Int

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction2D ToDirection2D(this Vector3Int value)
        {
            return new Vector2(value.x, value.y).ToDirection2D();
        }

        #endregion Vector3Int
    }
}