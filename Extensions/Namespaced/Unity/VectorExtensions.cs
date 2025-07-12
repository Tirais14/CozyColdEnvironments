using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable

namespace UTIRLib.UExtensions
{
    [SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together", Justification = "<Pending>")]
    public static class VectorExtensions
    {
        #region Vector2
        #endregion Vector2

        #region Vector2Int
        #endregion Vector2Int

        #region Vector3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetDirectionRelative(this Vector3 position, Vector3 target)
        {
            return (target - position).normalized;
        }
        #endregion Vector3

        #region Vector3Int
        #endregion Vector3Int
    }
}