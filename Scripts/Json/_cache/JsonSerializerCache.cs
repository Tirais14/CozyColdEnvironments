#nullable enable
using CCEnvs.TypeMatching;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs.Json
{
    public static class JsonSerializerCache
    {
        private readonly static HashSet<Type> bindings = new(0);
        private readonly static Dictionary<Type, object> objects = new(0);

        public static bool TryCache(Type deserializedType, object obj)
        {
            CC.Validate.ArgumentNull(deserializedType, nameof(deserializedType));
            CC.Validate.ArgumentNull(obj, nameof(obj));

            if (objects.ContainsKey(deserializedType))
                return false;

            objects.Add(deserializedType, obj);
            return true;
        }

        public static bool TryGetCached(Type deserializedType,
                                        [NotNullWhen(true)] out object? result)
        {
            CC.Validate.ArgumentNull(deserializedType, nameof(deserializedType));

            return objects.TryGetValue(deserializedType, out result);
        }

        public static bool TryGetCached<T>(Type deserializedType,
                                        [NotNullWhen(true)] out T? result)
        {
            TryGetCached(deserializedType, out object? resultUntyped);

            return resultUntyped.Is(out result);
        }

        public static bool TryBind(Type deserializedType)
        {
            CC.Validate.ArgumentNull(deserializedType, nameof(deserializedType));

            if (bindings.Contains(deserializedType))
                return false;

            bindings.Add(deserializedType);
            return true;
        }

        public static bool Unbind(Type deserializedType)
        {
            CC.Validate.ArgumentNull(deserializedType, nameof(deserializedType));

            return bindings.Remove(deserializedType);
        }

        public static bool IsBinded(Type deserializedType)
        {
            CC.Validate.ArgumentNull(deserializedType, nameof(deserializedType));

            return bindings.Contains(deserializedType);
        }
    }
}
