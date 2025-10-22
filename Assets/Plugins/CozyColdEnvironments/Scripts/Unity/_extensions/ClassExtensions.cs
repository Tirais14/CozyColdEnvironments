using CCEnvs.Diagnostics;
using CCEnvs.Unity.Diagnostics;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class ClassExtensions
    {
        public static T ValidateGetOperation<T>(this T? obj,
            GameObject? context = null)
            where T : class
        {
            if (obj.IsNull())
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
