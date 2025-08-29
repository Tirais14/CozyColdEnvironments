using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using CCEnvs.AddressableAssets;
using CCEnvs.Diagnostics;
using CCEnvs.Initables;

#nullable enable
#pragma warning disable S2743
namespace CCEnvs.Unity.AddressableAssets.Databases
{
    public abstract class AddressablesDatabase<T> 
        :
        MonoCCInitableAsync

        where T : UnityEngine.Object
    {
        private AsyncOperationHandle<IList<T>> loadHandle;

        [SerializeField]
        protected string[] addressablesTags;

        protected override async UniTask OnInitAsync()
        {
            try
            {
                IList<T> assets = await LoadDBAsync();
                OnLoaded(assets);
            }
            catch (Exception ex)
            {
                CCEDebug.PrintException(ex);
            }
        }

        protected virtual void OnDestroy()
        {
            RealeseAddressableLoadHanders();
        }

        protected abstract void OnLoaded(IList<T> assets);

        protected void RealeseAddressableLoadHanders()
        {
            if (loadHandle.IsNotDefault())
                loadHandle.Release();
        }

        private async UniTask<IList<T>> LoadDBAsync()
        {
            try
            {
                if (addressablesTags.IsNullOrEmpty())
                    throw new CollectionException($"{nameof(addressablesTags)} cannot be null or empty.");

                loadHandle = await AddressableLoader.LoadAssetsByTagsAsync<T>(addressablesTags);
                return loadHandle.Result;
            }
            catch (Exception ex)
            {
                CCEDebug.PrintException(ex);
                return Array.Empty<T>();
            }
        }
    }
}
