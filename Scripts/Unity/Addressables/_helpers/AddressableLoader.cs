using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public static class AddressableLoader
    {
        /// <exception cref="CollectionArgumentException"></exception>
        public static async UniTask<AsyncOperationHandle<IList<IResourceLocation>>>
            LoadResourceLocationsByTagsAsync(Type? assetType, params string[] tags)
        {
            if (tags.IsNullOrEmpty())
                throw new CollectionArgumentException(nameof(tags), tags);

            var handle = Addressables.LoadResourceLocationsAsync(tags,
                Addressables.MergeMode.Intersection);

            await handle;
            if (handle.Status != AsyncOperationStatus.Succeeded
                ||
                !handle.IsValid())
            {
                handle.Release();
                return Addressables.ResourceManager.CreateCompletedOperation(
                    (IList<IResourceLocation>)Array.Empty<IResourceLocation>(),
                    string.Empty);
            }

            //distinct without equality comparer
            var locationsFilterd = from location in handle.Result
                                   where location.ResourceType.IsType(assetType)
                                   group location by (location.PrimaryKey, location.ResourceType) into locGroups
                                   select locGroups.First() into location
                                   select location;

            return Addressables.ResourceManager.CreateCompletedOperation(
                (IList<IResourceLocation>)locationsFilterd.ToList(),
                string.Empty);
        }
        public static async UniTask<AsyncOperationHandle<IList<IResourceLocation>>>
            LoadResourceLocationsByTagsAsync(params string[] tags)
        {
            return await LoadResourceLocationsByTagsAsync(assetType: null, tags);
        }

        /// <exception cref="CollectionArgumentException"></exception>
        public static async UniTask<AsyncOperationHandle<IList<T>>> LoadAssetsByTagsAsync<T>(
            Action<T>? callback = null,
            params string[] tags
            )
            where T : UnityEngine.Object
        {
            if (tags.IsNullOrEmpty())
                throw new CollectionArgumentException(nameof(tags), tags);

            var locationsHandle = await LoadResourceLocationsByTagsAsync(typeof(T), tags);

            IList<IResourceLocation> locations = await locationsHandle;

            if (locationsHandle.Status != AsyncOperationStatus.Succeeded)
            {
                locationsHandle.Release();
                var emptyHandle = Addressables.ResourceManager.CreateCompletedOperation(
                    (IList<T>)Array.Empty<T>(),
                    string.Empty);

                return await UniTask.FromResult(emptyHandle);
            }

            var result = await Addressables.LoadAssetsAsync(locations,
                                                            callback);

            return Addressables.ResourceManager.CreateCompletedOperation(result,
                                                                         string.Empty);
        }
        public static async UniTask<AsyncOperationHandle<IList<T>>> LoadAssetsByTagsAsync<T>(
            params string[] tags
            )
            where T : UnityEngine.Object
        {
            return await LoadAssetsByTagsAsync<T>(callback: null, tags);
        }
    }
}
