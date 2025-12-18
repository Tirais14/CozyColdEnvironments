using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots.UI
{
    [Serializable]
    public sealed class CanvasGroupSnapshot : BehaviourSnapshot<CanvasGroup>
    {
        [field: SerializeField]
        public float Alpha { get; private set; } = 1f;

        [field: SerializeField]
        public bool Interctable { get; private set; } = true;

        [field: SerializeField]
        public bool BlockRaycasts { get; private set; } = true;

        [field: SerializeField]
        public bool IgnoreParentGroups { get; private set; }

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
