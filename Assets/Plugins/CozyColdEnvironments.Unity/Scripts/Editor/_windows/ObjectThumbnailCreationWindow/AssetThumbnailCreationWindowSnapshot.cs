using CCEnvs.Attributes.Serialization;
using CCEnvs.Snapshots;
using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Editr
{
    [Serializable, SerializationDescriptor("ObjectThumbnailCreationWindow.Snapshot", "adc8d658-0cca-4bf5-8ca8-67ac798d2194")]
    public record AssetThumbnailCreationWindowSnapshot : Snapshot<AssetThumbnailCreationWindow>
    {
        [JsonProperty("previewTextureSize")]
        public int PreviewTextureSize { get; set; }

        [JsonProperty("typeNameFilter")]
        public string? TypeNameFilter { get; set; }

        [JsonProperty("isComponent")]
        public bool IsComponent { get; set; }

        [JsonProperty("exportInSourceDirectory")]
        public bool ExportInSourceDirectory { get; set; }

        [JsonProperty("previewPositionOffset")]
        public Vector3 PreviewPositionOffset { get; set; }

        [JsonProperty("previewRotationOffset")]
        public Vector3 PreviewRotationOffset { get; set; }

        [JsonProperty("lightRotationOffset")]
        public Vector3 LightRotationOffset { get; set; }

        protected override void OnRestore(ref AssetThumbnailCreationWindow target)
        {
            target.PreviewSize = PreviewTextureSize;
            target.TypeNameFilter = TypeNameFilter;
            target.IsComponent = IsComponent;
            //target.ExportInSourceDirectory
            target.PositionOffset = PreviewPositionOffset;
            target.RotationOffset = PreviewRotationOffset;
            target.LightRotationOffset = LightRotationOffset;
        }

        protected override void OnCapture(AssetThumbnailCreationWindow target)
        {
            base.OnCapture(target);
            PreviewTextureSize = target.PreviewSize;
            TypeNameFilter = target.TypeNameFilter;
            IsComponent = target.IsComponent;
            PreviewPositionOffset = target.PositionOffset;
            PreviewRotationOffset = target.RotationOffset;
            LightRotationOffset = target.LightRotationOffset;
        }

        protected override void OnReset()
        {
            base.OnReset();
            PreviewTextureSize = default;
            TypeNameFilter = default;
            IsComponent = default;
            PreviewPositionOffset = default;
            PreviewPositionOffset = default;
            LightRotationOffset = default;
        }
    }
}
