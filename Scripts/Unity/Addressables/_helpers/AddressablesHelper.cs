using CCEnvs.Common;
using CCEnvs.Diagnostics;
using CCEnvs.Unity.AddrsAssets.Databases;
using LinqAF;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public static class AddressablesHelper
    {
        public static Type[] GetResourceTypes(IEnumerable<IResourceLocation> locations)
        {
            CC.Validate.ArgumentNull(locations, nameof(locations));

            return locations.Select(x => x.ResourceType).ToArray();
        }

        public static int GetLoadPriority(Type fromType)
        {
            CC.Validate.ArgumentNull(fromType, nameof(fromType));

            if (fromType.GetCustomAttribute<LoadPriorityAttribute>()
                is LoadPriorityAttribute attribute
                )
                return attribute.Priority;

            return default;
        }

        public static IReadOnlyDictionary<Type, int> GetLoadPriorites(params Type[] types)
        {
            CC.Validate.ArgumentNull(types, nameof(types));
            if (types.IsEmpty())
                return ImmutableDictionary<Type, int>.Empty;

            var results = new Dictionary<Type, int>(types.Length);
            foreach (var type in types.Distinct())
                results.Add(type, GetLoadPriority(type));

            return results;
        }

        public static T LoadAsset<T>(object key)
            where T : UnityEngine.Object
        {
            try
            {
                var task = Addressables.LoadAssetAsync<T>(key).Task;
                task.RunSynchronously();

                return task.Result;
            }
            catch (System.Exception ex)
            {
                CCDebug.PrintException(ex);
                throw;
            }
        }
    }
}
