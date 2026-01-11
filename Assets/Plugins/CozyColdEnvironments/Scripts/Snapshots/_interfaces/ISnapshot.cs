#nullable enable
using CCEnvs.FuncLanguage;
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

        bool TryRestore(object? target, [NotNullWhen(true)] out object? restored);

        bool CanRestore(object? target);
    }

    [JsonConverter(typeof(PolymorphJsonConverter<ISnapshot>))]
    public interface ISnapshot<T> : ISnapshot
    {
        bool TryRestore(T? target, [NotNullWhen(true)] out T? restored);

        bool CanRestore(T? target);

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
    }

    //public static class ISnapshotExtensions
    //{
    //    public static void RestoreSnapshotStates<T>(this IEnumerable<T> states)
    //        where T : struct, ISnapshot
    //    {
    //        CC.Guard.IsNotNull(states, nameof(states));

    //        foreach (var state in states)
    //            state.Restore();
    //    }

    //    public static void RestoreSnapshotStates(this IEnumerable<ISnapshot> states)
    //    {
    //        CC.Guard.IsNotNull(states, nameof(states));

    //        foreach (var state in states)
    //            state.Restore();
    //    }
    //}
}
