using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
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

        public override bool Restore(Material? target, [NotNullWhen(true)] out Material? restored)
        {
            if (!CanRestore(target))
            {
                restored = null;
                return false;
            }

            target.color = Color;

            restored = target;
            return true;
        }
    }
}
