using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public sealed class GameObjectSnapshot : Snapshot<GameObject>
    {
        [field: SerializeField]
        public string? Name { get; private set; }

        [field: SerializeField]
        public string? Tag { get; private set; }

        [field: SerializeField]
        public int Layer { get; private set; }

        [field: SerializeField]
        public bool ActiveSelf { get; private set; } = true;

        [field: SerializeField]
        public TransformSnapshot? Transform { get; private set; }

        [field: SerializeField]
        public GameObjectExtraInfo? ExtraInfo { get; private set; }

        public GameObjectSnapshot()
        {
        }

        public GameObjectSnapshot(GameObject target) : base(target)
        {
            Name = target.name;
            Tag = target.tag;
            Layer = target.layer;
            ActiveSelf = target.activeSelf;
            Transform = new TransformSnapshot(target.transform);
            ExtraInfo = target.GetExtraInfo();
        }

        public override bool Restore(
            GameObject? target, 
            [NotNullWhen(true)] out GameObject? restored)
        {
            if (!CanRestore(target))
            {
                restored = null;
                return false;
            }

            if (target == null
                && 
                (ExtraInfo is null || !ExtraInfo.FindGameObject().TryGetValue(out target)))
            {
                restored = null;
                return false;
            }

            CC.Guard.IsNotNull(Transform, nameof(Transform));
            Guard.IsNotNullOrWhiteSpace(Name);

            target.name = Name;
            target.tag = Tag;
            target.layer = Layer;
            target.SetActive(ActiveSelf);

            if (Transform is not null)
                Transform.Restore(target.transform, out _);

            restored = target;
            return true;
        }
    }
}
