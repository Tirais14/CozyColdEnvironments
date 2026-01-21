#nullable enable
using CCEnvs.Collections;
using CCEnvs.Patterns.Factories;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Buffers;

#pragma warning disable S108
namespace CCEnvs.Pools
{
    public class ObjectPool<T> : ObjectPoolBase<T>, IObjectPool<T>
        where T : class
    {
        private readonly IFactory<T>? factory;

        private PreheatOperation? preheatOperation;

        public override bool HasFactory => factory is not null;

        public float PreheatProgress {
            get
            {
                if (preheatOperation is null)
                    return -1f;

                return preheatOperation.Progress;
            }
        }

        public bool PreheatStarted => preheatOperation is not null;

        public ObjectPool(
            IFactory<T>? factory = null,
            int capacity = 4,
            int? maxSize = null)
            :
            base(capacity, maxSize)
        {
            this.factory = factory;
        }

        public virtual PooledHandle<T> Get()
        {
            T? obj = null;

            while (obj.IsNull())
            {
                if (InactiveCount <= 0)
                {
                    if (factory is null)
                        throw IsEmptyException();

                    obj = factory.Create();
                    Return(obj);
                }

                if (fastObject is not null)
                {
                    obj = fastObject;
                    fastObject = null;
                }
                else
                    obj = inactiveItems.Pop();
            }

            var handle = CreateHandle(obj);
            OnGet(handle);

            return handle;
        }

        public IDisposable Preheat(
            int? count = null, 
            int? batchSize = null)
        {
            if (preheatOperation is not null)
                throw new InvalidOperationException("Preheating already launched");

            count ??= DefaultCapacity;

            Guard.IsGreaterThan(count.Value, 0, nameof(count));

            if (batchSize is null || batchSize.Value < 1)
                batchSize = Math.Clamp(Environment.ProcessorCount / 2, 1, int.MaxValue);

            preheatOperation = new PreheatOperation(
                this,
                count.Value,
                batchSize.Value
                );

            while (preheatOperation.MoveNext(0L))
            {
            }

            return preheatOperation;
        }

        public IDisposable Preheat(
            FrameProvider frameProvider,
            int? count = null,
            int? batchSize = null,
            int delayFrameBetweenBatchesCount = 0)
        {
            Guard.IsNotNull(frameProvider, nameof(frameProvider));

            if (preheatOperation is not null)
                throw new InvalidOperationException("Preheating already launched");

            count ??= DefaultCapacity;

            Guard.IsGreaterThan(count.Value, 0, nameof(count));

            if (batchSize is null || batchSize.Value < 1)
                batchSize = Math.Clamp(Environment.ProcessorCount / 2, 1, int.MaxValue);

            preheatOperation = new PreheatOperation(
                this,
                count.Value,
                batchSize.Value,
                delayFrameBetweenBatchesCount
                );

            frameProvider.Register(preheatOperation);

            return preheatOperation;
        }

#pragma warning disable S4487
        private sealed class PreheatOperation : IFrameRunnerWorkItem, IDisposable
        {
            public float Progress;

            private readonly ObjectPool<T> pool;

            private readonly int count;
            private readonly int batchSize;
            private readonly int frameDelayBetweenBatches;

            private readonly float progressPerItem;

            private readonly bool hasFrameDelayBetweenBatches;

            private int batch;
            private int idleFramCount;

            private bool disposed;

            private PooledArray<PooledHandle<T>> handles;

            public PreheatOperation(
                ObjectPool<T> pool,
                int count,
                int batchSize,
                int frameDelayBetweenBatches = 0)
            {
                this.pool = pool;
                this.count = count;
                this.batchSize = batchSize;
                this.frameDelayBetweenBatches = frameDelayBetweenBatches;

                progressPerItem = 1f / count;

                ///for some delay before is disposed and all handles will be freed
                Progress -= progressPerItem;

                handles = ArrayPool<PooledHandle<T>>.Shared.RentHandled(count, count);

                hasFrameDelayBetweenBatches = frameDelayBetweenBatches > 0;
            }

            public bool MoveNext(long _)
            {
                if (disposed)
                    return false;

                idleFramCount++;

                try
                {
                    if (IsFrameToSkip())
                        return true;

                    idleFramCount = 0;
                    int handlesIdxOffseted = batch * batchSize;
                    int itemCount = Math.Min(count - handlesIdxOffseted, batchSize);

                    if (itemCount < 1)
                    {
                        Dispose();
                        return false;
                    }

                    var handles = this.handles.GetSpan(handlesIdxOffseted, itemCount);

                    for (int i = 0; i < itemCount; i++)
                    {
                        if (disposed)
                        {
                            Dispose();
                            return false;
                        }

                        handles[i] = pool.Get();
                        Progress += progressPerItem;
                    }

                    batch++;

                    return true;
                }
                catch (Exception ex)
                {
                    Dispose();

                    this.PrintException(ex);

                    return false;
                }
            }

            private bool IsFrameToSkip()
            {
                if (!hasFrameDelayBetweenBatches)
                    return false;

                return batch != 0 && idleFramCount <= frameDelayBetweenBatches;
            }

            public void Dispose()
            {
                if (disposed)
                    return;

                Progress = 1f;

                handles.DisposeEach();
                handles.Dispose();

                pool.preheatOperation = null;

                disposed = true;
            }
        }
    }
}
