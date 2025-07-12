using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

#nullable enable

namespace UTIRLib.DB
{
    public class AssetDatabaseItemScene : AssetDatabaseItem<SceneInstance>
    {
        public AssetDatabaseItemScene(AddressableAssetInfo addressableAssetInfo)
            : base(addressableAssetInfo)
        { }

        /// <exception cref="System.NotImplementedException"></exception>
        public new Task<SceneInstance> LoadAssetAsync() =>
            throw new System.NotImplementedException($"Instead of {nameof(LoadAssetAsync)} use {nameof(LoadSceneAsync)}");

        /// <exception cref="System.Exception"></exception>
        public async Task<SceneInstance> LoadSceneAsync(LoadSceneInfo loadSceneInfo)
        {
            if (IsAssetLoaded)
            {
                Debug.LogWarning($"Scene {AssetName}(\"{Address}\") already loaded.");
                return asset;
            }

            asset = await Addressables.LoadSceneAsync(Address, loadSceneInfo.loadMode, loadSceneInfo.activateOnLoad,
                loadSceneInfo.priority, loadSceneInfo.releaseMode).Task;

            return asset;
        }
    }
}