using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using System;
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

        public override bool IgnoreTarget => false;

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

        public override Maybe<GameObject> Restore()
        {

            if (!Target.TryGetValue(out GameObject? target)
                &&
                ExtraInfo is not null
                &&
                ExtraInfo.PersistenGuid.IsNotNullOrWhiteSpace()
                &&
                GameObjectHelper.FindByPersistenGuid(ExtraInfo.PersistenGuid).TryGetValue(out GameObject? targetByGuid))
            {
                return Restore(targetByGuid);
            }

            return Restore(target!);
        }

        public override Maybe<GameObject> Restore(GameObject? target)
        {
            if (target.IsNull())
                return Maybe<GameObject>.None;

            CC.Guard.IsNotNull(Transform, nameof(Transform));
            Guard.IsNotNullOrWhiteSpace(Name);

            target.name = Name;
            target.tag = Tag;
            target.layer = Layer;
            target.SetActive(ActiveSelf);
            Transform.Restore(target.transform);

            return target;
        }
    }
}
