using CCEnvs.FuncLanguage;
using CCEnvs.Json.Converters;
using Newtonsoft.Json;
using System;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    [JsonConverter(typeof(SnapshotConverter))]
    public abstract class Snapshot<T> : ISnapshot<T>
    {
        [JsonProperty("selfType")]
        private TypeSnapshot selfType;

        [JsonIgnore]
        public Maybe<T> Target { get; private set; }

        public Snapshot()
        {
            selfType = new TypeSnapshot(GetType());
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
