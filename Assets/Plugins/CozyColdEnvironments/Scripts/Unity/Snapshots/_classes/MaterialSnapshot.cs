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
        [Header(nameof(Material))]
        [Space(8)]

        [SerializeField]
        protected Color m_Color;

        public Color Color {
            get => m_Color;
            protected set => m_Color = value;
        }

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
