#nullable enable
#pragma warning disable S2743
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Concurrent;

namespace CCEnvs.Reflection.Caching
{
    public static class TypeCache
    {
        private static readonly ConcurrentDictionary<Type, TypeCacheInfo> entries = new();

        public static TypeCacheInfo Get(Type type)
        {
            Guard.IsNotNull(type);

            if (!entries.TryGetValue(type, out var entry))
            {
                entry = new TypeCacheInfo(type);
                entries.TryAdd(type, entry);
            }

            return entry;
        }

        public static bool IsUnityObject(Type type) => Get(type).IsUnityObject;

        public static bool IsUnityComponent(Type type) => Get(type).IsUnityComponent;

        public static bool IsUnityGameObject(Type type) => Get(type).IsUnityGameObject;

        public static bool IsCCBheaviour(Type type) => Get(type).IsCCBheaviour;

        public static bool IsValueType(Type type) => Get(type).IsValueType;

        public static bool IsPrimitive(Type type) => Get(type)  .IsPrimitive;

        public static string GetName(Type type) => Get(type).Name;

        public static string GetFullName(Type type) => Get(type).Name;

        public static string GetNamespace(Type type) => Get(type).Namespace; 
    }

    public static class TypeCache<T>
    {
        private static TypeCacheInfo core = new(TypeofCache<T>.Type);

        public static bool IsUnityObject => core.IsUnityObject;
        public static bool IsUnityComponent => core.IsUnityComponent;
        public static bool IsUnityGameObject => core.IsUnityGameObject;
        public static bool IsCCBheaviour => core.IsCCBheaviour;
        public static bool IsValueType => core.IsValueType;
        public static bool IsPrimitive => core.IsPrimitive;

        public static string Name => core.Name;
        public static string FullName => core.FullName;
        public static string Namespace => core.Namespace;
    }
}
