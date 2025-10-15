using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Unity.AddrsAssets.Databases;
using Cysharp.Threading.Tasks;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using ZLinq;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public static class AddressableLoader
    {
        /// <exception cref="EmptyCollectionArgumentException"></exception>
        public static async UniTask<AsyncOperationHandle<IList<IResourceLocation>>> LoadLocationsAsync(
            string[] labels,
            Addressables.MergeMode mergeMode = Addressables.MergeMode.Intersection,
            Type? assetType = null)
        {
            CC.Guard.CollectionArgument(labels, nameof(labels));

            var handle = Addressables.LoadResourceLocationsAsync(labels,
                mergeMode);

            await handle;

            if (!isValidOperation(out var failedHandle))
                return failedHandle;

            bool hasTypeFilter = assetType is not null;

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

            return Addressables.ResourceManager.CreateCompletedOperation(
                locations.As<IList<IResourceLocation>>(),
                string.Empty);

            bool isValidOperation(out AsyncOperationHandle<IList<IResourceLocation>> failed)
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

        public static async UniTask<AsyncOperationHandle<IList<T>>> LoadAssetsAsync<T>(
            AssetLabels labels,
            Action<T>? callback = null,
            Addressables.MergeMode mergeMode = Addressables.MergeMode.Intersection)
            where T : Object
        {
            CC.Guard.Argument(labels.IsDefault(), nameof(labels));

            var locationsHandle = await LoadLocationsAsync(labels, mergeMode, assetType: typeof(T));

            IList<IResourceLocation> locations = await locationsHandle;

            if (!isValidOperation(out var failedResourcesHandle))
                return failedResourcesHandle;

            return Addressables.LoadAssetsAsync(locations, callback);

            bool isValidOperation(out AsyncOperationHandle<IList<T>> failed)
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

        public static async UniTask<AsyncOperationHandle<IList<T>>> LoadAssetsPrioritizedAsync<T>(
            AssetLabels labels,
            Action<T>? callback = null,
            Addressables.MergeMode mergeMode = Addressables.MergeMode.Intersection)
            where T : Object
        {
            CC.Guard.Argument(labels.IsDefault(), nameof(labels));

            var locationsHandle = await LoadLocationsAsync(labels, mergeMode, assetType: typeof(T));

            var assetHandles =
                from loc in locationsHandle.Result.ZL()
                select new PrioritizedValue<IResourceLocation>(loc, Prioritized.ResolvePriority(loc.ResourceType, throwIfNotFound: false)) into prioritized
                orderby prioritized
                select Addressables.LoadAssetAsync<T>(prioritized.Value);

            var tasks = new List<UniTask<T>>(locationsHandle.Result.Count);
            foreach (var assetHandle in assetHandles)
            {
                if (callback is not null)
                    assetHandle.Completed += (x) => callback(x.Result);

                tasks.Add(assetHandle.ToUniTask());
            }

            await UniTask.WhenAll(tasks);

            var assets = assetHandles.Select(x => x.Result).ToArray();

            return Addressables.ResourceManager.CreateCompletedOperation(
                assets.As<IList<T>>(),
                string.Empty);
        }
    }
}
