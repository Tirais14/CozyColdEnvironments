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
        private readonly Dictionary<Type, Func<object, ISnapshot>> converters = new();

        public static Func<object, ISnapshot> GetSnapshotConverter(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            return self.converters[type];
        }
    }
}
