using CCEnvs.Snapshots;
using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snaphots.UI
{
    [Serializable]
    public class CanvasGroupSnapshot : Snapshot<CanvasGroup>
    {
        [SerializeField]
        [JsonProperty("behaviourSnapshot")]
        protected BehaviourSnapshot behSnapshot = new();

        [SerializeField]
        [JsonProperty("alpha")]
        protected float alpha = 1f;

        [SerializeField]
        [JsonProperty("interactable")]
        protected bool interactable = true;

        [SerializeField]
        [JsonProperty("blockRaycasts")]
        protected bool blockRaycasts = true;

        [SerializeField]
        [JsonProperty("ignoreParentGroups")]
        protected bool ignoreParentGroups;

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

        public override CanvasGroup Restore(CanvasGroup target)
        {
            CC.Guard.IsNotNull(target, nameof(target));

            target = behSnapshot.Restore(target).To<CanvasGroup>();
            target.alpha = alpha;
            target.interactable = interactable;
            target.blocksRaycasts = blockRaycasts;
            target.ignoreParentGroups = ignoreParentGroups;

            return target;
        }
    }
}
