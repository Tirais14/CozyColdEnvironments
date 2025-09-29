using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using Cysharp.Threading.Tasks;
using LinqAF;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public static class AddressableLoader
    {
        /// <exception cref="CollectionArgumentException"></exception>
        public static async UniTask<AsyncOperationHandle<IList<IResourceLocation>>>LoadResourceLocationsByLabelsAsync(
            string[] labels,
            Type? assetType = null,
            IReadOnlyDictionary<Type, int>? loadPriorities = null)
        {
            if (labels.IsNullOrEmpty())
                throw new CollectionArgumentException(nameof(labels), labels);

            var handle = Addressables.LoadResourceLocationsAsync(labels,
                Addressables.MergeMode.Intersection);

            await handle;

            if (!IsValidOperation(out var failedHandle))
                return failedHandle;

            bool hasTypeFilter = assetType is not null;

            IEnumerable<IResourceLocation> locationsFilterd = FilterByType();
            ResolvePriorities();
            locationsFilterd = Distinct();

            bool hasPriorities = loadPriorities.IsNotNull() && loadPriorities.IsNotEmpty();

            locationsFilterd = OrderByPriorities();

            return Addressables.ResourceManager.CreateCompletedOperation(
                (IList<IResourceLocation>)locationsFilterd.ToList(),
                string.Empty);

            void ResolvePriorities()
            {
                Type[] resourceTypes = AddressablesHelper.GetResourceTypes(locationsFilterd);
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

            IEnumerable<IResourceLocation> FilterByType()
            {
                if (hasTypeFilter)
                {
                    return handle.Result.Where(
                        location => location.ResourceType.IsType(assetType))
                        .AsEnumerable();
                }

                return handle.Result;
            }

            IEnumerable<IResourceLocation> OrderByPriorities()
            {
                if (hasPriorities)
                {
                    return (from location in locationsFilterd
                            select (location, priority: loadPriorities!.ContainsKey(location.ResourceType)
                                                        ?
                                                        loadPriorities[location.ResourceType]
                                                        :
                                                        0) into prioritized
                            orderby prioritized.priority
                            select prioritized.location)
                            .AsEnumerable();

                }

                return locationsFilterd;
            }

            IEnumerable<IResourceLocation> Distinct()
            {
                return (from location in locationsFilterd
                        group location by location.PrimaryKey into g
                        select g.First() into location
                        select location)
                        .AsEnumerable();
            }
        }

        /// <exception cref="CollectionArgumentException"></exception>
        public static async UniTask<AsyncOperationHandle<IList<T>>> LoadAssetsByLabelsAsync<T>(
            string[] labels,
            IReadOnlyDictionary<Type, int>? loadPriorities = null,
            Action<T>? callback = null)
            where T : Object
        {
            if (labels.IsNullOrEmpty())
                throw new CollectionArgumentException(nameof(labels), labels);

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
