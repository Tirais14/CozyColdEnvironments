#nullable enable
using CCEnvs.Collections;
using CCEnvs.Patterns.Factories;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Buffers;
using System.Collections.Generic;

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
            FrameProvider? frameProvider,
            int? count = null, 
            int? batchSize = null,
            int delayFrameBetweenBatchesCount = 0)
        {
            if (preheatOperation is not null)
                throw new InvalidOperationException("Preheating already launched");

            count ??= DefaultCapacity;

            Guard.IsGreaterThan(count.Value, 0, nameof(count));

            if (batchSize is null || batchSize.Value < 1)
                batchSize = Environment.ProcessorCount;

            preheatOperation = new PreheatOperation(
                this,
                count.Value,
                batchSize.Value,
                delayFrameBetweenBatchesCount
                );

            if (frameProvider is not null)
                frameProvider.Register(preheatOperation);
            else
            {
                while (preheatOperation.MoveNext(0L))
                {
                }
            }

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

            private int currentBatch;
            private int currentFrame;

            private bool disposed;

            private PooledArray<PooledHandle<T>> handles;

            public PreheatOperation(
                ObjectPool<T> pool,
                int count,
                int batchSize,
                int frameDelayBetweenBatches)
            {
                this.pool = pool;
                this.count = count;
                this.batchSize = batchSize;
                this.frameDelayBetweenBatches = frameDelayBetweenBatches;

                progressPerItem = 1f / count;
                handles = ArrayPool<PooledHandle<T>>.Shared.RentHandled(count, count);
                hasFrameDelayBetweenBatches = frameDelayBetweenBatches > 0;
            }

            public bool MoveNext(long _)
            {
                if (disposed)
                    return false;

                if (hasFrameDelayBetweenBatches && !PassFrame())
                    return true;

                int handlesIdx = currentBatch * batchSize;
                int itemCount = Math.Min(count - handlesIdx, batchSize);

                if (itemCount < 1)
                {
                    Close();
                    return false;
                }

                var handles = new Span<PooledHandle<T>>(this.handles.Value.Array, handlesIdx, itemCount);

                for (int i = 0; i < itemCount; i++)
                {
                    handles[i] = pool.Get();
                    Progress += progressPerItem;
                }

                currentBatch++;

                return true;
            }

            private void Close()
            {
                Progress = 1f;

                handles.DisposeEach();
                handles.Dispose();
                pool.preheatOperation = null;
            }

            private bool PassFrame()
            {
                currentFrame++;

                if (currentFrame <= frameDelayBetweenBatches)
                    return false;

                currentFrame = 1;
                return true;
            }

            public void Dispose()
            {
                if (disposed)
                    return;

                Close();

                disposed = true;
            }
        }
    }
}
