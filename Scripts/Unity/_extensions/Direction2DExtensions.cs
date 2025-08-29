using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class Direction2DExtensions
    {
        public static Vector3Int ToVector3Int(this Direction2D direction) => direction switch
        {
            Direction2D.Up => Vector3Int.up,
            Direction2D.Down => Vector3Int.down,
            Direction2D.Right => Vector3Int.right,
            Direction2D.Left => Vector3Int.left,
            Direction2D.RightUp => Vector3Int.right + Vector3Int.up,
            Direction2D.LeftUp => Vector3Int.left + Vector3Int.up,
            Direction2D.RightDown => Vector3Int.right + Vector3Int.down,
            Direction2D.LeftDown => Vector3Int.left + Vector3Int.down,
            _ => Vector3Int.zero
        };

        public static Vector3 ToVector3(this Direction2D direction)
        {
            return ToVector3Int(direction);
        }

        public static Vector2 ToVector2(this Direction2D direction)
        {
            return ToVector3Int(direction).ToVector2();
        }

        public static Vector2Int ToVector2Int(this Direction2D direction)
        {
            return ToVector3Int(direction).ToVector2();
        }
    }
}
