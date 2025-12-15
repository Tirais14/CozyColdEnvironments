using CCEnvs.Snapshots;
using Newtonsoft.Json;
using System;
using System.Collections.Immutable;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    [Serializable]
    public struct SaveContext
    {
        [JsonProperty("scene")]
        public SceneInfo SceneInfo { get; }

        [JsonProperty("data")]
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
