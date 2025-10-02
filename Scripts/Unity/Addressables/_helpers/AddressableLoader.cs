using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public static class AddressableLoader
    {
        /// <exception cref="EmptyCollectionArgumentException"></exception>
        public static async UniTask<AsyncOperationHandle<IList<IResourceLocation>>>LoadResourceLocationsByLabelsAsync(
            string[] labels,
            Type? assetType = null,
            IReadOnlyDictionary<Type, int>? loadPriorities = null)
        {
            CC.Guard.CollectionArgument(labels, nameof(labels));

            var handle = Addressables.LoadResourceLocationsAsync(labels,
                Addressables.MergeMode.Intersection);

            await handle;

            if (!IsValidOperation(out var failedHandle))
                return failedHandle;

            bool hasTypeFilter = assetType is not null;

            ResolvePriorities();

            int count = handle.Result.Count;
            var locations = new List<IResourceLocation>();
            IResourceLocation location;
            for (int i = 0; i < count; i++)
            {
                location = handle.Result[i];

                if (hasTypeFilter
                    && 
                    location.ResourceType.IsNotType(assetType!)
                    )
                    continue;

                locations.Add(location);
            }

            locations.Sort((x, y) => loadPriorities![x.ResourceType].CompareTo(loadPriorities[y.ResourceType]));

            return Addressables.ResourceManager.CreateCompletedOperation(
                locations.As<IList<IResourceLocation>>(),
                string.Empty);

            void ResolvePriorities()
            {
                Type[] resourceTypes = AddressablesHelper.GetResourceTypes(handle.Result);
                var priorities = AddressablesHelper.GetLoadPriorites(resourceTypes);

                if (loadPriorities.IsNullOrEmpty())
                    loadPriorities = priorities;
                else
                    loadPriorities = new Dictionary<Type, int>(loadPriorities.Concat(priorities).AsEnumerable());
            }

            bool IsValidOperation(out AsyncOperationHandle<IList<IResourceLocation>> failed)
            {
                if (handle.Status != AsyncOperationStatus.Succeeded
                    ||
                    !handle.IsValid())
                {
                    handle.Release();
                    failed = Addressables.ResourceManager.CreateCompletedOperation(
                        (IList<IResourceLocation>)Array.Empty<IResourceLocation>(),
                        string.Empty);

                    return false;
                }

                failed = default;
                return true;
            }
        }

        /// <exception cref="EmptyCollectionArgumentException"></exception>
        public static async UniTask<AsyncOperationHandle<IList<T>>> LoadAssetsByLabelsAsync<T>(
            string[] labels,
            IReadOnlyDictionary<Type, int>? loadPriorities = null,
            Action<T>? callback = null)
            where T : Object
        {
            CC.Guard.CollectionArgument(labels, nameof(labels));

            var locationsHandle = await LoadResourceLocationsByLabelsAsync(labels, typeof(T), loadPriorities);

            IList<IResourceLocation> locations = await locationsHandle;

            if (!IsValidOperation(out var failedResourcesHandle))
                return failedResourcesHandle;

            return Addressables.LoadAssetsAsync(locations, callback);

            bool IsValidOperation(out AsyncOperationHandle<IList<T>> failed)
            {
                if (locationsHandle.Status != AsyncOperationStatus.Succeeded 
                    ||
                    !locationsHandle.IsValid())
                {
                    locationsHandle.Release();
                    failed = Addressables.ResourceManager.CreateCompletedOperation(
                        (IList<T>)Array.Empty<T>(),
                        string.Empty);

                    return false;
                }

                failed = default;
                return true;
            }
        }
    }
}
