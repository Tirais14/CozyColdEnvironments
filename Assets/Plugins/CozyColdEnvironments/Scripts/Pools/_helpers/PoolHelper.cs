using CCEnvs.Diagnostics;
using CCEnvs.Patterns.Factories;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

#nullable enable 
namespace CCEnvs.Pools
{
    public static class PoolHelper
    {
        public static PooledObject<T> GetOrCreate<T>(
            this IObjectPool<T> pool,
            Func<T> factory
            )
            where T : class
        {
            CC.Guard.IsNotNull(pool, nameof(pool));
            Guard.IsNotNull(factory);

            if (pool.InactiveCount == 0)
            {
                T item = factory();
                pool.Return(item);
            }

            return pool.Get();
        }
        public static PooledObject<T> GetOrCreate<T, TState>(
            this IObjectPool<T> pool,
            TState state,
            Func<TState, T> factory
            )
            where T : class
        {
            CC.Guard.IsNotNull(pool, nameof(pool));
            Guard.IsNotNull(factory);

            if (pool.InactiveCount == 0)
            {
                T item = factory(state);
                pool.Return(item);
            }

            return pool.Get();
        }

        public static async ValueTask<PooledObject<T>> GetOrCreateAsync<T>(
            this IObjectPoolAsync<T> pool,
            Func<CancellationToken, ValueTask<T>> factory,
            CancellationToken cancellationToken = default
            )
            where T : class
        {
            CC.Guard.IsNotNull(pool, nameof(pool));
            Guard.IsNotNull(factory);

            if (pool.InactiveCount == 0)
            {
                T item = await factory(cancellationToken);
                pool.Return(item);
            }

            return await pool.GetAsync(cancellationToken);
        }
        public static async ValueTask<PooledObject<T>> GetOrCreateAsync<T, TState>(
            this IObjectPoolAsync<T> pool,
            TState state,
            Func<TState, CancellationToken, ValueTask<T>> factory,
            CancellationToken cancellationToken = default
            )
            where T : class
        {
            CC.Guard.IsNotNull(pool, nameof(pool));
            Guard.IsNotNull(factory);

            if (pool.InactiveCount == 0)
            {
                T item = await factory(state, cancellationToken);
                pool.Return(item);
            }

            return await pool.GetAsync();
        }

        public static async ValueTask PreheatAsync<T>(
            this IObjectPoolBase<T> pool,
            int count,
            int batchSize = 1,
            int delayFrameCountBetweenBatches = 0,
            IFactory<ValueTask<T>>? customFactory = null
            )
            where T : class
        {
            await new ObjectPoolPreheatOperation<T>(
                pool,
                count,
                batchSize,
                delayFrameCountBetweenBatches,
                customFactory
                )
                .ExecuteAsync();
        }

        public static void Preheat<T>(
            this IObjectPoolBase<T> pool,
            int count,
            int batchSize = 1,
            int delayFrameCountBetweenBatches = 0,
            IFactory<T>? customFactory = null
            )
            where T : class
        {
            IFactory<ValueTask<T>>? factoryAsync = null;

            if (customFactory is not null)
                factoryAsync = Factory.Create(customFactory, factory => new ValueTask<T>(factory.Create()));

            new ObjectPoolPreheatOperation<T>(
                pool,
                count,
                batchSize,
                delayFrameCountBetweenBatches,
                factoryAsync
                )
                .ExecuteAsync().Forget(ex => pool.PrintException(ex));
        }

        public static PooledObject<T> Get<T>(this IObjectPool<T> pool, Transform? parent)
            where T : Component
        {
            CC.Guard.IsNotNull(pool, nameof(pool));
            var handle = pool.Get();
            handle.Value.transform.parent = parent;
            return handle;
        }

        public static async ValueTask<PooledObject<T>> Get<T>(this IObjectPoolAsync<T> pool, Transform? parent)
            where T : Component
        {
            CC.Guard.IsNotNull(pool, nameof(pool));
            var handle = await pool.GetAsync();
            handle.Value.transform.parent = parent;
            return handle;
        }
    }
}
