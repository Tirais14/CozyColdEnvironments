using CCEnvs.FuncLanguage;
using CCEnvs.Json.Converters;
using System;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Snapshots
{
    public abstract class Snapshot
    {

    }

    [Serializable]
    [JsonConverter(typeof(SnapshotConverter))]
    public abstract class Snapshot<T> : Snapshot, ISnapshot<T>
    {
#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        [JsonIgnore]
        public Maybe<T> Target { get; private set; }

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

        public T Restore() => Restore(Target.Raw);

        public abstract T Restore(T target);

        protected virtual void OnCreated()
        {
        }
    }
}
