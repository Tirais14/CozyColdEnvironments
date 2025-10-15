using CCEnvs.Reflection;
using CCEnvs.Unity.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity
{
    public static class ClassExtensions
    {
        public static T ThrowIfNotFound<T>(this T? obj)
            where T : Object
        {
            if (obj == null)
            {
                if (typeof(T).IsType<Component>())
                    throw new ComponentNotFoundException(typeof(T));
                else
                    throw new ObjectNotFoundException(typeof(T));
            }

            return obj;
        }
    }
}
