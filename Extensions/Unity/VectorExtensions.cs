using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
namespace UTIRLib
{
    public static class VectorExtensions
    {
        #region Vector2
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
        #endregion Vector2

        #region Vector2Int
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToVector3(this Vector2Int vector2Int, int z = 0)
        {
            return new Vector3Int(vector2Int.x, vector2Int.y, z);
        }
        #endregion Vector2Int

        #region Vector3
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

        #endregion Vector3

        #region Vector3Int
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ToVector2(this Vector3Int vector3Int)
        {
            return new(vector3Int.x, vector3Int.y);
        }
        #endregion Vector3Int
    }
}
