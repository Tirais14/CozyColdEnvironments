using UnityEngine;

namespace UTIRLib.Math
{
    public interface IMathShape2D
    {
        public Vector2 CenterPoint { get; }
        public float Square { get; }

        public void Set(params Vector2[] points);

        public void TranslateTo(Vector2 point);

        public float CalculateSquare();

        public bool IsPointIn(Vector2 point);
    }
}