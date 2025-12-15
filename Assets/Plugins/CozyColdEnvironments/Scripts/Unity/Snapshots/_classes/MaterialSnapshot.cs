using CCEnvs.Snapshots;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snaphots
{
    [Serializable]
    public class MaterialSnapshot : Snapshot<Material>
    {
        [SerializeField]
        [JsonInclude]
		[JsonPropertyName("color")]
        protected Color color;

        public MaterialSnapshot()
        {
        }

        public MaterialSnapshot(Material target)
            :
            base(target)
        {
            color = target.color;
        }

        public override Material Restore(Material target)
        {
            CC.Guard.IsNotNullTarget(target);

            target.color = color;

            return target;
        }
    }
}
