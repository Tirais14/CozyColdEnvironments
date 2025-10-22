using CCEnvs.Reflection;
using CCEnvs.Unity.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity
{
    public static class ClassExtensions
    {
        public static T ValidateFindOperation<T>(this T? obj,
            GameObject? context = null)
            where T : Component
        {
            if (obj == null)
                throw new ComponentNotFoundException(typeof(T), context);

            return obj;
        }
        public static GameObject ValidateFindOperation(this GameObject? obj,
            object? key = null)
        {
            if (obj == null)
                throw new GameObjectNotFoundException(key);

            return obj;
        }
    }
}
