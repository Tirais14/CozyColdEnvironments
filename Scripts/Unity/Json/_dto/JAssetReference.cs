using CCEnvs.Async;
using CCEnvs.Diagnostics;
using CCEnvs.Json.DTO;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;
using CCEnvs.Unity.AddressableAssets;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

#nullable enable
#pragma warning disable S101
namespace CCEnvs.Unity.Json.Converters
{
    [JsonObject]
    [Serializable]
    public record JAssetReference : IJsonDto
    {
        [JsonProperty]
        public string AssetPath { get; set; } = string.Empty;

        [JsonProperty("GUID")]
        public string GUID { get; set; } = string.Empty;

        [JsonProperty]
        public Type AssetType { get; set; } = null!;

        [JsonProperty]
        public bool ImmediateStartLoading { get; set; }

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

            LoadHandle = MethodInvoker.Invoke<AsyncOperationHandle>(
                TypeValuePair.T<object>(Key),
                nameof(Addressables.LoadAssetAsync),
                new ExplicitArguments(ExplicitArgument.T<object>(Key)),
                AssetType);

            return LoadHandle;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext _)
        {
            if (ImmediateStartLoading)
                CC.NeccesaryTasks.RegisterTask(StartAssetLoading());
        }
    }

    [Serializable]
    public record JAssetReference<T>
        :
        JAssetReference

        where T : Object
    {
        [JsonIgnore]
        new public T Asset => (T)base.Asset;
        [JsonIgnore]
        new public AsyncOperationHandle<T> LoadHandle {
            get => base.LoadHandle.Convert<T>();
            set => base.LoadHandle = value;
        }

        public JAssetReference()
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
