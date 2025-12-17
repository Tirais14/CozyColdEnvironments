using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots.UI
{
    [Serializable]
    public sealed class CanvasGroupSnapshot : BehaviourSnapshot<CanvasGroup>
    {
        public float Alpha { get; set; } = 1f;
        public bool Interctable { get; set; } = true;
        public bool BlockRaycasts { get; set; } = true;
        public bool IgnoreParentGroups { get; set; }

        public CanvasGroupSnapshot()
        {
        }

        public CanvasGroupSnapshot(CanvasGroup target) : base(target)
        {
            Alpha = target.alpha;
            Interctable = target.interactable;
            BlockRaycasts = target.blocksRaycasts;
            IgnoreParentGroups = target.ignoreParentGroups;
        }

        public override CanvasGroup Restore(CanvasGroup target)
        {
            base.Restore(target);
            CC.Guard.IsNotNull(target, nameof(target));

            target.alpha = Alpha;
            target.interactable = Interctable;
            target.blocksRaycasts = BlockRaycasts;
            target.ignoreParentGroups = IgnoreParentGroups;

            return target;
        }
    }
}
