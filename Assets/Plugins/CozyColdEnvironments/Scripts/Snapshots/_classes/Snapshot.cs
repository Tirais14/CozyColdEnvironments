using CCEnvs.Json.Converters;
using CCEnvs.Reflection;
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
        public virtual Type TargetType => CachedTypeof<T>.Type;

        protected Snapshot()
        {
        }

        protected Snapshot(T target)
            :
            this()
        {
            CC.Guard.IsNotNullTarget(target);
        }

        public virtual bool TryRestore(T? target, [NotNullWhen(true)] out T? restored)
        {
            restored = default;

            if (!CanRestore(target))
                return false;

            if (target.IsNull() && !CreateValue().Let(out target))
                return false;

            var targetNotNull = target.IsNotNull();

            if (targetNotNull)
                OnRestore(ref target!);

            return targetNotNull;
        }

        public virtual bool CanRestore(T? target)
        {
            if (!isValueType && target.IsNull())
                return false;

            return true!;
        }

        protected abstract void OnRestore(ref T target);

        protected virtual T? CreateValue()
        {
            return default;
        }
    }
}
