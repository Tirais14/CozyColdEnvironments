#nullable enable
using CCEnvs.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs.Snapshots
{
    [JsonConverter(typeof(PolymorphJsonConverter<ISnapshot>))]
    public interface ISnapshot
    {
        [JsonIgnore]
        Type TargetType { get; }

        ISnapshot CaptureFrom(object obj);

        bool TryRestore(object? target, [NotNullWhen(true)] out object? restored);

        bool CanRestore(object? target);

        ISnapshot Reset();
    }

    [JsonConverter(typeof(PolymorphJsonConverter<ISnapshot>))]
    public interface ISnapshot<T> : ISnapshot
    {
        ISnapshot<T> CaptureFrom(T obj);

        bool TryRestore(T? target, [NotNullWhen(true)] out T? restored);

        bool CanRestore(T? target);

        new ISnapshot<T> Reset();

        ISnapshot ISnapshot.CaptureFrom(object obj)
        {
            return CaptureFrom((T)obj);
        }

        bool ISnapshot.TryRestore(object? target, [NotNullWhen(true)] out object? restored)
        {
            var isRestored = TryRestore((T)target!, out var restoredTyped);
            restored = restoredTyped;

            return isRestored;
        }

        bool ISnapshot.CanRestore(object? target)
        {
            return target is T typed && CanRestore(typed);
        }

        ISnapshot ISnapshot.Reset()
        {
            return Reset();
        }
    }
}
