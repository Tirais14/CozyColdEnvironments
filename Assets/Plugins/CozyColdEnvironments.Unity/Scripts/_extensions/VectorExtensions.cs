using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
#pragma warning disable S2094
namespace CCEnvs.Unity
{
    public static class Vector2Extensions
    {
        public static Direction2D ToDirection(this Vector2 source)
        {
            float x = source.normalized.x;
            float y = source.normalized.y;
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

        public static Vector2 WithX(this Vector2 source, float value)
        {
            source.x = value;
            return source;
        }

        public static Vector2 WithY(this Vector2 source, float value)
        {
            source.y = value;
            return source;
        }

        public static Vector2 AddX(this Vector2 source, float value)
        {
            source.x += value;
            return source;
        }

        public static Vector2 AddY(this Vector2 source, float value)
        {
            source.y += value;
            return source;
        }

        public static Vector2 MinusX(this Vector2 source, float value)
        {
            source.x -= value;
            return source;
        }

        public static Vector2 MinusY(this Vector2 source, float value)
        {
            source.y -= value;
            return source;
        }

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

        public static Direction2D ToDirection2D(this Vector2 value)
        {
            if (!value.x.NearlyEquals(0) && !value.y.NearlyEquals(0))
            {
                if (value.x > 0 && value.y > 0)
                    return Direction2D.RightUp;
                else if (value.x < 0 && value.y > 0)
                    return Direction2D.LeftUp;
                else if (value.x > 0 && value.y < 0)
                    return Direction2D.RightDown;
                else if (value.x < 0 && value.y < 0)
                    return Direction2D.LeftDown;
            }
            else if (!value.x.NearlyEquals(0) && value.y.NearlyEquals(0))
            {
                if (value.x > 0)
                    return Direction2D.Right;
                else if (value.x < 0)
                    return Direction2D.Left;
            }
            else if (!value.y.NearlyEquals(0) && value.x.NearlyEquals(0))
            {
                if (value.y > 0)
                    return Direction2D.Up;
                else if (value.y < 0)
                    return Direction2D.Down;
            }

            return Direction2D.None;
        }

        public static bool IsFinite(this Vector2 source)
        {
            return !float.IsNaN(source.x)
                   &&
                   !float.IsNaN(source.y);
        }

        public static bool NearlyEquals(this Vector2 source, Vector2 other, float? epsilon = null)
        {
            return source.x.NearlyEquals(other.x, epsilon)
                   &&
                   source.y.NearlyEquals(other.y, epsilon);
        }

        public static bool NotNearlyEquals(this Vector2 source, Vector2 other, float? epsilon = null)
        {
            return !source.NearlyEquals(other, epsilon);
        }
    }

    public static class Vector2IntExtensions
    {
        public static Vector2Int WithX(this Vector2Int source, int value)
        {
            source.x = value;
            return source;
        }

        public static Vector2Int WithY(this Vector2Int source, int value)
        {
            source.y = value;
            return source;
        }

        public static Vector2Int AddX(this Vector2Int source, int value)
        {
            source.x += value;
            return source;
        }

        public static Vector2Int AddY(this Vector2Int source, int value)
        {
            source.y += value;
            return source;
        }

        public static Vector2Int MinusX(this Vector2Int source, int value)
        {
            source.x -= value;
            return source;
        }

        public static Vector2Int MinusY(this Vector2Int source, int value)
        {
            source.y -= value;
            return source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToVector3(this Vector2Int vector2Int, int z = 0)
        {
            return new Vector3Int(vector2Int.x, vector2Int.y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction2D ToDirection2D(this Vector2Int value)
        {
            return new Vector2(value.x, value.y).ToDirection2D();
        }

        public static bool IsFinite(this Vector2Int source)
        {
            return !float.IsNaN(source.x)
                   &&
                   !float.IsNaN(source.y);
        }
    }

    public static class Vector3Extensions
    {
        public static Vector3 WithX(this Vector3 source, float value)
        {
            source.x = value;
            return source;
        }

        public static Vector3 WithY(this Vector3 source, float value)
        {
            source.y = value;
            return source;
        }

        public static Vector3 WithZ(this Vector3 source, float value)
        {
            source.z = value;
            return source;
        }

        public static Vector3 AddX(this Vector3 source, float value)
        {
            source.x += value;
            return source;
        }

        public static Vector3 AddY(this Vector3 source, float value)
        {
            source.y += value;
            return source;
        }

        public static Vector3 AddZ(this Vector3 source, float value)
        {
            source.z += value;
            return source;
        }

        public static Vector3 MinusX(this Vector3 source, float value)
        {
            source.x -= value;
            return source;
        }

        public static Vector3 MinusY(this Vector3 source, float value)
        {
            source.y -= value;
            return source;
        }

        public static Vector3 MinusZ(this Vector3 source, float value)
        {
            source.z -= value;
            return source;
        }

        public static Direction2D ToDirection2D(this Vector3 value)
        {
            return new Vector2(value.x, value.y).ToDirection2D();
        }

        public static Vector2 ToVector2(this Vector3 source)
        {
            return source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetDirectionRelative(this Vector3 position, Vector3 target)
        {
            return (target - position).normalized;
        }

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

        public static bool IsFinite(this Vector3 source)
        {
            return !float.IsNaN(source.x)
                   &&
                   !float.IsNaN(source.y)
                   &&
                   !float.IsNaN(source.z);
        }

        public static bool NearlyEquals(this Vector3 source, Vector3 other, float? epsilon = null)
        {
            return source.x.NearlyEquals(other.x, epsilon)
                   &&
                   source.y.NearlyEquals(other.y, epsilon)
                   &&
                   source.z.NearlyEquals(other.z, epsilon);
        }

        public static bool NotNearlyEquals(this Vector3 source, Vector3 other, float? epsilon = null)
        {
            return !source.NearlyEquals(other, epsilon);
        }
    }

    public static class Vector3IntExtensions
    {
        public static Vector3Int WithX(this Vector3Int source, int value)
        {
            source.x = value;
            return source;
        }

        public static Vector3Int WithY(this Vector3Int source, int value)
        {
            source.y = value;
            return source;
        }

        public static Vector3Int WithZ(this Vector3Int source, int value)
        {
            source.z = value;
            return source;
        }


        public static Vector3Int AddX(this Vector3Int source, int value)
        {
            source.x += value;
            return source;
        }

        public static Vector3Int AddY(this Vector3Int source, int value)
        {
            source.y += value;
            return source;
        }

        public static Vector3Int AddZ(this Vector3Int source, int value)
        {
            source.z += value;
            return source;
        }

        public static Vector3Int MinusX(this Vector3Int source, int value)
        {
            source.x -= value;
            return source;
        }

        public static Vector3Int MinusY(this Vector3Int source, int value)
        {
            source.y -= value;
            return source;
        }

        public static Vector3Int MinusZ(this Vector3Int source, int value)
        {
            source.z -= value;
            return source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction2D ToDirection2D(this Vector3Int value)
        {
            return new Vector2(value.x, value.y).ToDirection2D();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ToVector2(this Vector3Int vector3Int)
        {
            return new(vector3Int.x, vector3Int.y);
        }

        public static bool IsFinite(this Vector3Int source)
        {
            return !float.IsNaN(source.x)
                   &&
                   !float.IsNaN(source.y)
                   &&
                   !float.IsNaN(source.z);
        }
    }
}
