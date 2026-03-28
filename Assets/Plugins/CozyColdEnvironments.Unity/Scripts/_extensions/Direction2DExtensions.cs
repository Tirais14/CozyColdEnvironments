using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class Direction2DExtensions
    {
        public static Vector3Int ToVector3Int(this Directions2D direction) => direction switch
        {
            Directions2D.Up => Vector3Int.up,
            Directions2D.Down => Vector3Int.down,
            Directions2D.Right => Vector3Int.right,
            Directions2D.Left => Vector3Int.left,
            Directions2D.RightUp => Vector3Int.right + Vector3Int.up,
            Directions2D.LeftUp => Vector3Int.left + Vector3Int.up,
            Directions2D.RightDown => Vector3Int.right + Vector3Int.down,
            Directions2D.LeftDown => Vector3Int.left + Vector3Int.down,
            _ => Vector3Int.zero
        };

        public static Vector3 ToVector3(this Directions2D direction)
        {
            return ToVector3Int(direction);
        }

        public static Vector2 ToVector2(this Directions2D direction)
        {
            return ToVector3Int(direction).ToVector2();
        }

        public static Vector2Int ToVector2Int(this Directions2D direction)
        {
            return ToVector3Int(direction).ToVector2();
        }
    }
}
