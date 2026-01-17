using CCEnvs.Collections;
using CCEnvs.Pools;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Unity.Patterns.Factory
{
    public abstract class APooledFactory<TDiscriminator, TOut> : IDisposable
    where TOut : class, IPoolable
    {
        private readonly Dictionary<TDiscriminator, ObjectPoolProxi<TOut>> pools = new();

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

        protected APooledFactory(
            int defaultCapacity = 10,
            bool collectionCheck = true,
            int maxSize = 100000)
        {
            poolDefaultCapacity = defaultCapacity;
            poolCollectionCheck = collectionCheck;
            poolMaxSize = maxSize;
        }

        public void Preheat(TDiscriminator discriminator)
        {
            var pool = GetOrCreatePool(discriminator);

            if (pool.IsPreheated)
                return;

            pool.Preheat();
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

        protected ObjectPoolProxi<TOut> GetOrCreatePool(TDiscriminator discriminator)
        {
            CC.Guard.IsNotNull(discriminator, nameof(discriminator));
            return pools.GetOrCreate(discriminator, () => CreatePool(discriminator));
        }

        protected TOut GetObject(TDiscriminator discriminator)
        {
            var pool = GetOrCreatePool(discriminator);
            return pool.Get();
        }

        private ObjectPoolProxi<TOut> CreatePool(TDiscriminator discriminator)
        {
            var pool = new ObjectPoolProxi<TOut>(
                factory: () =>
                {
                    return CreateInternal(discriminator);
                },

                onDestroy: OnPooledDestroy,
                defaultCapacity: poolDefaultCapacity,
                collectionCheck: poolCollectionCheck,
                maxSize: poolMaxSize);

            return pool;
        }
    }
}
