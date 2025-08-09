using UnityEngine;

namespace UTIRLib.Vectors.Linq
{
    public static class Vector3Queries
    {
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
