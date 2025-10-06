using CCEnvs.Diagnostics;
using CCEnvs.Json.DTO;
using CCEnvs.Unity.GameSystems.Storages;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

#nullable enable

namespace CCEnvs.Unity.Json
{
    [JsonObject]
    [Serializable]
    public abstract record StorageItemReference<T> : IJsonDto
        where T : IStorageItem
    {
        [JsonProperty("id")]
        protected int ID { get; set; } = -1;

        [JsonIgnore]
        public abstract T Value { get; }

        protected StorageItemReference()
        {
        }

        protected StorageItemReference(IStorageItem item)
        {
            ID = item.ID;
        }

        public static implicit operator T(StorageItemReference<T> reference)
        {
            return reference.Value;
        }

        protected virtual void ValidateOnDeserialized()
        {
            if (ID < 1)
                throw new IncorrectDataException(ID);
        }

        [OnDeserialized]
        private void Validate(StreamingContext _)
        {
            ValidateOnDeserialized();
        }
    }
}