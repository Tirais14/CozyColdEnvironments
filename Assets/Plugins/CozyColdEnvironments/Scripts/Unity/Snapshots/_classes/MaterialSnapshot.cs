using CCEnvs.FuncLanguage;
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

        public override bool IgnoreTarget => false;

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

        public override Maybe<Material> Restore(Material? target)
        {
            if (target.IsNull())
                return Maybe<Material>.None;

            target.color = Color;
            return target;
        }
    }
}
