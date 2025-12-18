using CCEnvs.Snapshots;
using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public class MaterialSnapshot : Snapshot<Material>
    {
        public Color Color { get; set; }

        public MaterialSnapshot()
        {
        }

        public MaterialSnapshot(Material target)
            :
            base(target)
        {
            Color = target.color;
        }

        [JsonConstructor]
        public MaterialSnapshot(Color color)
        {
            Color = color;
        }

        public override Material Restore(Material? target)
        {
            CC.Guard.IsNotNullTarget(target);

            target.color = Color;
            return target;
        }
    }
}
