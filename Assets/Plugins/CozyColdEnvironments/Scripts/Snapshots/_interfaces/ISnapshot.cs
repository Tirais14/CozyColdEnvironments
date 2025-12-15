#nullable enable
using CCEnvs.FuncLanguage;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CCEnvs.Snapshots
{
    public interface ISnapshot
    {
        [JsonIgnore]
        Maybe<object> Target { get; }

        object Restore();
        object Restore(object? target);
    }

    public interface ISnapshot<T> : ISnapshot
    {
        [JsonIgnore]
        new Maybe<T> Target { get; }

        [JsonIgnore]
        Maybe<object> ISnapshot.Target => Target;

        new T Restore();
        T Restore(T? target);

        object ISnapshot.Restore() => Restore()!;
        object ISnapshot.Restore(object? target) => Restore(target.To<T>())!;
    }

    public static class ISnapshotExtensions
    {
        public static void RestoreStates<T>(this IEnumerable<T> states)
            where T : struct, ISnapshot
        {
            CC.Guard.IsNotNull(states, nameof(states));

            foreach (var state in states)
                state.Restore();
        }

        public static void RestoreStates(this IEnumerable<ISnapshot> states)
        {
            CC.Guard.IsNotNull(states, nameof(states));

            foreach (var state in states)
                state.Restore();
        }
    }
}
