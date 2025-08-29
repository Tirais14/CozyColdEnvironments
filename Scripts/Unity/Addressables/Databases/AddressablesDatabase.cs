using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using CozyColdEnvironments.AddressableAssets;
using CozyColdEnvironments.Diagnostics;
using CozyColdEnvironments.Initables;

#nullable enable
#pragma warning disable S2743
namespace CozyColdEnvironments.AddressableAssets.Databases
{
    public abstract class AddressablesDatabase<T> 
        :
        MonoXInitableAsync

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
                TirLibDebug.PrintException(ex);
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
                TirLibDebug.PrintException(ex);
                return Array.Empty<T>();
            }
        }
    }
}
