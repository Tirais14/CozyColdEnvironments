#nullable enable
using System;
using System.Runtime.CompilerServices;

#pragma warning disable S2743
namespace CCEnvs.Reflection.Caching
{
    public static class TypeCache<T>
    {
        private static string? _name;

        private static string? _fullName;

        private static string? _namespace;

        public static bool IsUnityObject { get; }

        public static bool IsUnityComponent { get; }

        public static bool IsUnityGameObject { get; }

        public static bool IsCCBheaviour { get; }

        public static string Name {
            get
            {
                _name ??= TypeofCache<T>.Type.Name;

                return _name;
            }
        }

        public static string FullName {
            get
            {
                _fullName ??= TypeofCache<T>.Type.FullName;

                return _fullName;
            }
        }

        public static string Namespace {
            get
            {
                _namespace ??= TypeofCache<T>.Type.Namespace;

                return _namespace;
            }
        }

        static TypeCache()
        {
#if UNITY_2017_1_OR_NEWER

            Type type = TypeofCache<T>.Type;

            if (type.IsType<UnityEngine.Object>())
            {
                IsUnityObject = true;
                IsUnityComponent = IsUnityComponentType(type);
                IsUnityGameObject = IsUnityGameObjectType(type);

                if (IsUnityComponent)
                {
                    while (type is not null)
                    {
                        if (type.Namespace is null
                            ||
                            type.Name is null)
                        {
                            continue;
                        }

                        if (!IsCCBheaviour)
                        {
                            IsCCBheaviour = IsCCBehaviourType(type);
                            break;
                        }

                        type = type.BaseType;
                    }
                }
            }

#endif
        }

#if UNITY_2017_1_OR_NEWER

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsUnityComponentType(Type type)
        {
            return !IsUnityGameObject && type.IsType<UnityEngine.Component>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsUnityGameObjectType(Type type)
        {
            return !IsUnityComponent && type.IsType<UnityEngine.GameObject>();
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsCCBehaviourType(Type type)
        {
            return type.Namespace.ContainsOrdinal(NamepsaceHelper.NAMESPACE_CCENVS_UNITY_COMPONENTS)
                   &&
                   type.Name.Contains("CCBehaviour");
        }
    }
}
