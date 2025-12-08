using CCEnvs.Snapshots;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snaphots
{
    [Serializable]
    public class MaterialSnapshot : Snapshot<Material>
    {
        [SerializeField]
        protected Color m_Color;

        public Color color => m_Color;

        public MaterialSnapshot()
        {
        }

        public MaterialSnapshot(Material target)
            :
            base(target)
        {
            m_Color = target.color;
        }

        public override void Restore(object target)
        {
            var mat = ValidateTarget<Material>(target);

            mat.color = color;
        }
    }
}
