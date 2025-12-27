using CCEnvs.Collections;
using CCEnvs.Pools;
using CCEnvs.Unity.Pools;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;

#nullable enable
namespace CCEnvs.Unity.Patterns.Factory
{
    public abstract class UnityPooledFactory<TOut> : IDisposable
        where TOut : class, IPoolable
    {
        protected readonly ObjectPool<TOut> pool;
        private readonly Dictionary<TOut, IDisposable> handles = new();

        protected UnityPooledFactory(
            int defaultCapacity = 10,
            bool collectionCheck = true,
            int maxSize = 100000)
        {
            pool = new ObjectPool<TOut>(
                createFunc: CreateInternal,

                actionOnGet: obj =>
                {
                    var handle = PooledHandle.Create(obj, pool,
                        static (obj, pool) =>
                        {
                            pool.Release(obj);
                        });

                    obj.BindPoolHandle(handle);
                    handles.TryAdd(obj, handle);

                    obj.OnSpawned();
                },

                actionOnRelease: obj =>
                {
                    if (!handles.TryGetValue(obj, out var handle))
                        obj.OnDespawned();

                    handle.Dispose();
                },

                actionOnDestroy: OnPooledDestroy,
                defaultCapacity: defaultCapacity,
                collectionCheck: collectionCheck,
                maxSize: maxSize
                );
        }

        private bool disposed;
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                pool.Dispose();
                handles.Values.DisposeEach();
            }

            disposed = true;
        }

        protected abstract TOut CreateInternal();

        protected virtual void OnPooledDestroy(TOut pooled)
        {
        }
    }

    public abstract class UnityPooledFactory<TDiscriminator, TOut> : IDisposable
    where TOut : class, IPoolable
    {
        private readonly Dictionary<TDiscriminator, UnityObjectPoolExtended<TOut>> pools = new();

        private readonly Dictionary<(TOut obj, TDiscriminator discriminator), IDisposable> handles = new(comparer: new AnonymousEqualityComparer<(TOut obj, TDiscriminator discriminator)>(
            comparison: (left, right) =>
            {
                return ReferenceEquals(left.obj, right.obj)
                       &&
                       EqualityComparer<TDiscriminator>.Default.Equals(left.discriminator, right.discriminator);
            },
            hashCodeGenerator: (pair) =>
            {
                return HashCode.Combine(RuntimeHelpers.GetHashCode(pair.obj), EqualityComparer<TDiscriminator>.Default.GetHashCode(pair.discriminator));
            }));

        private readonly int poolDefaultCapacity;
        private readonly bool poolCollectionCheck;
        private readonly int poolMaxSize;
        private readonly bool poolPreheat;
        private readonly Func<TOut, TDiscriminator> discrimintatorFactory;

        protected UnityPooledFactory(
            Func<TOut, TDiscriminator> discrimintatorFactory,
            int defaultCapacity = 10,
            bool collectionCheck = true,
            int maxSize = 100000,
            bool preheat = false)
        {
            this.discrimintatorFactory = discrimintatorFactory;
            poolDefaultCapacity = defaultCapacity;
            poolCollectionCheck = collectionCheck;
            poolMaxSize = maxSize;
            poolPreheat = preheat;
        }

        private bool disposed;
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                pools.Values.DisposeEach();
                handles.Values.DisposeEach();
            }

            disposed = true;
        }

        protected abstract TOut CreateInternal(object? input);

        protected virtual void OnPooledDestroy(TOut pooled)
        {
        }

        protected UnityObjectPoolExtended<TOut> GetOrCreatePool(TOut obj)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            TDiscriminator discriminator = discrimintatorFactory(obj);   
            return pools.GetOrCreate(discriminator, () => CreatePool(discriminator));
        }

        protected TOut GetObject(TDiscriminator discriminator)
        {
            return pools[discriminator].Get();
        }

        protected UnityObjectPoolExtended<TOut> CreatePool(TDiscriminator discriminator)
        {
            return new UnityObjectPoolExtended<TOut>(
                factory: () =>
                {
                    return CreateInternal(discriminator);
                },

                onDestroy: OnPooledDestroy,
                defaultCapacity: poolDefaultCapacity,
                collectionCheck: poolCollectionCheck,
                maxSize: poolMaxSize,
                preheat: poolPreheat
                );
        }
    }
}
