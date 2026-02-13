using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Pools
{
    public static class Poolable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Validate(this IPoolable poolable)
        {
            CC.Guard.IsNotNull(poolable, nameof(poolable));

            if (poolable.PoolHandle.IsSome)
                throw new System.InvalidOperationException($"Cannot access the poolable: {poolable}, because it in pool");
        }
    }
}
