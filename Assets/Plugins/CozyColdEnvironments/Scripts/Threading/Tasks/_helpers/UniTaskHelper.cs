#if UNITASK_PLUGIN
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.Threading.Tasks
{
    public static class UniTaskHelper
    {
        //public static async UniTask RunBatched<T, TState>(
        //    IEnumerable<T> items,
        //    TState state,
        //    Func<T, TState, UniTask> taskSelector,
        //    int? batchSize = null,
        //    CancellationToken cancellationToken = default
        //    )
        //{
        //    cancellationToken.ThrowIfCancellationRequested();

        //    CC.Guard.IsNotNull(items, nameof(items));

        //    if (items.IsEmpty())
        //        return;

        //    Guard.IsNotNull(taskSelector, nameof(taskSelector));

        //    batchSize ??= Environment.ProcessorCount * 2;

        //    var tasks = ListPool<UniTask>.Shared.Get();

        //    var itemsCopy = items.EnumerableToArrayPooled();

        //    int processedCount = 0;

        //    int toProcessCount;

        //    int batchCount = (int)MathF.Ceiling(itemsCopy.Value.Count / (float)batchSize.Value);

        //    UniTask task;

        //    T item;

        //    for (int i = 0; i < batchCount; i++)
        //    {
        //        toProcessCount = Math.Min(
        //            itemsCopy.Value.Count - processedCount,
        //            batchSize.Value
        //            );

        //        for (int j = 0; j < toProcessCount; j++)
        //        {
        //            processedCount++;

        //            item = itemsCopy[i * batchSize.Value];

        //            try
        //            {
        //                task = taskSelector(item, state);
        //            }
        //            catch (Exception ex)
        //            {
        //                typeof(UniTaskHelper).PrintException(ex);
        //                continue;
        //            }

        //            tasks.Value.Add(task);
        //        }

        //        UniTask.WhenAll(tasks.Value)
        //    }
        //}

        public static async UniTask TrySwitchToThreadPool()
        {
            if (!CC.IsMainThread(Thread.CurrentThread))
                return;

            await UniTask.SwitchToThreadPool();
        }

        public static async UniTask TrySwitchToMainThread(bool configureAwait = true)
        {
            if (!configureAwait || CC.IsMainThread(Thread.CurrentThread))
                return;

            await UniTask.SwitchToMainThread();
        }

        public static async UniTask Update(
            Func<bool> moveNext,
            PlayerLoopTiming timing = PlayerLoopTiming.Update,
            CancellationToken cancellationToken = default
            )
        {
            Guard.IsNotNull(moveNext, nameof(moveNext));

            while(!cancellationToken.IsCancellationRequested && moveNext())
                await UniTask.Yield(timing, cancellationToken);
        }
        public static async UniTask Update<TState>(
            TState state,
            Func<TState, bool> moveNext,
            PlayerLoopTiming timing = PlayerLoopTiming.Update,
            CancellationToken cancellationToken = default
            )
        {
            Guard.IsNotNull(moveNext, nameof(moveNext));

            while (!cancellationToken.IsCancellationRequested && moveNext(state))
                await UniTask.Yield(timing, cancellationToken);
        }

        public static async UniTask Timer(
            TimeSpan dueTime,
            Action? action = null, 
            PlayerLoopTiming timing = PlayerLoopTiming.Update,
            bool unsacledDeltaTime = false,
            CancellationToken cancellationToken = default
            )
        {
            float seconds = 0f;
            bool fixedDeltaTime = timing == PlayerLoopTiming.FixedUpdate ||
                                  timing == PlayerLoopTiming.LastFixedUpdate;

            while (seconds < dueTime.TotalSeconds)
            {
                await UniTask.NextFrame(timing, cancellationToken);

                if (fixedDeltaTime)
                    seconds += Time.fixedDeltaTime;
                else
                    seconds += unsacledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;
            }

            action?.Invoke();
        }
        public static async UniTask Timer<TState>(
            TimeSpan dueTime,
            TState? state = default,
            Action<TState>? action = null,
            PlayerLoopTiming timing = PlayerLoopTiming.Update,
            bool unsacledDeltaTime = false,
            CancellationToken cancellationToken = default
            )
        {
            float seconds = 0f;
            bool fixedDeltaTime = timing == PlayerLoopTiming.FixedUpdate ||
                                  timing == PlayerLoopTiming.LastFixedUpdate;

            while (seconds < dueTime.TotalSeconds)
            {
                await UniTask.NextFrame(timing, cancellationToken);

                if (fixedDeltaTime)
                    seconds += Time.fixedDeltaTime;
                else
                    seconds += unsacledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;
            }

            if (state.IsNotNull())
                action?.Invoke(state);
        }
    }
}
#endif