using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
#pragma warning disable S2094
namespace CozyColdEnvironments.Unity
{
    public static class Vector2Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int FloorToInt(this Vector2 vector)
        {
            return new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
        }

        public static Vector2Int RoundToInt(this Vector2 value)
        {
            return new Vector2Int(Mathf.RoundToInt(value.x), Mathf.RoundToInt(value.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector2 value, float z = 0f)
        {
            return new Vector3(value.x, value.y, z);
        }
    }

    public static class Vector2IntExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToVector3(this Vector2Int vector2Int, int z = 0)
        {
            return new Vector3Int(vector2Int.x, vector2Int.y, z);
        }
    }

    public static class Vector3Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int FloorToInt(this Vector3 vector)
        {
            return new Vector3Int(Mathf.FloorToInt(vector.x),
                                  Mathf.FloorToInt(vector.y),
                                  Mathf.FloorToInt(vector.z));
        }

        public static Vector3Int RoundToInt(this Vector3 vector)
        {
            return new Vector3Int(Mathf.RoundToInt(vector.x),
                                  Mathf.RoundToInt(vector.y),
                                  Mathf.RoundToInt(vector.z));
        }
    }

    public static class Vector3IntExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ToVector2(this Vector3Int vector3Int)
        {
            return new(vector3Int.x, vector3Int.y);
        }
    }
}

namespace CozyColdEnvironments.Unity.Extensions
{
    public static class Vector2Extensions
    {
        public static Direction2D ToDirection2D(this Vector2 value)
        {
            if (!value.x.NearlyEquals(0) && !value.y.NearlyEquals(0))
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
            else if (!value.x.NearlyEquals(0) && value.y.NearlyEquals(0))
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
            else if (!value.y.NearlyEquals(0) && value.x.NearlyEquals(0))
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
    }

    public static class Vector2IntExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction2D ToDirection2D(this Vector2Int value)
        {
            return new Vector2(value.x, value.y).ToDirection2D();
        }
    }

    public static class Vector3Extensions
    {
        public static Direction2D ToDirection2D(this Vector3 value)
        {
            return new Vector2(value.x, value.y).ToDirection2D();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetDirectionRelative(this Vector3 position, Vector3 target)
        {
            return (target - position).normalized;
        }
    }

    public static class Vector3IntExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction2D ToDirection2D(this Vector3Int value)
        {
            return new Vector2(value.x, value.y).ToDirection2D();
        }
    }
}
