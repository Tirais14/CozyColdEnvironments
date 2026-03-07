using System;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Snapshots;
using Newtonsoft.Json;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public record MaterialSnapshot<T> : Snapshot<T>
        where T : Material
    {
        [Header(nameof(Material))]
        [Space(8)]

        [JsonIgnore]
        [SerializeField]
        protected Color? m_Color;

        public Color? Color {
            get => m_Color;
            set => m_Color = value;
        }

        public MaterialSnapshot()
        {
        }

        public MaterialSnapshot(T target)
            :
            base(target)
        {
        }

        public MaterialSnapshot(Snapshot<T> original) : base(original)
        {
        }

        protected override void OnRestore(ref T target)
        {
            if (Color != null)
                target.color = Color.Value;
        }

        protected override void OnCapture(T target)
        {
            base.OnCapture(target);

            Color = target.color;
        }

        protected override void OnReset()
        {
            base.OnReset();

            Color = null;
        }
    }

    [Serializable]
    [SerializationDescriptor("MaterialSnapshot", "15fde967-8b34-4521-944a-608baa11b83f")]
    public record MaterialSnapshot : MaterialSnapshot<Material>
    {
        public MaterialSnapshot()
        {
        }

        public MaterialSnapshot(Material target) : base(target)
        {
        }

        protected MaterialSnapshot(MaterialSnapshot<Material> original) : base(original)
        {
        }
    }
}
