using Newtonsoft.Json;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;
using Object = UnityEngine.Object;

#nullable enable
#pragma warning disable S101
namespace UTIRLib.Json.DTO
{
    [Serializable]
    public record AddressableDTO : IJsonDTO
    {
        [JsonProperty("assetPath")]
        public string AssetPath { get; set; } = string.Empty;
        [JsonProperty("guid")]
        public string GUID { get; set; } = string.Empty;
        [JsonProperty("assetType")]
        public Type AssetType { get; set; } = null!;

        [JsonIgnore]
        public AsyncOperationHandle LoadHandle { get; protected set; }
        [JsonIgnore]
        public Object Asset {
            get
            {
                if (!IsAssetLoaded)
                    throw new InvalidOperationException($"Asset not loaded.");

                return (Object)LoadHandle.Result;
            }
        }
        [JsonIgnore]
        public bool IsAssetLoaded => LoadHandle.IsNotDefault() && LoadHandle.IsDone;
        [JsonIgnore]
        public bool HasAssetPath => AssetPath.IsNotNullOrEmpty();
        [JsonIgnore]
        public bool HasGUID => GUID.IsNotNullOrEmpty();
        [JsonIgnore]
        public bool HasKey => HasAssetPath || HasGUID;
        [JsonIgnore]
        public string Key {
            get
            {
                if (HasGUID)
                    return GUID;

                return AssetPath;
            }
        }

        public AsyncOperationHandle StartAssetLoading()
        {
            if (IsAssetLoaded)
                throw new InvalidOperationException("Asset already loaded.");

            LoadHandle = MethodInvoker.Invoke<AsyncOperationHandle>(typeof(Addressables),
                new MethodBindings
            {
                MethodName = nameof(Addressables.LoadAssetAsync),
                Arguments = new object[] { Key },
                Signature = new InvokableSignature(typeof(object)),
                GenericArguments = new Type[] { AssetType }
            });

            return LoadHandle;
        }
    }

    [Serializable]
    public record AddressableDTO<T>
        :
        AddressableDTO

        where T : Object
    {
        [JsonIgnore]
        new public T? Asset => (T?)base.Asset;
        [JsonIgnore]
        new public AsyncOperationHandle<T> LoadHandle {
            get => base.LoadHandle.Convert<T>();
            set => base.LoadHandle = value;
        }

        public AddressableDTO()
        {
            AssetType = typeof(T);
        }

        new public AsyncOperationHandle<T> StartAssetLoading()
        {
            if (IsAssetLoaded)
                throw new InvalidOperationException("Asset already loaded.");

            LoadHandle = Addressables.LoadAssetAsync<T>(Key);

            return LoadHandle;
        }
    }
}
