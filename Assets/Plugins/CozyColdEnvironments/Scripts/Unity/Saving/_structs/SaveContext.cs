using CCEnvs.Snapshots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using UnityEditor.Search;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    [Serializable]
    public struct SaveContext
    {
        [JsonInclude]
        [JsonPropertyName("data")]
        private ISnapshot[] data;

        [JsonInclude]
		[JsonPropertyName("scene")]
        public SceneInfo SceneInfo { get; }

        [JsonIgnore]
        public readonly IEnumerable<ISnapshot> Data => data;

        [JsonConstructor]
        public SaveContext(SceneInfo sceneInfo, IEnumerable<ISnapshot> data)
        {
            SceneInfo = sceneInfo;
            this.data = data.ToArray();
        }
    }
}
