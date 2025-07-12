using System.Collections.Generic;
using UnityEngine;

namespace UTIRLib.Math
{
    public sealed class Quadrilateral : MathShape2D, IMathShape2D
    {
        private Vector2 a;
        private Vector2 b;
        private Vector2 c;
        private Vector2 d;

        public Vector2 A {
            get => a;
            set {
                a = value;
                if (autoCalculating)
                {
                    Recalculate();
                }
            }
        }

        public Vector2 B {
            get => b;
            set {
                b = value;
                if (autoCalculating)
                {
                    Recalculate();
                }
            }
        }

        public Vector2 C {
            get => c;
            set {
                c = value;
                if (autoCalculating)
                {
                    Recalculate();
                }
            }
        }

        public Vector2 D {
            get => d;
            set {
                d = value;
                if (autoCalculating)
                {
                    Recalculate();
                }
            }
        }

        public Quadrilateral(bool autoCalculating = true) : base(autoCalculating)
        {
        }

        public Quadrilateral(Vector2 a, Vector2 b, Vector2 c, Vector2 d, bool autoCalculating = true) : this(autoCalculating)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            Recalculate();
        }

        public void Set(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            if (autoCalculating)
            {
                Recalculate();
            }
        }

        public override void TranslateTo(Vector2 point)
        {
            Vector2 offsetVector = MathShape2DHelper.TranslateTo(ref centerPoint, point);
            a += offsetVector;
            b += offsetVector;
            c += offsetVector;
            d += offsetVector;
        }

        public override float CalculateSquare()
        {
            square = MathShape2DHelper.CalculateSquareOfQuadrilateral(a, b, c, d);
            return square;
        }

        public override bool IsPointIn(Vector2 point) => MathShape2DHelper.IsPointInQuadrilateral(point, a, b, c, d);

        public override IEnumerator<Vector2> GetEnumerator()
        {
            yield return a;
            yield return b;
            yield return c;
            yield return d;
        }

        protected override void CalculateCenterPoint() => centerPoint = MathShape2DHelper.CalculateCenterPoint(a, b, c, d);

        public override void Set(params Vector2[] points) => throw new System.NotImplementedException();
    }
}