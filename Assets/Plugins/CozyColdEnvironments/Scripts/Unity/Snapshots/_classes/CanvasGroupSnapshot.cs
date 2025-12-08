using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snaphots.UI
{
    [Serializable]
    public class CanvasGroupSnapshot : BehaviourSnapshot
    {
        [SerializeField]
        protected float m_Alpha = 1f;

        [SerializeField]
        protected bool m_Interactable = true;

        [SerializeField]
        protected bool m_BlockRaycasts = true;

        [SerializeField]
        protected bool m_IgnoreParentGroups;

        public float Alpha => m_Alpha;
        public bool Interactable => m_Interactable;
        public bool BlockRaycasts => m_BlockRaycasts;
        public bool IgnoreParentGroups => m_IgnoreParentGroups; 

        public CanvasGroupSnapshot()
        {
        }

        public CanvasGroupSnapshot(CanvasGroup target) : base(target)
        {
            m_Alpha = target.alpha;
            m_Interactable = target.interactable;
            m_BlockRaycasts = target.blocksRaycasts;
            m_IgnoreParentGroups = target.ignoreParentGroups;
        }

        public override void Restore(object target)
        {
            var group = ValidateTarget<CanvasGroup>(target);

            group.alpha = Alpha;
            group.interactable = Interactable;
            group.blocksRaycasts = BlockRaycasts;
            group.ignoreParentGroups = IgnoreParentGroups;
        }
    }
}
