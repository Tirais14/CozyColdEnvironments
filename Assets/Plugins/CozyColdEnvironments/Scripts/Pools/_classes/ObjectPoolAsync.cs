#nullable enable
using CCEnvs.Patterns.Factories;
using System;
using System.Buffers;
using System.Threading;

namespace CCEnvs.Pools  
{
    public class ObjectPoolAsync<T> : ObjectPoolBase<T>, IObjectPoolAsync<T>
        where T : class
    {
        private readonly IFactory<CancellationToken,
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<T>
#else
        System.Threading.Tasks.ValueTask<T>
#endif
            >? factory;

        public override bool HasFactory => factory is not null;

        public ObjectPoolAsync(

            IFactory<CancellationToken,
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<T>
#else
        System.Threading.Tasks.ValueTask<T>
#endif
                >? factory = null,

            int capacity = 4,
            int? maxSize = null)
            :
            base(capacity, maxSize)
        {
            this.factory = factory;
        }

        public async
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<PooledHandle<T>>
#else
        System.Threading.Tasks.ValueTask<PooledHandle<T>>
#endif
            GetAsync(CancellationToken cancellationToken = default)
        {
            T? obj = null;

            while (!IsObjectValid(obj))
            {
                if (InactiveCount < 1)
                {
                    if (factory is null)
                        throw IsEmptyException();

                    obj = await factory.Create(cancellationToken);
                    Return(obj);
                }

                obj = GetFromInactive();
            }

            var handle = CreateHandle(obj);
            GetCore(handle);

            return handle;
        }

        public async
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask
#else
        System.Threading.Tasks.ValueTask
#endif
            PreheatAsync(
            int? count = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default
            )
        {
            int resolvedCount = (count ?? DefaultCapacity) - Count;

            if (batchSize is null || batchSize.Value < 1)
                batchSize = Environment.ProcessorCount * 2;

            var tasks = ArrayPool<
#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask<PooledHandle<T>>
#else
            System.Threading.Tasks.ValueTask<T>
#endif
            >.Shared.RentHandled(resolvedCount, resolvedCount);

#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask<PooledHandle<T>>
#else
            System.Threading.Tasks.ValueTask<T>
#endif
                task;

            var handles = ArrayPool<PooledHandle<T>>.Shared.RentHandled(resolvedCount, resolvedCount);

            int batchCount = (int)MathF.Round((float)resolvedCount / (float)batchSize.Value, MidpointRounding.AwayFromZero);

            PooledHandle<T> handle;

            try
            {
                for (int i = 0; i < batchCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    int taskCount = Math.Min(batchSize.Value, resolvedCount - batchSize.Value * i);

                    for (int j = 0; j < taskCount; j++)
                    {
                        cancellationToken.ThrowIfCancellationRequestedByInterval(j, taskCount / 2);

                        task = GetAsync(cancellationToken);

                        tasks[batchSize.Value * i + j] = task;
                    }

                    for (int j = 0; j < taskCount; j++)
                    {
                        cancellationToken.ThrowIfCancellationRequestedByInterval(j, taskCount / 2);

                        int offsetedIdx = batchSize.Value * i + j;

                        task = tasks[offsetedIdx];

#if UNITASK_PLUGIN
                        if (task.Status != Cysharp.Threading.Tasks.UniTaskStatus.Pending)
                            handle = task.GetAwaiter().GetResult();
#else
                        if (task.IsCompleted)
                            handle = task.Result;
#endif
                        else
                            handle = await task;

                        handles[offsetedIdx] = handle;
                    }
                }
            }
            finally
            {
                tasks.Dispose();
                handles.Dispose();
            }
        }
    }
}
