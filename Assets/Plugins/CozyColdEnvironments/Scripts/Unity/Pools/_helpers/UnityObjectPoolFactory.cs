using CCEnvs.Pools;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

#nullable enable
namespace CCEnvs.Unity.Pools
{
    public static class UnityObjectPoolFactory
    {
        public static ObjectPool<T> Create<T>(
            Func<T> factory,
            Action<T> onDestroy,
            int defaultCapacity = 10,
            bool collectionCheck = true,
            int maxSize = 100000)
            where T : class, IPoolable
        {
            return new ObjectPool<T>(
                createFunc: factory,

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

                actionOnDestroy: onDestroy,
                defaultCapacity: defaultCapacity,
                collectionCheck: collectionCheck,
                maxSize: maxSize
                );
        }
    }
}
