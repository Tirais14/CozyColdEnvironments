#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.TypeMatching;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs.Json
{
    public static class JsonDtoCache
    {
        private readonly static HashSet<Type> bindings = new(0);
        private readonly static Dictionary<Type, object> objects = new(0);

        public static bool TryCache(Type deserializedType, object dto)
        {
            Validate.ArgumentNull(deserializedType, nameof(deserializedType));
            Validate.ArgumentNull(dto, nameof(dto));

            if (objects.ContainsKey(deserializedType))
                return false;

            objects.Add(deserializedType, dto);
            return true;
        }

        public static bool TryGetCached(Type deserializedType,
                                        [NotNullWhen(true)] out object? result)
        {
            Validate.ArgumentNull(deserializedType, nameof(deserializedType));

            return objects.TryGetValue(deserializedType, out result);
        }

        public static bool TryGetCached<T>(Type deserializedType,
                                        [NotNullWhen(true)] out T? result)
        {
            bool state = TryGetCached(deserializedType, out object? resultUntyped);

            return resultUntyped.Is(out result);
        }

        public static bool TryBind(Type dtoType)
        {
            Validate.ArgumentNull(dtoType, nameof(dtoType));

            if (bindings.Contains(dtoType))
                return false;

            bindings.Add(dtoType);
            return true;
        }

        public static bool Unbind(Type dtoType)
        {
            Validate.ArgumentNull(dtoType, nameof(dtoType));

            return bindings.Remove(dtoType);
        }

        public static bool IsBinded(Type dtoType)
        {
            Validate.ArgumentNull(dtoType, nameof(dtoType));

            return bindings.Contains(dtoType);
        }
    }
}
