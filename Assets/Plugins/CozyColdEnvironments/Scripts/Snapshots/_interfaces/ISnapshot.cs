#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using CCEnvs.Serialization;
using Newtonsoft.Json;

namespace CCEnvs.Snapshots
{
    [PolymorphSerializable]
    public interface ISnapshot
    {
        [JsonIgnore]
        Type TargetType { get; }

        ISnapshot CaptureFrom(object obj);

        bool TryRestore(object? target, [NotNullWhen(true)] out object? restored);

        bool TryRestore(object? target);

        ISnapshot TryRestoreQ(object? target);

        bool CanRestore(object? target);

        ISnapshot Reset();
    }

    [PolymorphSerializable]
    public interface ISnapshot<T> : ISnapshot
    {
        ISnapshot<T> CaptureFrom(T obj);

        bool TryRestore(T? target, [NotNullWhen(true)] out T? restored);

        bool TryRestore(T? target);

        ISnapshot<T> TryRestoreQ(T? target);

        bool CanRestore(T? target);

        new ISnapshot<T> Reset();

        ISnapshot ISnapshot.CaptureFrom(object obj) => CaptureFrom((T)obj);

        bool ISnapshot.TryRestore(object? target, [NotNullWhen(true)] out object? restored)
        {
            var isRestored = TryRestore((T)target!, out var restoredTyped);
            restored = restoredTyped;

            return isRestored;
        }

        bool ISnapshot.TryRestore(object? target) => TryRestore(target, out _);

        ISnapshot ISnapshot.TryRestoreQ(object? target)
        {
            TryRestore(target);
            return this;
        }

        bool ISnapshot.CanRestore(object? target) => target is T typed && CanRestore(typed);

        ISnapshot ISnapshot.Reset() => Reset();
    }
    public interface ISnapshot<T, TSelf> : ISnapshot<T>
        where TSelf : ISnapshot<T>
    {
        new TSelf CaptureFrom(T obj);

        new TSelf Reset();

        new TSelf TryRestoreQ(T? target);

        ISnapshot<T> ISnapshot<T>.CaptureFrom(T obj) => CaptureFrom(obj);

        ISnapshot<T> ISnapshot<T>.Reset() => Reset();

        ISnapshot<T> ISnapshot<T>.TryRestoreQ(T? target) => TryRestoreQ(target);
    }
}
