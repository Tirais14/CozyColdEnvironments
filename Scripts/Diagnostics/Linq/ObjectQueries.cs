using UnityEngine;

#nullable enable

namespace UTIRLib.Diagnostics
{
    public static class ObjectQueries
    {
        public static T ThrowIfNotFound<T>(this T? obj)
        where T : Object
        {
            if (obj == null)
            {
                if (typeof(Component).IsAssignableFrom(typeof(T)))
                {
                    throw new ComponentNotFoundException(typeof(T));
                }
                else throw new ObjectNotFoundException(typeof(T));
            }

            return obj;
        }
    }
}