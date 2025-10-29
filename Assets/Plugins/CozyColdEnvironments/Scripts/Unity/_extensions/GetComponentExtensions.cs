using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class GetComponentExtensions
    {
        public static bool TryGetComponentInChildren(this Component value,
            Type type,
            bool includeInactive,
            [NotNullWhen(true)] out Component? result)
        {
            return value.gameObject.TryGetComponentInChildren(type,
                                                              includeInactive,
                                                              out result);
        }
        public static bool TryGetComponentInChildren(this Component value,
            Type type,
            [NotNullWhen(true)] out Component? result)
        {
            return value.TryGetComponentInChildren(type,
                                                   includeInactive: false,
                                                   out result);
        }

        public static bool TryGetComponentInChildren<T>(this Component value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            return value.gameObject.TryGetComponentInChildren(includeInactive, out result);
        }
        public static bool TryGetComponentInChildren<T>(this Component value,
            [NotNullWhen(true)] out T? result)
        {
            return value.TryGetComponentInChildren(includeInactive: false, out result);
        }

        public static bool TryGetComponentInParent(this Component value,
            Type type,
            bool includeInactive,
            [NotNullWhen(true)] out Component? result)
        {
            return value.gameObject.TryGetComponentInParent(type, includeInactive, out result);
        }
        public static bool TryGetComponentInParent(this Component value,
            Type type,
            [NotNullWhen(true)] out Component? result)
        {
            return value.TryGetComponentInParent(type, includeInactive: false, out result);
        }

        public static bool TryGetComponentInParent<T>(this Component value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            return value.gameObject.TryGetComponentInParent(includeInactive, out result);
        }
        public static bool TryGetComponentInParent<T>(this Component value,
            [NotNullWhen(true)] out T? result)
        {
            return value.TryGetComponentInParent(includeInactive: false, out result);
        }
    }
}
