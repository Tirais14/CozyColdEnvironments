using CCEnvs.Diagnostics;
using CCEnvs.Json.DTO;
using CCEnvs.Reflection;
using CCEnvs.Reflection.ObjectModel;
using Newtonsoft.Json;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

#nullable enable
#pragma warning disable S101
namespace CCEnvs.Unity.Json.Converters
{
    [JsonObject]
    [Serializable]
    public record AddressableDto : IJsonDto
    {
        [JsonProperty]
        public string AssetPath { get; set; } = string.Empty;
        [JsonProperty]
        public string GUID { get; set; } = string.Empty;
        [JsonProperty]
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

            LoadHandle = MethodHelper.Invoke<AsyncOperationHandle>(
                new TypeValuePair(typeof(object), Key),
                nameof(Addressables.LoadAssetAsync),
                new ExplicitArguments(new TypeValuePair(typeof(object), Key)),
                new Signature(AssetType));

            return LoadHandle;
        }
    }

    [Serializable]
    public record AddressableDto<T>
        :
        AddressableDto

        where T : Object
    {
        [JsonIgnore]
        new public T Asset => (T)base.Asset;
        [JsonIgnore]
        new public AsyncOperationHandle<T> LoadHandle {
            get => base.LoadHandle.Convert<T>();
            set => base.LoadHandle = value;
        }

        public AddressableDto()
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
