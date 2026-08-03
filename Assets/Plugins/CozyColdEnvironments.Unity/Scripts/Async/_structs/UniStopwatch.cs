#if UNITASK_PLUGIN
using CCEnvs.Disposables;
using CCEnvs.Threading;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Async
{
    public struct UniStopwatch : IDisposable
    {
        private CancellationTokenSource cancellationTokenSource;

        public float Elapsed { get; private set; }

        public UniStopwatchTimeType TimeType { get; }
        public PlayerLoopTiming Timing { get; }

        public bool IsStarted { get; private set; }

        public UniStopwatch(
            UniStopwatchTimeType timeType,
            PlayerLoopTiming timing = PlayerLoopTiming.Update
            )
        {
            cancellationTokenSource = new CancellationTokenSource();
            disposed = default;
            Elapsed = default;
            IsStarted = false;

            TimeType = timeType;
            Timing = timing;
        }

        public static UniStopwatch Create(
            UniStopwatchTimeType timeType = UniStopwatchTimeType.Default,
            PlayerLoopTiming timing = PlayerLoopTiming.Update
            )
        {
            return new UniStopwatch(UniStopwatchTimeType.Default, timing);
        }

        public UniStopwatch Reset()
        {
            Elapsed = 0f;
            return this;
        }

        public UniStopwatch Start()
        {
            OnTickAsync().Forget();
            IsStarted = true;
            return this;
        }

        public UniStopwatch Stop()
        {
            IsStarted = false;
            return this;
        }

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            cancellationTokenSource.CancelAndDispose();
        }

        private readonly float GetDeltaTime()
        {
            return TimeType switch
            {
                UniStopwatchTimeType.DeltaTime => Time.deltaTime,
                UniStopwatchTimeType.UnscaledDeltaTime => Time.unscaledDeltaTime,
                _ => throw CC.ThrowHelper.InvalidOperationException(TimeType)
            };
        }

        private async UniTask OnTickAsync()
        {
#if CC_DEBUG_ENABLED
            var loopFuse = LoopFuse.Create();
#endif

            while (!CCDisposable.IsDisposed(disposed)
                   &&
                   !cancellationTokenSource.IsCancellationRequested)
            {
#if CC_DEBUG_ENABLED
                loopFuse.MoveNextThrow();
#endif

                await UniTask.Yield(Timing, cancellationTokenSource.Token);
                Elapsed += GetDeltaTime();
            }
        }
    }
}
#endif