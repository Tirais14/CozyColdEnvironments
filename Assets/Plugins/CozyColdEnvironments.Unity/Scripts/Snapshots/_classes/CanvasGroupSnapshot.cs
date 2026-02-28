using CCEnvs.Attributes.Serialization;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots.UI
{
    [Serializable]
    [TypeSerializationDescriptor("CanvasGroupSnapshot", "5b152852-c9bb-4775-aeb0-aaad5b29143e")]
    public sealed record CanvasGroupSnapshot : BehaviourSnapshot<CanvasGroup>
    {
        [field: SerializeField]
        public float? Alpha { get; set; } = 1f;

        [field: SerializeField]
        public bool? Interctable { get; set; } = true;

        [field: SerializeField]
        public bool? BlockRaycasts { get; set; } = true;

        [field: SerializeField]
        public bool? IgnoreParentGroups { get; set; }

        public CanvasGroupSnapshot()
        {
        }

        public CanvasGroupSnapshot(CanvasGroup target) : base(target)
        {
        }

        public CanvasGroupSnapshot(BehaviourSnapshot<CanvasGroup> original) : base(original)
        {
        }

        protected override void OnRestore(ref CanvasGroup target)
        {
            base.OnRestore(ref target);

            if (Alpha.HasValue)
                target!.alpha = Alpha.Value;

            if (Interctable.HasValue)
                target.interactable = Interctable.Value;

            if (BlockRaycasts.HasValue)
                target.blocksRaycasts = BlockRaycasts.Value;

            if (IgnoreParentGroups.HasValue)
                target.ignoreParentGroups = IgnoreParentGroups.Value;
        }

        protected override void OnCapture(CanvasGroup target)
        {
            base.OnCapture(target);

            Alpha = target.alpha;
            Interctable = target.interactable;
            BlockRaycasts = target.blocksRaycasts;
            IgnoreParentGroups = target.ignoreParentGroups;
        }

        protected override void OnReset()
        {
            base.OnReset();

            Alpha = default;
            Interctable = default;
            BlockRaycasts = default;
            IgnoreParentGroups = default;
        }
    }
}
