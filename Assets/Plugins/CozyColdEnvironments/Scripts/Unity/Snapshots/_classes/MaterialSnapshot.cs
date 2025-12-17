using CCEnvs.Snapshots;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snaphots
{
    [Serializable]
    public class MaterialSnapshot : Snapshot<Material>
    {
        [JsonInclude]
        [SerializeField]
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

        [JsonConstructor]
        public MaterialSnapshot(Color color)
        {
            this.color = color;
        }

        public override Material Restore(Material? target)
        {
            CC.Guard.IsNotNullTarget(target);

            target.color = color;

            return target;
        }
    }
}
