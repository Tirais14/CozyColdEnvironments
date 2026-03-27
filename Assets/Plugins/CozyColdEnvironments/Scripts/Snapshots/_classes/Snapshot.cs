using CCEnvs.Reflection;
using CCEnvs.Serialization;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using Humanizer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#nullable enable
namespace CCEnvs.Snapshots
{
    public abstract record Snapshot
    {
        private static readonly Dictionary<Type, ConstructorInfo> ctors = new(0);
        private static readonly Dictionary<Type, SnapshotConvertibleAttribute> attributes = new(0);

        private static readonly object ctorsGate = new();
        private static readonly object attributesGate = new();

        public static ConstructorInfo GetEmptyConstructor(
            Type snapshotType,
            bool throwIfNotFound = true
            )
        {
            Guard.IsNotNull(snapshotType, nameof(snapshotType));

            ConstructorInfo? ctor;

            lock (ctorsGate)
            {
                if (!ctors.TryGetValue(snapshotType, out ctor))
                {
                    ctor = snapshotType.GetConstructor(
                        BindingFlagsDefault.InstanceAll,
                        null,
                        Type.EmptyTypes,
                        Array.Empty<ParameterModifier>()
                        );

                    ctors.Add(snapshotType, ctor);
                }
            }

            if (throwIfNotFound && ctor is null)
                throw new InvalidOperationException($"Cannot find empty constructor. SnapshotType: {snapshotType}");

            return ctor;
        }

        public static ConstructorInfo GetConstructorByAttribute(
            Type type,
            bool throwIfNotFound = true
            )
        {
            Guard.IsNotNull(type, nameof(type));

            SnapshotConvertibleAttribute? attribute;

            lock (attributesGate)
            {
                if (!attributes.TryGetValue(type, out attribute))
                {
                    attribute = type.GetCustomAttribute<SnapshotConvertibleAttribute>(inherit: true);

                    attributes.Add(type, attribute);
                }
            }

            if (attribute is null)
            {
                if (throwIfNotFound)
                    throw new InvalidOperationException($"Cannot find {nameof(SnapshotConvertibleAttribute).Humanize()} attribute. Type: {type}");

                return null!;
            }
            else if (attribute.SnapshotType is null)
            {
                if (throwIfNotFound)
                    throw new InvalidOperationException($"Snapshot type in attribute {nameof(SnapshotConvertibleAttribute).Humanize()} cannot be null. Type: {type}");

                return null!;
            }

            return GetEmptyConstructor(attribute.SnapshotType, throwIfNotFound);
        }

        public static AnonymousSnapshot Create<T>()
        {
            return new AnonymousSnapshot(TypeofCache<T>.Type);
        }

        public static AnonymousSnapshot Create(Type type)
        {
            return new AnonymousSnapshot(type);
        }

        public static AnonymousSnapshot Create(object target)
        {
            return (AnonymousSnapshot)new AnonymousSnapshot(target).CaptureFrom(target);
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

            restored = target;
            return targetNotNull;
        }

        public bool TryRestore(T? target) => TryRestore(target, out _);

        public ISnapshot<T> TryRestoreQ(T? target)
        {
            TryRestore(target);
            return this;
        }

        public virtual bool CanRestore(T? target)
        {
            if (!TypeofCache<T>.Type.IsValueType && target.IsNull())
                return false;

            return true;
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

    public abstract record Snapshot<T, TSelf> : Snapshot<T>, ISnapshot<T, TSelf>
        where TSelf : Snapshot<T>
    {
        protected Snapshot()
        {
        }

        protected Snapshot(Snapshot<T> original) : base(original)
        {
        }

        protected Snapshot(T target) : base(target)
        {
        }

        public new TSelf TryRestoreQ(T? target) => (TSelf)base.TryRestoreQ(target);

        public new TSelf CaptureFrom(T target)
        {
            return (TSelf)base.CaptureFrom(target);
        }

        public new TSelf Reset()
        {
            return (TSelf)base.Reset();
        }
    }
}
