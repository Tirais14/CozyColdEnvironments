using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Dependencies
{
    public static class DependencyContainer
    {
        private static readonly Dictionary<(Type type, object? id), object> bindings = new();

        public static void Bind(object obj, object? id = null)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            Type objType = obj.GetType();
            if (bindings.ContainsKey((objType, id)))
                throw new CCException($"Object {objType.GetFullName()} already binded.");

            bindings.Add((objType, id), obj);
        }

        public static object Resolve(Type type, object? id = null)
        {
            if (!bindings.TryGetValue((type, id), out var result))
                throw new CCException($"Cannot find binding with key: {(type, id)}.");

            return result;
        }
        public static T Resolve<T>(object? id = null) => Resolve(typeof(T), id).As<T>();

        public static bool HasBinding(Type type, object? id = null)
        {
            return bindings.ContainsKey((type, id));
        }
        public static bool HasBinding<T>(object? id = null)
        {
            return HasBinding(typeof(T), id);
        }

#if UNITY_2017_1_OR_NEWER
        [UnityEditor.InitializeOnEnterPlayMode]
        private static void ClearOnPlayMode()
        {
            bindings.Clear();
        }
#endif
    }
}
