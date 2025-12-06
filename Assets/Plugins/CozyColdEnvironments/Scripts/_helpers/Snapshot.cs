#nullable enable
using System.Collections.Generic;

namespace CCEnvs
{
    public static class Snapshot
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
