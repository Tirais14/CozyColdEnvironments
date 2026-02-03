using CCEnvs.Collections;
using Cysharp.Threading.Tasks;
using System;
using System.Buffers;
using System.Linq;
using System.Threading;

#nullable enable
namespace CCEnvs.Pools
{
    public readonly struct ObjectPoolPreheatOperation<T>
        where T : class
    {
        private readonly IObjectPoolBase<T> pool;

        private readonly IObjectPool<T>? syncPool;
        private readonly IObjectPoolAsync<T>? asyncPool;

        private readonly bool isAsync;

        private readonly int count;
        private readonly int batchSize;
        private readonly int delayFrameCountBetweenBatches;

        public ObjectPoolPreheatOperation(
            IObjectPoolBase<T> pool,
            int count,
            int batchSize = 1,
            int delayFrameCountBetweenBatches = 0
            )
            :
            this()
        {
            CC.Guard.IsNotNull(pool, nameof(pool));

            this.pool = pool;

            if (pool is IObjectPoolAsync<T> asyncPool)
            {
                this.asyncPool = asyncPool;
                isAsync = true;
            }
            else
                syncPool = (IObjectPool<T>)pool;

            this.count = Math.Clamp(count, 0, int.MaxValue);
            this.batchSize = Math.Clamp(batchSize, 1, int.MaxValue);

            this.delayFrameCountBetweenBatches = delayFrameCountBetweenBatches;
        }

        public async
#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask
#else
            System.Threading.Tasks.ValueTask
#endif
            ExecuteAsync(
            CancellationToken cancellationToken = default
            )
        {
            if (count < 1)
                return;

            int batchCount = (int)MathF.Ceiling((float)count / batchSize);

            using var handles = ArrayPool<PooledHandle<T>>.Shared.Get(count);

            using var tasks = ArrayPool<
#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask<PooledHandle<T>>
#else
            System.Threading.Tasks.ValueTask<PooledHandle<T>>
#endif
                >.Shared.Get(batchSize);

            //for more readabilty
            var task = tasks.Value.FirstOrDefault();
            int processedCount = 0;

            try
            {
                for (int batch = 0; batch < batchCount; batch++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    int iterationCount = Math.Min(batchSize, count - processedCount);

                    for (int i = 0; i < iterationCount; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequestedByInterval(i, iterationCount / 2);

                        task = GetFromPoolAsync();

                        tasks[i] = task;
                    }

                    var tHandles = await
#if UNITASK_PLUGIN
                        UniTask.WhenAll(tasks.Value.Array);
#else
                        ValueTaskHelper.WhenAll(tasks.Value.Array);
#endif

                    for (int i = 0; i < iterationCount; i++)
                        handles[batch * batchSize + i] = tHandles[i];

                    if (delayFrameCountBetweenBatches > 0)
                    {
#if UNITASK_PLUGIN
                        await UniTask.DelayFrame(delayFrameCountBetweenBatches);
#else
                        await ValueTaskHelper.DelayFrame(delayFrameCountBetweenBatches);
#endif
                    }
                }
            }
            finally
            {
                handles.DisposeEach();
                handles.Dispose();
            }
        }

        private async
#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask<PooledHandle<T>>
#else
            System.Threading.Tasks.ValueTask<PooledHandle<T>>
#endif
            GetFromPoolAsync()
        {
            if (isAsync)
                await asyncPool!.GetAsync();

            return syncPool!.Get();
        }

        public override string ToString()
        {
            if (this.IsDefault())
                return StringHelper.EMPTY_OBJECT;

            return $"{nameof(pool)}: {pool}";
        }
    }
}
