using CCEnvs.FuncLanguage;
using CCEnvs.Json.Converters;
using System;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    public abstract class Snapshot
    {
        [JsonPropertyName("selfType")]
        private TypeSnapshot selfType;

        public Snapshot()
        {
            selfType = new TypeSnapshot(GetType());
        }
    }

    [Serializable]
    [JsonConverter(typeof(SnapshotConverter))]
    public abstract class Snapshot<T> : Snapshot, ISnapshot<T>
    {
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

        public abstract T Restore(T? target);
    }
}
