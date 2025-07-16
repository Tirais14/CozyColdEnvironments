using System;
using UnityEngine;
using UTIRLib.Extensions;
using UTIRLib.UExtensions;

namespace UTIRLib.TwoD
{
    [Flags]
    public enum Direction2D
    {
        None,
        Down,
        Left = 2,
        Right = 4,
        Up = 8,
        LeftDown = Left | Down,
        LeftUp = Left | Up,
        RightDown = Right | Down,
        RightUp = Right | Up
    }

    public static class Direction2DExtensions
    {
        public static Relative2DPosition GetRelativePosition(this Direction2D direction)
        {
            const Direction2D CENTER = Direction2D.Left | Direction2D.Right
                | Direction2D.Down | Direction2D.Up;

            const Direction2D TOP = Direction2D.Left | Direction2D.Right | Direction2D.Down;

            const Direction2D BOTTOM = Direction2D.Left | Direction2D.Right | Direction2D.Up;

            const Direction2D LEFT =  Direction2D.Right | Direction2D.Down | Direction2D.Up;
            const Direction2D LEFT_TOP =  Direction2D.Right | Direction2D.Down;
            const Direction2D LEFT_BOTTOM =  Direction2D.Right | Direction2D.Up;

            const Direction2D RIGHT = Direction2D.Left | Direction2D.Down | Direction2D.Up;
            const Direction2D RIGHT_TOP = Direction2D.Left | Direction2D.Down;
            const Direction2D RIGHT_BOTTOM = Direction2D.Left | Direction2D.Up;

            Relative2DPosition position = Relative2DPosition.None;
            if (direction == CENTER)
                position = Relative2DPosition.Center;
            else if (direction == TOP)
                position = Relative2DPosition.Top;
            else if (direction == BOTTOM)
                position = Relative2DPosition.Bottom;
            else if (direction == LEFT)
                position = Relative2DPosition.Left;
            else if (direction == LEFT_TOP)
                position = Relative2DPosition.LeftTop;
            else if (direction == LEFT_BOTTOM)
                position = Relative2DPosition.LeftBottom;
            else if (direction == RIGHT)
                position = Relative2DPosition.Right;
            else if (direction == RIGHT_TOP)
                position = Relative2DPosition.RightTop;
            else if (direction == RIGHT_BOTTOM)
                position = Relative2DPosition.RightBottom;

            return position;
        }

        public static Vector3Int ToVector3Int(this Direction2D direction) => direction switch {
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

        public static bool IsDiagonal(this Direction2D direction) =>
            direction switch {
                Direction2D.LeftUp => true,
                Direction2D.LeftDown => true,
                Direction2D.RightUp => true,
                Direction2D.RightDown => true,
                _ => false,
            };
    }
}