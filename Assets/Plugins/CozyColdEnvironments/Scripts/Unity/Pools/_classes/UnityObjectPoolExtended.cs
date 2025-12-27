using CCEnvs.Pools;
using System;
using System.Collections.Generic;
using UnityEngine.Pool;

#nullable enable
namespace CCEnvs.Unity.Pools
{
    public class UnityObjectPoolExtended<T> : IObjectPool<T>, IDisposable
        where T : class, IPoolable
    {
        protected readonly ObjectPool<T> poolInternal;
        private readonly Dictionary<T, IDisposable> handles = new();

        public int CountInactive => ((IObjectPool<T>)poolInternal).CountInactive;

        public UnityObjectPoolExtended(
            Func<T> factory,
            Action<T>? onDestroy = null,
            int defaultCapacity = 10,
            bool collectionCheck = true,
            int maxSize = 100000)
        {
            poolInternal = new ObjectPool<T>(
                createFunc: factory,

                actionOnGet: obj =>
                {
                    var handle = PooledHandle.Create(obj, poolInternal,
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

                actionOnDestroy: onDestroy,
                defaultCapacity: defaultCapacity,
                collectionCheck: collectionCheck,
                maxSize: maxSize
                );
        }

        public void Clear() => poolInternal.Clear();

        public T Get() => poolInternal.Get();

        public PooledObject<T> Get(out T v) => poolInternal.Get(out v);

        public void Release(T element) => poolInternal.Release(element);

        private bool disposed;
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                poolInternal.Dispose();
                handles.Values.DisposeEach();
            }

            disposed = true;
        }
    }
}
