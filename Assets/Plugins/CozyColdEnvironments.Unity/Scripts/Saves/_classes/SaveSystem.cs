using CCEnvs.Snapshots;
using CCEnvs.Unity.Components;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public class SaveSystem : CCBehaviourStatic<SaveSystem>
    {
        public static IReadOnlyDictionary<Type, Func<object, ISnapshot>> Converters => self.converters;

        private readonly Dictionary<Type, Func<object, ISnapshot>> converters = new();
        private readonly Dictionary<(string groupName, string? groupID), SaveGroup> saveGroups = new();

        public static Func<object, ISnapshot> GetSnapshotConverter(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            return self.converters[type];
        }

        public static void RegisterObject(
            object obj,
            string? key = null,
            (string groupName, string? groupID) toSaveGroup = default
            )
        {

        }

        public static void RegisterType(Type type, Func<object, ISnapshot> converter)
        {
            Guard.IsNotNull(type, nameof(type));
            Guard.IsNotNull(converter, nameof(converter));

            self.converters.Add(type, converter);

            var defaultSaveGroup = ResolveDefaultSaveGroup(type);

            self.saveGroups.Add((defaultSaveGroup.Name, defaultSaveGroup.ID), defaultSaveGroup);
        }
        public static void RegisterType<T>(Func<object, ISnapshot> converter)
        {
            RegisterType(typeof(T), converter);
        }

        public static bool UnregisterType(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            var defaultSaveGroup = ResolveDefaultSaveGroup(type);

            self.saveGroups.Remove((defaultSaveGroup.Name, defaultSaveGroup.ID));

            return self.converters.Remove(type);   
        }

        public static bool UnregisterType<T>()
        {
            return UnregisterType(typeof(T));
        }

        private static SaveGroup ResolveDefaultSaveGroup(Type type)
        {
            if (type.GetCustomAttribute<SaveGroupAttribute>(inherit: false)
                .IsNull(out var saveGroupAttribute)
                )
            {
                return default;
            }

            return new SaveGroup(saveGroupAttribute.Name, saveGroupAttribute.ID);
        }
    }
}
