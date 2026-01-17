using CCEnvs.Pools;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Pools
{
    public class PoolableUnityObjectPool<T> : IObjectPool<T>
        where T : class, IPoolable
    {
        private readonly HashSet<IDisposable> handles = new();
        private readonly UnityEngine.Pool.ObjectPool<T> poolInternal;

        public PooledHandle<T> Get()
        {
            T obj = poolInternal.Get();

            return PooledHandle.Create(
                obj,
                this,
                static (obj, @this) =>
                {
                    @this.Release(obj);
                });
        }

        public void Release(T obj)
        {
            throw new System.NotImplementedException();
        }

        private void OnObjectDestroy(T obj)
        {

        }
    }
}
