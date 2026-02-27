using System;
using CCEnvs.Snapshots;
using Newtonsoft.Json;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public class MaterialSnapshot : Snapshot<Material>
    {
        [Header(nameof(Material))]
        [Space(8)]

        [JsonIgnore]
        [SerializeField]
        protected Color m_Color;

        public Color Color {
            get => m_Color;
            set => m_Color = value;
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

        protected override void OnRestore(ref Material target)
        {
            target.color = Color;
        }
    }
}
