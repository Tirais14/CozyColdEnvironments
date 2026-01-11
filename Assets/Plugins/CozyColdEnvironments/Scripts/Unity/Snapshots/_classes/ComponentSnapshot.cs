using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using System;
using System.Diagnostics.CodeAnalysis;
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
        protected GameObjectExtraInfo? m_ExtraInfo;

        public GameObjectExtraInfo? ExtraInfo {
            get => m_ExtraInfo;
            protected set => m_ExtraInfo = value;
        }

        protected ComponentSnapshot()
        {
        }

        protected ComponentSnapshot(T target)
            :
            base(target)
        {
            ExtraInfo = target.GetExtraInfo();
        }

        /// <summary>
        /// Tries to find in scene if <paramref name="target"/> is null.
        /// </summary>
        public override bool TryRestore(T? target, [NotNullWhen(true)] out T? restored)
        {
            if (!CanRestore(target))
            {
                restored = null;
                return false;
            }

            if (target == null
                &&
                (ExtraInfo is null
                ||
                ExtraInfo.FindGameObject(includeInactive: true).Map(go => go.Q().Component<T>().Raw).TryGetValue(out target)
                ))
            {
                restored = null;
                return false;
            }

            restored = target!;
            return true;
        }

        public override bool CanRestore([NotNull] T? target)
        {
            return target != null || ExtraInfo is not null;
        }
    }
}
