using UnityEngine;

namespace CozyColdEnvironments.Vectors.Linq
{
    public static class Vector3Queries
    {
        //public static ref Vector3 SetX(this ref Vector3 value, float x)
        //{
        //    value.x = x;

        //    return ref value;
        //}

        //public static ref Vector3 SetY(this ref Vector3 value, float y)
        //{
        //    value.y = y;

        //    return ref value;
        //}

        //public static ref Vector3 SetZ(this ref Vector3 value, float z)
        //{
        //    value.z = z;

        //    return ref value;
        //}

        //public static ref Vector3 AddX(this ref Vector3 value, float x)
        //{
        //    value.x += x;

        //    return ref value;
        //}

        //public static ref Vector3 AddY(this ref Vector3 value, float y)
        //{
        //    value.y += y;

        //    return ref value;
        //}

        //public static ref Vector3 AddZ(this ref Vector3 value, float z)
        //{
        //    value.z += z;

        //    return ref value;
        //}

        //public static ref Vector3 ReduceX(this ref Vector3 value, float x)
        //{
        //    value.x -= x;

        //    return ref value;
        //}

        //public static ref Vector3 ReduceY(this ref Vector3 value, float y)
        //{
        //    value.y -= y;

        //    return ref value;
        //}

        //public static ref Vector3 ReduceZ(this ref Vector3 value, float z)
        //{
        //    value.z -= z;

        //    return ref value;
        //}

        public static Vector3Queryable ToQueryable(this Vector3 value)
        {
            return new Vector3Queryable(value);
        }

        public static Vector3Queryable Q(this Vector3 value)
        {
            return new Vector3Queryable(value);
        }
    }
}
