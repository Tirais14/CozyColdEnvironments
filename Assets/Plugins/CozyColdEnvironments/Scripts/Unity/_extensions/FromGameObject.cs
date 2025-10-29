using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class FromGameObject
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetComponentInChildren(this GameObject value,
            Type type,
            bool includeInactive,
            [NotNullWhen(true)] out Component? result)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            result = value.GetComponentInChildren(type, includeInactive);

            return result != null;
        }

        public static bool TryGetComponentInChildren(this GameObject value,
            Type type,
            [NotNullWhen(true)] out Component? result)
        {
            return value.TryGetComponentInChildren(type,
                                                   includeInactive: false,
                                                   out result);
        }

        public static bool TryGetComponentInChildren<T>(this GameObject value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            result = value.GetComponentInChildren<T>(includeInactive);

            return result.IsNotNull();
        }

        public static bool TryGetComponentInChildren<T>(this GameObject value,
            [NotNullWhen(true)] out T? result)
        {
            return value.TryGetComponentInChildren(includeInactive: false, out result);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetComponentInParent(this GameObject value,
            Type type,
            bool includeInactive,
            [NotNullWhen(true)] out Component? result)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            result = value.GetComponentInParent(type, includeInactive);

            return result != null;
        }
        public static bool TryGetComponentInParent(this GameObject value,
            Type type,
            [NotNullWhen(true)] out Component? result)
        {
            return value.TryGetComponentInParent(type,
                                                 includeInactive: false,
                                                 out result);
        }

        public static bool TryGetComponentInParent<T>(this GameObject value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            result = value.GetComponentInParent<T>(includeInactive);

            return result.IsNotNull();
        }
        public static bool TryGetComponentInParent<T>(this GameObject value,
            [NotNullWhen(true)] out T? result)

        {
            return value.TryGetComponentInParent(includeInactive: false, out result);
        }
    }
}

