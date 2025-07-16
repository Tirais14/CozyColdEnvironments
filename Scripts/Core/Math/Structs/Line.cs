using UnityEngine;

#nullable enable

namespace UTIRLib.Math
{
    public struct Line
    {
        public Vector2 a;
        public Vector2 b;

        public Line(Vector2 a, Vector2 b)
        {
            this.a = a;
            this.b = b;
        }
    }
}