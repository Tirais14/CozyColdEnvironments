using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Pools
{
    public class PoolableObjectPoolProxi<TPool, T> : ObjectPoolProxi<TPool, T>
        where T : class, IPoolable
    {
        private readonly HashSet<IDisposable> objectHandles = new();

        public PoolableObjectPoolProxi(TPool pool,
            Func<TPool, PooledHandle<T>> getAction,
            Action<TPool, T> releaseAction)
            :
            base(pool, getAction, releaseAction)
        {
        }

        public override PooledHandle<T> Get()
        {
            var objHandle = base.Get();

            objHandle.Value.PoolHandle = objHandle;
            objHandle.Value.OnSpawned();

            return objHandle;
        }

        public override void Release(T obj)
        {
            Guard.IsNotNull(obj, nameof(obj));

            if (obj.PoolHandle.TryGetValue(out var handle))
                objectHandles.Remove(handle);

            base.Release(obj);

            obj.OnDespawned();
        }
    }
}
