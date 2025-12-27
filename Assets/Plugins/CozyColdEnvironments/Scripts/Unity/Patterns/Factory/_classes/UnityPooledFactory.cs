using CCEnvs.Collections;
using CCEnvs.Pools;
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
        private readonly Dictionary<TDiscriminator, ObjectPool<TOut>> pools = new();

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
        private readonly Func<object, TDiscriminator> discrimintatorFactory;

        protected UnityPooledFactory(
            Func<object, TDiscriminator> discrimintatorFactory,
            int defaultCapacity = 10,
            bool collectionCheck = true,
            int maxSize = 100000)
        {
            this.discrimintatorFactory = discrimintatorFactory;
            poolDefaultCapacity = defaultCapacity;
            poolCollectionCheck = collectionCheck;
            poolMaxSize = maxSize;
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

        protected abstract TOut CreateInternal();

        protected virtual void OnPooledDestroy(TOut pooled)
        {
        }

        protected ObjectPool<TOut> GetPool(object input)
        {
            return pools.GetOrCreate(discrimintatorFactory(input), () => CreatePool(input));
        }

        protected TOut GetObject(object input)
        {
            return pools[discrimintatorFactory(input)].Get();
        }

        private ObjectPool<TOut> CreatePool(object input)
        {
            return new ObjectPool<TOut>(
                createFunc: CreateInternal,

                actionOnGet: obj =>
                {
                    var handle = PooledHandle.Create(obj, pools,
                        (obj, pool) =>
                        {
                            pools[discrimintatorFactory(input)].Release(obj);
                        });

                    obj.BindPoolHandle(handle);
                    handles.TryAdd((obj, discrimintatorFactory(input)), handle);

                    obj.OnSpawned();
                },

                actionOnRelease: obj =>
                {
                    if (!handles.TryGetValue((obj, discrimintatorFactory(input)), out var handle))
                        obj.OnDespawned();

                    handle.Dispose();
                },

                actionOnDestroy: OnPooledDestroy,
                defaultCapacity: poolDefaultCapacity,
                collectionCheck: poolCollectionCheck,
                maxSize: poolMaxSize
                );
        }
    }
}
