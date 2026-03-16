using System;
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
            int delayFrameCountBetweenBatches = 0)
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

            using var handles = new PooledArray<PooledObject<T>>(count);

            using var tasks = new PooledArray<
#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask<PooledObject<T>>
#else
            System.Threading.Tasks.ValueTask<PooledObject<T>>
#endif
                >(batchSize);

#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask<PooledObject<T>>
#else
            System.Threading.Tasks.ValueTask<PooledObject<T>>
#endif
                task;

            int processedCount = 0;

            for (int batch = 0; batch < batchCount; batch++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int processCount = Math.Min(batchSize, count - processedCount);

                for (int i = 0; i < processCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    task = GetFromPoolAsync(cancellationToken);

                    tasks[i] = task;
                }

                var tHandles = await
#if UNITASK_PLUGIN
                    Cysharp.Threading.Tasks.UniTask.WhenAll(tasks.Value.Array);
#else
                    ValueTaskSupplement.ValueTaskEx.WhenAll(tasks.Value.Array);
#endif

                for (int i = 0; i < processCount; i++)
                    handles[batch * batchSize + i] = tHandles[i];

                if (delayFrameCountBetweenBatches > 0)
                {
#if UNITASK_PLUGIN
                    await Cysharp.Threading.Tasks.UniTask.DelayFrame(delayFrameCountBetweenBatches);
#else
                    await CCEnvs.Threading.Tasks.ValueTaskHelper.DelayFrame(delayFrameCountBetweenBatches);
#endif
                }

                processedCount += processCount;
            }

            handles.DisposeEach(bufferized: false);
        }

        private async
#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask<PooledObject<T>>
#else
            System.Threading.Tasks.ValueTask<PooledObject<T>>
#endif
            GetFromPoolAsync(CancellationToken cancellationToken)
        {
            if (isAsync)
                return await asyncPool!.GetAsync(cancellationToken);

            return syncPool!.Get();
        }

        public override string ToString()
        {
            if (this.IsDefault())
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(pool)}: {pool}; {nameof(batchSize)}: {batchSize}; {nameof(delayFrameCountBetweenBatches)}: {delayFrameCountBetweenBatches})";
        }
    }
}
