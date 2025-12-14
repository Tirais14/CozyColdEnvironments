using CCEnvs.Snapshots;
using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snaphots
{
    [Serializable]
    public class MaterialSnapshot : Snapshot<Material>
    {
        [SerializeField]
        [JsonProperty("color")]
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
