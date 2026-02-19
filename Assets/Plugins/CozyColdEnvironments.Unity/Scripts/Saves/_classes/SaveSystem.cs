using CCEnvs.Snapshots;
using CCEnvs.Unity.Components;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public class SaveSystem : CCBehaviourStatic<SaveSystem>
    {
        public static IReadOnlyDictionary<Type, Func<object, ISnapshot>> Converters => self.converters;

        private readonly Dictionary<Type, Func<object, ISnapshot>> converters = new();

        public static void RegisterType(Type type, Func<object, ISnapshot> converter)
        {
            Guard.IsNotNull(type, nameof(type));
            Guard.IsNotNull(converter, nameof(converter));

            self.converters.Add(type, converter);
        }
        public static void RegisterType<T>(Func<object, ISnapshot> converter)
        {
            RegisterType(typeof(T), converter);
        }

        public static bool UnregisterType(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            return self.converters.Remove(type);   
        }

        public static bool UnregisterType<T>()
        {
            return UnregisterType(typeof(T));
        }
    }
}
