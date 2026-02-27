using System;
using CCEnvs.Snapshots;
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

        protected override GameObject? CreateValue()
        {
            return ExtraInfo!.FindGameObject().GetValue();
        }

        protected override void OnRestore(ref GameObject target)
        {
            target.name = Name;
            target.tag = Tag;
            target.layer = Layer;
            target.SetActive(ActiveSelf);

            Transform?.TryRestore(target.transform, out _);
        }
    }
}
