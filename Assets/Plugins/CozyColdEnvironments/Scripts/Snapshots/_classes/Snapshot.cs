using CCEnvs.Reflection;
using CCEnvs.Saves;
using CCEnvs.Serialization;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs.Snapshots
{
    public abstract record Snapshot
    {
        public static bool TryGetConstructor(Type snapshotType, [NotNullWhen(true)] out ConstructorInfo? ctor)
        {
            Guard.IsNotNull(snapshotType, nameof(snapshotType));

            ctor = (from ctr in snapshotType.GetConstructors(BindingFlagsDefault.InstanceAll)
                    let prms = ctr.GetParameters()
                    where prms.Count(param => !param.HasDefaultValue) == 1
                    select ctr)
                    .FirstOrDefault();

            return ctor != null;
        }

        public static bool TryResolveSnapshotType(Type type, [NotNullWhen(true)] out Type? snapshotType)
        {
            if (type.GetCustomAttribute<SnapshotConvertibleAttribute>(inherit: true).IsNull(out var snapshotConvertibleAttribute))
            {
                snapshotType = null;
                return false;
            }

            snapshotType = snapshotConvertibleAttribute.SnapshotType;
            return snapshotType != null;
        }

        public static bool TryGetConstructorByAttribute(Type type, [NotNullWhen(true)] out ConstructorInfo? ctor)
        {
            Guard.IsNotNull(type, nameof(type));

            ctor = null;

            return TryResolveSnapshotType(type, out var snapshotType)
                   &&
                   TryGetConstructor(snapshotType, out ctor);
        }

        public static ConstructorInfo GetConstructor(Type snapshotType)
        {
            if (!TryGetConstructor(snapshotType, out var ctor))
                throw new InvalidOperationException($"Not found snapshot constructor with only one required parameter. Type: {snapshotType.Name}");

            return ctor;
        }

        public static ConstructorInfo GetConstructorByAttribute(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            if (!TryResolveSnapshotType(type, out var snapshotType))
                throw new InvalidOperationException($"Cannot resolve snapshot type. Type: {type}");

            return GetConstructor(snapshotType);
        }
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

            if (target.IsNull() && CreateValue().IsNot(out target))
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
