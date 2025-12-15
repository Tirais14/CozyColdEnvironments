using CCEnvs.Snapshots;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Immutable;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    [Serializable]
    public struct SaveContext
    {
        [JsonInclude]
		[JsonPropertyName("scene")]
        public SceneInfo SceneInfo { get; }

        [JsonInclude]
		[JsonPropertyName("data")]
        public ImmutableArray<string> Data { get; }

        [JsonConstructor]
        public SaveContext(
            SceneInfo sceneInfo,
            ImmutableArray<string> serialized)
        {
            SceneInfo = sceneInfo;
            Data = serialized;
        }
    }
}
