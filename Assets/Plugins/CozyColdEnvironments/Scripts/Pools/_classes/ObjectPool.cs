#nullable enable
using CCEnvs.Collections;
using CCEnvs.Patterns.Commands;
using CCEnvs.Patterns.Factories;
using CommunityToolkit.Diagnostics;
using Humanizer;
using R3;
using System;
using System.Buffers;

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

        public PooledHandle<T> Get()
        {
            T obj;

            if (FastObject is not null)
                obj = FastObject;
            else if (InactiveCount <= 0)
            {
                if (factory is null)
                    throw IsEmptyException();

                obj = factory.Create();
            }
            else
                obj = inactiveItems.Dequeue();

            var handle = CreateHandle(obj);
            OnGet(handle);

            return handle;
        }

        /// <param name="frameProvider">if null will be setted <see cref="TimerFrameProvider"/> with period of 32 ms</param>
        public LazyLight<ICommand, IObjectPool<T>> Preheat(
            FrameProvider? frameProvider,
            int? count = null, 
            int? batchSize = null)
        {
            if (preheatOperation is not null)
                throw new InvalidOperationException("Preheating already launched");

            count ??= DefaultCapacity;

            Guard.IsGreaterThan(count.Value, 0, nameof(count));

            if (batchSize is null || batchSize.Value < 1)
                batchSize = Environment.ProcessorCount;

            frameProvider ??= new TimerFrameProvider(32.Milliseconds());

            preheatOperation = new PreheatOperation(this, count.Value, batchSize.Value);

            frameProvider.Register(preheatOperation);

            return new LazyLight<ICommand, IObjectPool<T>>(this,
                static (@this) =>
                {
                    return Command.Builder.ExecuteWhen(@this,
                        static @this =>
                        {
                            return @this.PreheatProgress >= 1f;
                        })
                        .Build(@this);
                });
        }

#pragma warning disable S4487
        private sealed class PreheatOperation : IFrameRunnerWorkItem
        {
            public float Progress;

            private readonly ObjectPool<T> pool;
            private readonly int count;
            private readonly int batchSize;
            private readonly float progressPerItem;

            private int currentBatch;
            private PooledArray<PooledHandle<T>> handles;

            public PreheatOperation(ObjectPool<T> pool, int count, int batchSize)
            {
                this.pool = pool;
                this.count = count;
                this.batchSize = batchSize;

                progressPerItem = 1f / count;
                handles = ArrayPool<PooledHandle<T>>.Shared.RentHandled(count, count);
            }

            public bool MoveNext(long _)
            {
                int handlesIdx = currentBatch * batchSize;
                int itemCount = Math.Min(count - handlesIdx, batchSize);
                var handles = new Span<PooledHandle<T>>(this.handles.Value.Array, handlesIdx, itemCount);

                for (int i = 0; i < itemCount; i++)
                    handles[i] = pool.Get();

                currentBatch++;

                if (itemCount < 1)
                {
                    Close();
                    return false;
                }

                Progress += progressPerItem;
                return true;
            }

            private void Close()
            {
                Progress = 1f;

                handles.DisposeEach();
                pool.preheatOperation = null;
            }
        }
    }
}
