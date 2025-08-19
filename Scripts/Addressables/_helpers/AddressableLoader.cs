using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.AddressableAssets
{
    public static class AddressableLoader
    {
        /// <exception cref="CollectionArgumentException"></exception>
        public static async UniTask<AsyncOperationHandle<IList<IResourceLocation>>>
            LoadResourceLocationsByTagsAsync(Type? assetType, params string[] tags)
        {
            if (tags.IsNullOrEmpty())
                throw new CollectionArgumentException(nameof(tags), tags);

            var locations = new List<IResourceLocation>(tags.Length);
            AsyncOperationHandle<IList<IResourceLocation>> handle;
            for (int i = 0; i < tags.Length; i++)
            {
                handle = Addressables.LoadResourceLocationsAsync(tags[i]);

                await handle;
                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    handle.Release();
                    continue;
                }

                if (assetType is not null)
                    locations.AddRange(handle.Result.Where(x => x.ResourceType.IsType(assetType)));
                else
                    locations.AddRange(handle.Result);
            }

            //distinct without equality comparer
            var locationsFilterd = locations.GroupBy(x => (x.PrimaryKey, x.ResourceType))
                                       .Select(x => x.First());

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
