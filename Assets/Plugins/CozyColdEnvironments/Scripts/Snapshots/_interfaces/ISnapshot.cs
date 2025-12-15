#nullable enable
using CCEnvs.FuncLanguage;
using System.Collections.Generic;

namespace CCEnvs.Snapshots
{
    public interface ISnapshot
    {
        Maybe<object> Target { get; }

        object Restore();
        object Restore(object? target);
    }

    public interface ISnapshot<T> : ISnapshot
    {
        new Maybe<T> Target { get; }

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
