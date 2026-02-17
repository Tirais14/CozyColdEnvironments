using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.Dependencies
{
    public static class BuiltInDependecyContainer
    {
        [OnInstallResetable]
        private static readonly Dictionary<(Type type, object? id), object> bindings = new();

        public static void Bind(
            Type? contractType,
            object obj,
            object? id = null
            )
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            contractType ??= obj.GetType();

            if (HasBinding(contractType, id))
                throw new CCException($"Object with key: {(contractType, obj)} already binded.");

            bindings.Add((contractType, id), obj);
        }

        public static void Bind<TContract>(TContract obj, object? id = null)
        {
            Bind(typeof(TContract), obj!, id);
        }

        public static object Resolve(Type type, object? id = null)
        {
            Guard.IsNotNull(type, nameof(type));

            if (!bindings.TryGetValue((type, id), out var result))
                throw new CCException($"Cannot find binding with key: {(type, id)}.");

            return result;
        }
        public static T Resolve<T>(object? id = null)
        {
            return Resolve(typeof(T), id).To<T>();
        }

        public static object? TryResolve(
            Type type,
            object? id = null
            )
        {
            Guard.IsNotNull(type, nameof(type));

            if (!HasBinding(type, id))
                return null;

            return Resolve(type, id);
        }

        public static T? TryResolve<T>(object? id = null)
        {
            if (!HasBinding<T>(id))
                return default;

            return Resolve<T>(id)!;
        }

        public static bool Unbind(Type contractType, object? id = null)
        {
            Guard.IsNotNull(contractType, nameof(contractType));

            return bindings.Remove((contractType, id));
        }

        public static bool Unbind<TContract>(object? id = null)
        {
            return Unbind(typeof(TContract), id);
        }

        public static bool HasBinding(Type type, object? id = null)
        {
            return bindings.ContainsKey((type, id));
        }
        public static bool HasBinding<T>(object? id = null)
        {
            return HasBinding(typeof(T), id);
        }
    }
}
