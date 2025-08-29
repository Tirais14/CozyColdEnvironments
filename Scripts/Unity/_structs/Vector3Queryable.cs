#nullable enable
using UnityEngine;

namespace CCEnvs.Linq
{
    public ref struct Vector3Queryable
    {
        private Vector3 vector;

        public Vector3Queryable(Vector3 vector)
        {
            this.vector = vector;
        }

        public Vector3Queryable SetX(float value)
        {
            vector.x = value;

            return this;
        }

        public Vector3Queryable SetY(float value)
        {
            vector.y = value;

            return this;
        }

        public Vector3Queryable SetZ(float value)
        {
            vector.z = value;

            return this;
        }

        public Vector3Queryable AddX(float value)
        {
            vector.x += value;

            return this;
        }

        public Vector3Queryable AddY(float value)
        {
            vector.y += value;

            return this;
        }

        public Vector3Queryable AddZ(float value)
        {
            vector.z += value;

            return this;
        }

        public Vector3Queryable ReduceX(float value)
        {
            vector.x -= value;

            return this;
        }

        public Vector3Queryable ReduceY(float value)
        {
            vector.y -= value;

            return this;
        }

        public Vector3Queryable ReduceZ(float value)
        {
            vector.z -= value;

            return this;
        }

        public Vector3Queryable Normalize()
        {
            vector.Normalize();

            return this;
        }

        public Vector3Queryable Rotate(Quaternion quaternion)
        {
            vector = quaternion * vector;

            return this;
        }

        public Vector3Queryable Rotate(float x, float y, float z)
        {
            return Rotate(Quaternion.Euler(x, y, z));
        }

        public readonly Vector3 ToVector() => vector;

        public static implicit operator Vector3(Vector3Queryable value) => value.ToVector();
    }
}
