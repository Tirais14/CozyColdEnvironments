#nullable enable
using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using System;

namespace CCEnvs.Pools
{
    public interface IPoolable : IUtilizable
    {
        Maybe<PooledObject> PoolHandle { get; set; }

        bool IsValid { get; }

        void OnDespawned();

        void OnSpawned();
    }

    public static class IPoolableExtensions
    {
        public static bool ReturnToPool<T>(this T poolable)
            where T : IPoolable
        {
            return poolable.PoolHandle.Map(
                x =>
                {
                    x.Dispose();
                    return true;
                })
                .GetValue();
        }

        public static void ReturnToPool<T>(this T poolable, Action<T> fallback)
            where T : IPoolable
        {
            CC.Guard.IsNotNull(poolable, nameof(poolable));
            Guard.IsNotNull(fallback, nameof(fallback));

            if (!poolable.ReturnToPool())
                fallback(poolable);
        }

        public static void ReturnToPool<T, TState>(this T poolable, TState state, Action<T, TState> fallback)
            where T : IPoolable
        {
            CC.Guard.IsNotNull(poolable, nameof(poolable));
            Guard.IsNotNull(fallback, nameof(fallback));

            if (!poolable.ReturnToPool())
                fallback(poolable, state);
        }
    }
}
