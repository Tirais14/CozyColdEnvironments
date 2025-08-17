using Newtonsoft.Json;
using System;
using System.Reflection;
using UnityEngine.AddressableAssets;
using UTIRLib.Json.Convertes;
using UnityEngine;

#nullable enable
#pragma warning disable S101
namespace UTIRLib.Json.DTO
{
    [Serializable]
    [JsonConverter(typeof(AssetReferenceDTOConverter))]
    public class AssetReferenceDTO<TRef> : ISerializationCallbackReceiver where TRef : AssetReference
    {
        [SerializeField] private TRef reference = default!;

        [JsonIgnore]
        public TRef Reference {
            get => reference;
            set => reference = value;
        }

        [JsonProperty("guid")]
        public string Guid { get; set; } = string.Empty;

        [JsonProperty("sub", NullValueHandling = NullValueHandling.Ignore)]
        public string? SubObjectName { get; set; }

        public AssetReferenceDTO() { }
        public AssetReferenceDTO(TRef reference) { Reference = reference; SyncFromReference(); }

        public void SyncFromReference()
        {
            if (reference == null)
            {
                Guid = string.Empty;
                SubObjectName = null;
                return;
            }

            Guid = reference.AssetGUID ?? string.Empty;
            SubObjectName = TryGetSubObjectName(reference);
        }

        public TRef? ToReference()
        {
            if (string.IsNullOrEmpty(Guid)) return null;
            var inst = (TRef?)Activator.CreateInstance(typeof(TRef), Guid);
            if (inst != null && !string.IsNullOrWhiteSpace(SubObjectName))
                TrySetSubObjectName(inst, SubObjectName!);
            return inst;
        }

        // Автосинк при изменении в инспекторе/сердесере Unity
        public void OnBeforeSerialize() => SyncFromReference();
        public void OnAfterDeserialize() { /* не требуется */ }
#if UNITY_EDITOR
        private void OnValidate() => SyncFromReference();
#endif

        private static string? TryGetSubObjectName(AssetReference r)
        {
            var prop = r.GetType().GetProperty("SubObjectName", BindingFlags.Instance | BindingFlags.Public);
            if (prop?.CanRead == true)
            {
                var val = prop.GetValue(r) as string;
                if (!string.IsNullOrWhiteSpace(val)) return val;
            }
            var field = r.GetType().GetField("m_SubObjectName", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                var val = field.GetValue(r) as string;
                if (!string.IsNullOrWhiteSpace(val)) return val;
            }
            return null;
        }

        private static void TrySetSubObjectName(AssetReference r, string value)
        {
            var prop = r.GetType().GetProperty("SubObjectName", BindingFlags.Instance | BindingFlags.Public);
            if (prop?.CanWrite == true) { prop.SetValue(r, value); return; }
            var field = r.GetType().GetField("m_SubObjectName", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null) field.SetValue(r, value);
        }
    }
}
