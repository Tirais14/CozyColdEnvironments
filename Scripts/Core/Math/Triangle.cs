using UnityEngine;

namespace UTIRLib.Math
{
    public sealed class Triangle
    {
        private readonly bool autoCalculating;
        private float square;

        public Vector2 a;
        public Vector2 b;
        public Vector2 c;

        public float Square => square;

        /// <summary>
        ///
        /// </summary>
        /// <param name="autoCalculating">If <see langword="true"/> when call method <see cref="Set"/> automatic calcluates square.</param>
        public Triangle(bool autoCalculating = true) => this.autoCalculating = autoCalculating;

        public Triangle(Vector2 a, Vector2 b, Vector2 c, bool autoCalculating = true) : this(autoCalculating)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            CalculateSquare();
        }

        public void Set(Vector2 a, Vector2 b, Vector2 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            if (autoCalculating)
            {
                CalculateSquare();
            }
        }

        public void Set(Triangle triangle) => Set(triangle.a, triangle.b, triangle.c);

        public float CalculateSquare()
        {
            square = MathShape2DHelper.CalculateSquareOfTriangle(a, b, c);
            return square;
        }

        public bool IsInTriangle(Vector2 point) => MathShape2DHelper.IsPointInTriangle(point, this);

        public bool IsInTriangle(float x, float y) => IsInTriangle(new Vector2(x, y));
    }
}