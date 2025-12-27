using CCEnvs.Pools;
using System;
using System.Buffers;
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
            int maxSize = 100000,
            bool preheat = false)
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
                    obj.OnDespawned();
                    handles.Remove(obj);
                },

                actionOnDestroy: onDestroy,
                defaultCapacity: defaultCapacity,
                collectionCheck: collectionCheck,
                maxSize: maxSize
                );

            if (preheat)
            {
                using var arrHandle = ArrayPool<T>.Shared.RentHandled(defaultCapacity);
                for (int i = 0; i < defaultCapacity; i++)
                {
                    arrHandle.Value[i] = Get();
                }

                foreach (var item in arrHandle.Value)
                    Release(item);
            }
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
