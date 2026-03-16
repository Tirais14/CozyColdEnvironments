#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Threading.Tasks
{
    public static class TaskHelper
    {
        public static async Task WaitWhile(
            Func<bool> func, 
            CancellationToken cancellationToken = default
            )
        {
            CC.Guard.IsNotNull(func, nameof(func));

            while (func())
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }
        public static async Task WaitWhile<TState>(
            TState state,
            Func<TState, bool> func,
            CancellationToken cancellationToken = default
            )
        {
            CC.Guard.IsNotNull(func, nameof(func));

            while (func(state))
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }

        public static async Task WaitUntil(
            Func<bool> func,
            CancellationToken cancellationToken = default
            )
        {
            CC.Guard.IsNotNull(func, nameof(func));

            while (!func())
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }
        public static async Task WaitUntil<TState>(
            TState state,
            Func<TState, bool> func,
            CancellationToken cancellationToken = default
            )
        {
            CC.Guard.IsNotNull(func, nameof(func));

            while (!func(state))
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }
    }
}
