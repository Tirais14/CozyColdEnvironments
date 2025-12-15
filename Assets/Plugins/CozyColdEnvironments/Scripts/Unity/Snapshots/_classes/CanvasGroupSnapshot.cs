using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snaphots.UI
{
    [Serializable]
    public class CanvasGroupSnapshot : Snapshot<CanvasGroup>
    {
        [SerializeField]
        [JsonInclude]
		[JsonPropertyName("behaviourSnapshot")]
        protected BehaviourSnapshot? behSnapshot;

        [SerializeField]
        [JsonInclude]
		[JsonPropertyName("alpha")]
        protected float alpha = 1f;

        [SerializeField]
        [JsonInclude]
		[JsonPropertyName("interactable")]
        protected bool interactable = true;

        [SerializeField]
        [JsonInclude]
		[JsonPropertyName("blockRaycasts")]
        protected bool blockRaycasts = true;

        [SerializeField]
        [JsonInclude]
		[JsonPropertyName("ignoreParentGroups")]
        protected bool ignoreParentGroups;

        public CanvasGroupSnapshot()
        {
        }

        public CanvasGroupSnapshot(CanvasGroup target) : base(target)
        {
            behSnapshot = new BehaviourSnapshot(target);
            alpha = target.alpha;
            interactable = target.interactable;
            blockRaycasts = target.blocksRaycasts;
            ignoreParentGroups = target.ignoreParentGroups;
        }

        public override CanvasGroup Restore(CanvasGroup target)
        {
            CC.Guard.IsNotNull(target, nameof(target));
            Guard.IsNotNull(behSnapshot);

            behSnapshot.Restore(target);
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
