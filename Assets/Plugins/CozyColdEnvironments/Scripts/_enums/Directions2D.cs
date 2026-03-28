using System;

namespace CCEnvs
{
    [Flags]
    public enum Directions2D
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
        public static Relative2DPosition GetRelativePosition(this Directions2D direction)
        {
            const Directions2D CENTER = Directions2D.Left | Directions2D.Right
                | Directions2D.Down | Directions2D.Up;

            const Directions2D TOP = Directions2D.Left | Directions2D.Right | Directions2D.Down;

            const Directions2D BOTTOM = Directions2D.Left | Directions2D.Right | Directions2D.Up;

            const Directions2D LEFT = Directions2D.Right | Directions2D.Down | Directions2D.Up;
            const Directions2D LEFT_TOP = Directions2D.Right | Directions2D.Down;
            const Directions2D LEFT_BOTTOM = Directions2D.Right | Directions2D.Up;

            const Directions2D RIGHT = Directions2D.Left | Directions2D.Down | Directions2D.Up;
            const Directions2D RIGHT_TOP = Directions2D.Left | Directions2D.Down;
            const Directions2D RIGHT_BOTTOM = Directions2D.Left | Directions2D.Up;

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

        public static bool IsDiagonal(this Directions2D direction) =>
            direction switch
            {
                Directions2D.LeftUp => true,
                Directions2D.LeftDown => true,
                Directions2D.RightUp => true,
                Directions2D.RightDown => true,
                _ => false,
            };
    }
}