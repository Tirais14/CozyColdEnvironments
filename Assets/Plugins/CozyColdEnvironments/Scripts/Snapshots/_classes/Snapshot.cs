using CCEnvs.Reflection;
using CCEnvs.Serialization;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.Snapshots
{
    public abstract record Snapshot
    {

    }

    [Serializable]
    [PolymorphSerializable]
    public abstract record Snapshot<T> : Snapshot, ISnapshot<T>
    {
        [JsonIgnore]
        public virtual Type TargetType => TypeofCache<T>.Type;

        protected Snapshot()
        {
            Reset();
        }

        protected Snapshot(T target)
            :
            this()
        {
            CaptureFrom(target);
        }

        public ISnapshot<T> CaptureFrom(T target)
        {
            CC.Guard.IsNotNull(target, nameof(target));

            OnCapture(target);

            return this;
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
            if (!TypeofCache<T>.Type.IsValueType && target.IsNull())
                return false;

            return true!;
        }

        public ISnapshot<T> Reset()
        {
            OnReset();

            return this;
        }

        protected abstract void OnRestore(ref T target);

        protected virtual T? CreateValue()
        {
            return default;
        }

        protected virtual void OnCapture(T target)
        {

        }

        protected virtual void OnReset()
        {

        }
    }
}
