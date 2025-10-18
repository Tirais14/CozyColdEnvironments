using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Dependencies
{
    public static class DiContainer
    {
        private static readonly Dictionary<(Type type, object id), object> bindings = new();

        public static void Bind(object obj, object id)
        {
            Type objType = obj.GetType();
            if (bindings.ContainsKey((objType, id)))
                throw new CCException($"Object {objType.GetFullName()} already binded.");

            bindings.Add((objType, id), obj);
        }

        public static object Resolve(Type type, object id)
        {
            if (!bindings.TryGetValue((type, id), out var result))
                throw new CCException($"Cannot find binding with key: {(type, id)}.");

            return result;
        }
        public static T Resolve<T>(object id) => Resolve(typeof(T), id).As<T>();
    }
}
