using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using System;
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

        [SerializeField]
        protected GameObjectExtraInfo m_ExtraInfo;

        public GameObjectExtraInfo ExtraInfo {
            get => m_ExtraInfo;
            protected set => m_ExtraInfo = value;
        }

        public override bool IgnoreTarget => false;

        public ComponentSnapshot()
        {
        }

        public ComponentSnapshot(T target)
            :
            base(target)
        {
            ExtraInfo = target.GetExtraInfo();
        }

        public override Maybe<T> Restore(T? target) => target;
    }
}
