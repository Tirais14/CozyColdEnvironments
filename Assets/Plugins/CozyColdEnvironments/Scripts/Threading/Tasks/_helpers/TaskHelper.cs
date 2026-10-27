#nullable enable
using CommunityToolkit.Diagnostics;
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

        public static async void Forget(this Task task)
        {
            Guard.IsNotNull(task, nameof(task));
            await task;
        }
        public static async void Forget<T>(this Task<T> task)
        {
            Guard.IsNotNull(task, nameof(task));
            await task;
        }

        public static async void Forget(this Task task, Action<Exception> fallback)
        {
            Guard.IsNotNull(task, nameof(task));
            Guard.IsNotNull(fallback, nameof(fallback));
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                fallback(ex);
            }
        }
        public static async void Forget<T>(this Task<T> task, Action<Exception> fallback)
        {
            Guard.IsNotNull(task, nameof(task));
            Guard.IsNotNull(fallback, nameof(fallback));
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                fallback(ex);
            }
        }

        public static async Task Finally(this Task task, Action action, Action<Exception>? fallback = null)
        {
            Guard.IsNotNull(task, nameof(task));
            Guard.IsNotNull(action, nameof(action));

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (fallback is not null)
                    fallback(ex);
                else
                    throw;
            }
            finally
            {
                action();
            }
        }
        public static async Task Finally<T>(this Task<T> task, Action action, Action<Exception>? fallback = null)
        {
            Guard.IsNotNull(task, nameof(task));
            Guard.IsNotNull(action, nameof(action));

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (fallback is not null)
                    fallback(ex);
                else
                    throw;
            }
            finally
            {
                action();
            }
        }
        public static async Task Finally<TState>(this Task task, TState state, Action<TState> action, Action<Exception, TState>? fallback = null)
        {
            Guard.IsNotNull(task, nameof(task));
            Guard.IsNotNull(action, nameof(action));

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (fallback is not null)
                    fallback(ex, state);
                else
                    throw;
            }
            finally
            {
                action(state);
            }
        }
        public static async Task Finally<T, TState>(this Task<T> task, TState state, Action<TState> action, Action<Exception, TState>? fallback = null)
        {
            Guard.IsNotNull(task, nameof(task));
            Guard.IsNotNull(action, nameof(action));

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (fallback is not null)
                    fallback(ex, state);
                else
                    throw;
            }
            finally
            {
                action(state);
            }
        }
    }
}
