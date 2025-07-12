using UnityEngine;
using UTIRLib.Extensions;

namespace UTIRLib.Math
{
#nullable enable

    public static class MathShape2DHelper
    {
        private static readonly (Vector2 a, Vector2 b, Vector2 c)[] triangles = new (Vector2 a, Vector2 b, Vector2 c)[2];

        /// <returns>Offset vector</returns>
        public static Vector2 TranslateTo(ref Vector2 pointPosition, Vector2 newPointPosition)
        {
            Vector2 offsetVector = newPointPosition - pointPosition;
            pointPosition = newPointPosition;
            return offsetVector;
        }

        public static Vector2 CalculateCenterPoint(params Vector2[] points)
        {
            float averageX = 0;
            float averageY = 0;
            for (int i = 0; i < points.Length; i++)
            {
                averageX += points[i].x;
                averageY += points[i].y;
            }

            averageX /= points.Length;
            averageY /= points.Length;

            return new Vector2(averageX, averageY);
        }

        public static float CalculateSquareOfTriangle(Vector2 a, Vector2 b, Vector2 c)
        {
            float square = a.x * (b.y - c.y) +
                b.x * (c.y - a.y) +
                c.x * (a.y - b.y);
            square /= 2;
            return Mathf.Abs(square);
        }

        public static float CalculateSquareOfTriangle(Triangle triangle) =>
            CalculateSquareOfTriangle(triangle.a, triangle.b, triangle.c);

        public static float CalculateSquareOfQuadrilateral(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            (Vector2 a, Vector2 b, Vector2 c)[] triangles = SplitQuadrilateralInTrianglesByDiagonal(a, b, c, d);
            (Vector2 a, Vector2 b, Vector2 c) triangle;
            float square = 0;
            for (int i = 0; i < triangles.Length; i++)
            {
                triangle = triangles[i];
                square += CalculateSquareOfTriangle(triangle.a, triangle.b, triangle.c);
            }

            return square;
        }

        public static float CalculateSquareOfQuadrilateral(Quadrilateral quadrilateral) =>
            CalculateSquareOfQuadrilateral(quadrilateral.A, quadrilateral.B, quadrilateral.C, quadrilateral.D);

        public static bool IsPointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
        {
            float triangleSquare = CalculateSquareOfTriangle(a, b, c);

            float firstSubTriangleSquare = CalculateSquareOfTriangle(a, point, b);
            float secondSubTriangleSquare = CalculateSquareOfTriangle(b, point, c);
            float thirdSubTriangleSquare = CalculateSquareOfTriangle(c, point, a);

            return (firstSubTriangleSquare + secondSubTriangleSquare + thirdSubTriangleSquare).
                EqualsWithEpsilon(triangleSquare);
        }

        public static bool IsPointInTriangle(Vector2 point, Triangle triangle) =>
            IsPointInTriangle(point, triangle.a, triangle.b, triangle.c);

        public static bool IsPointInQuadrilateral(Vector2 point, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            (Vector2 a, Vector2 b, Vector2 c)[] triangles = SplitQuadrilateralInTrianglesByDiagonal(a, b, c, d);
            (Vector2 a, Vector2 b, Vector2 c) triangle;
            for (int i = 0; i < triangles.Length; i++)
            {
                triangle = triangles[i];
                if (IsPointInTriangle(point, triangle.a, triangle.b, triangle.c))
                {
                    return true;
                }
            }

            return false;
        }

        private static (Vector2, Vector2, Vector2)[] SplitQuadrilateralInTrianglesByDiagonal(Vector2 a, Vector2 b,
            Vector2 c, Vector2 d)
        {
            triangles[0] = (a, b, c);
            triangles[1] = (b, c, d);
            return triangles;
        }
    }
}