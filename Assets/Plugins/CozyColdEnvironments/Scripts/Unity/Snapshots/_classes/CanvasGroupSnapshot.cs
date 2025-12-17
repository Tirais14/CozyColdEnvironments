using System;
using System.Text.Json.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snaphots.UI
{
    [Serializable]
    public sealed class CanvasGroupSnapshot : BehaviourSnapshot<CanvasGroup>
    {
        [JsonInclude]
        [SerializeField]
        private float alpha = 1f;

        [JsonInclude]
        [SerializeField]
        private bool interactable = true;

        [JsonInclude]
        [SerializeField]
        private bool blockRaycasts = true;

        [JsonInclude]
        [SerializeField]
        private bool ignoreParentGroups;

        public CanvasGroupSnapshot()
        {
        }

        public CanvasGroupSnapshot(CanvasGroup target) : base(target)
        {
            alpha = target.alpha;
            interactable = target.interactable;
            blockRaycasts = target.blocksRaycasts;
            ignoreParentGroups = target.ignoreParentGroups;
        }

        [JsonConstructor]
        public CanvasGroupSnapshot(float alpha, bool interactable, bool blockRaycasts, bool ignoreParentGroups)
        {
            this.alpha = alpha;
            this.interactable = interactable;
            this.blockRaycasts = blockRaycasts;
            this.ignoreParentGroups = ignoreParentGroups;
        }

        public override CanvasGroup Restore(CanvasGroup? target)
        {
            base.Restore(target);
            CC.Guard.IsNotNull(target, nameof(target));

            target.alpha = alpha;
            target.interactable = interactable;
            target.blocksRaycasts = blockRaycasts;
            target.ignoreParentGroups = ignoreParentGroups;

            return target;
        }
    }

    public static class CanvasGroupSnapshotExtensions
    {
        public static CanvasGroupSnapshot CaptureState(this CanvasGroup source)
        {
            CC.Guard.IsNotNullSource(source);
            return new CanvasGroupSnapshot(source);
        }
    }
}
