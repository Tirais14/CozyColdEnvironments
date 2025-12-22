using CCEnvs.FuncLanguage;
using CCEnvs.Json.Converters;
using Newtonsoft.Json;
using System;

#nullable enable
namespace CCEnvs.Snapshots
{
    public abstract class Snapshot
    {

    }

    [Serializable]
    [JsonConverter(typeof(SnapshotJsonConverter))]
    public abstract class Snapshot<T> : Snapshot, ISnapshot<T>
    {
        [JsonIgnore]
#if UNITY_2017_1_OR_NEWER
        [field: UnityEngine.SerializeField]
#endif
        protected T? m_Target;

        [JsonIgnore]
        public Maybe<T> Target {
            get => m_Target;
            set => m_Target = value.Raw;
        }

        [JsonIgnore]
        public virtual Type TargetType => typeof(T);

        public virtual bool CanRestoreWithoutTarget => IgnoreTarget;
        public abstract bool IgnoreTarget { get; }

        public Snapshot()
        {
        }

        public Snapshot(T target)
            :
            this()
        {
            CC.Guard.IsNotNull(target, nameof(target));

            Target = target;
        }

        public virtual Maybe<T> Restore() => Restore(Target.Raw!);

        public abstract Maybe<T> Restore(T? target);

        public bool CanRestore()
        {
            if (!CanRestoreWithoutTarget && Target.IsNone)
                return false;

            return true;
        }

        protected T FlushTarget(T? target)
        {
            CC.Guard.IsNotNullTarget(target);
            return target;
        }
    }
}
