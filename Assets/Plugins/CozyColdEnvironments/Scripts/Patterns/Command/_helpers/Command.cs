using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public static class Command
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnonymousCommand Create<T>(
            Func<bool> isReadyToExecute,
            Action execute,
            string? name = null)
        {
            return new AnonymousCommand(isReadyToExecute, execute, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnonymousCommand<T> Create<T>(
            T state,
            Predicate<T> isReadyToExecute,
            Action<T> execute,
            string? name = null)
        {
            return new AnonymousCommand<T>(state, isReadyToExecute, execute, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnonymousCommand<T, T1> Create<T, T1>(
            T state,
            T1 state1,
            Func<T, T1, bool> isReadyToExecute,
            Action<T, T1> execute,
            string? name = null)
        {
            return new AnonymousCommand<T, T1>(
                state,
                state1,
                isReadyToExecute,
                execute,
                name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnonymousCommand<T, T1, T2> Create<T, T1, T2>(
            T state,
            T1 state1,
            T2 state2,
            Func<T, T1, T2, bool> isReadyToExecute,
            Action<T, T1, T2> execute,
            string? name = null)
        {
            return new AnonymousCommand<T, T1, T2>(
                state,
                state1,
                state2,
                isReadyToExecute,
                execute,
                name);
        }
    }
}
