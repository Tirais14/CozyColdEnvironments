using CCEnvs.FuncLanguage;
using System;
using System.Diagnostics.CodeAnalysis;
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

        public override bool TryRestore(CanvasGroup? target, [NotNullWhen(true)] out CanvasGroup? restored)
        {
            if (!base.TryRestore(target, out restored))
                return false;

            target!.alpha = Alpha;
            target.interactable = Interctable;
            target.blocksRaycasts = BlockRaycasts;
            target.ignoreParentGroups = IgnoreParentGroups;

            restored = target;
            return true;
        }
    }
}
