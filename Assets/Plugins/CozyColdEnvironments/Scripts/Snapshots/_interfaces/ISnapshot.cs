#nullable enable
using CCEnvs.FuncLanguage;
using System.Collections.Generic;

namespace CCEnvs.Snapshots
{
    public interface ISnapshot
    {
        Maybe<object> Target { get; }

        void Restore();
        void Restore(object target);
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
