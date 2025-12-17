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
        public string? Name { get; set; }
        public string? Tag { get; set; }
        public int Layer { get; set; }
        public bool ActiveSelf { get; set; } = true;
        public TransformSnapshot? Transform { get; set; }
        public string? Guid { get; set; }

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
            Guid = target.GetGuid().Raw;
        }

        public override GameObject Restore()
        {

            if (!Target.TryGetValue(out GameObject? target)
                && 
                Guid.IsNotNullOrWhiteSpace()
                &&
                GameObjectHelper.FindByGuid(Guid).TryGetValue(out GameObject? targetByGuid))
            {
                return Restore(targetByGuid);
            }

            return Restore(target!);
        }

        public override GameObject Restore(GameObject target)
        {
            CC.Guard.IsNotNullTarget(target);
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
