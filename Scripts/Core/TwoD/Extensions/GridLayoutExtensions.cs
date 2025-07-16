using System.Runtime.CompilerServices;
using UnityEngine;
using UTIRLib.UExtensions;

#nullable enable

namespace UTIRLib.TwoD
{
    public static class GridLayoutExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int WorldToCell(this GridLayout gridLayout, Vector3 position)
        {
            return gridLayout.WorldToCell(position).ToVector2();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 CellToWorld(this GridLayout gridLayout, Vector2Int position)
        {
            return gridLayout.CellToWorld(position.ToVector3());
        }
    }
}