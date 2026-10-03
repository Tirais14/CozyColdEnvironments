using CommunityToolkit.Diagnostics;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Pools
{
    public static class PoolableHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Validate(this IPoolable poolable)
        {
            CC.Guard.IsNotNull(poolable, nameof(poolable));

            if (poolable.PoolHandle.IsSome)
                throw new System.InvalidOperationException($"Cannot access the poolable: {poolable}, because it in pool");
        }

        public static void ReturnToPool<T>(this T poolable, Action<T> fallback)
            where T : IPoolable
        {
            CC.Guard.IsNotNull(poolable, nameof(poolable));
            Guard.IsNotNull(fallback);

            if (!poolable.ReturnToPool())
                fallback(poolable);
        }

        public static void ReturnToPool<T, TState>(this T poolable, TState state, Action<T, TState> fallback)
            where T : IPoolable
        {
            CC.Guard.IsNotNull(poolable, nameof(poolable));
            Guard.IsNotNull(fallback);

            if (!poolable.ReturnToPool())
                fallback(poolable, state);
        }
    }
}
