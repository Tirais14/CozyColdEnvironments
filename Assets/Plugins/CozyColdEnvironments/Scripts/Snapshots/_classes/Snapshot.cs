using CCEnvs.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

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
        protected readonly bool isValueType = typeof(T).IsValueType;

        [JsonIgnore]
        public virtual Type TargetType { get; } = typeof(T);

        protected Snapshot()
        {
        }

        protected Snapshot(T target)
            :
            this()
        {
            CC.Guard.IsNotNullTarget(target);
        }

        public abstract bool TryRestore(T? target, [NotNullWhen(true)] out T? restored);

        public virtual bool CanRestore([NotNull] T? target)
        {
            if (!isValueType && target.IsNull())
                return false;

            return true;
        }
    }
}
