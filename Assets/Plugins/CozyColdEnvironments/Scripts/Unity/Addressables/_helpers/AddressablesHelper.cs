using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public static class AddressablesHelper
    {
        public static Type[] GetResourceTypes(IEnumerable<IResourceLocation> locations)
        {
            CC.Guard.IsNotNull(locations, nameof(locations));

            return locations.Select(x => x.ResourceType).ToArray();
        }

        public static int GetLoadPriority(Type fromType)
        {
            CC.Guard.IsNotNull(fromType, nameof(fromType));

            if (fromType.GetCustomAttribute<LoadPriorityAttribute>()
                is LoadPriorityAttribute attribute
                )
                return attribute.Priority;

            return default;
        }

        public static IReadOnlyDictionary<Type, int> GetLoadPriorites(params Type[] types)
        {
            CC.Guard.IsNotNull(types, nameof(types));
            if (types.IsEmpty())
                return ImmutableDictionary<Type, int>.Empty;

            var results = new Dictionary<Type, int>(types.Length);
            foreach (var type in types.Distinct())
                results.Add(type, GetLoadPriority(type));

            return results;
        }

        public static void ReleasePrefabComponentAsset<T>(T cmp)
        {
            CC.Guard.IsNotNull(cmp, nameof(cmp));
            Addressables.Release(cmp.To<Component>().gameObject);
        }
    }
}
