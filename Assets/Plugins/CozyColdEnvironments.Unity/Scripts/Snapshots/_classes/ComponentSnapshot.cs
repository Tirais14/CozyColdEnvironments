using System;
using CCEnvs.Snapshots;
using Newtonsoft.Json;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public class ComponentSnapshot<T> : Snapshot<T>
        where T : Component
    {
        [Header(nameof(Component))]
        [Space(8)]

        [JsonIgnore]
        [SerializeField]
        protected GameObjectExtraInfo? m_ExtraInfo;

        public GameObjectExtraInfo? ExtraInfo {
            get => m_ExtraInfo;
            set => m_ExtraInfo = value;
        }

        public ComponentSnapshot()
        {
        }

        public ComponentSnapshot(T target)
            :
            base(target)
        {
            ExtraInfo = target.GetExtraInfo();
        }

        public override bool CanRestore(T? target)
        {
            return target != null || ExtraInfo is not null;
        }

        protected override void OnRestore(ref T target)
        {
        }

        protected override T? CreateValue()
        {
            return ExtraInfo!.FindGameObject(includeInactive: true)
                .Map(static go => go.Q().Component<T>().Raw)
                .GetValue();
        }
    }
}
