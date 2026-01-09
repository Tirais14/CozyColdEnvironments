using CCEnvs.Pools;
using Cysharp.Threading.Tasks;
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
        public int DefaultCapacity { get; }

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
                    UniTask.Create(obj,
                        static async obj =>
                        {
                            await UniTask.NextFrame(timing: PlayerLoopTiming.Initialization);
                            obj.OnSpawnedLate();
                        })
                        .Forget();
                },

                actionOnRelease: obj =>
                {
                    if (obj.IsNotNull())
                        obj.OnDespawned();

                    handles.Remove(obj);
                },

                actionOnDestroy: onDestroy,
                defaultCapacity: defaultCapacity,
                collectionCheck: collectionCheck,
                maxSize: maxSize
                );

            DefaultCapacity = defaultCapacity;
        }

        public void Clear() => poolInternal.Clear();

        public T Get() => poolInternal.Get();

        public PooledObject<T> Get(out T v) => poolInternal.Get(out v);

        public void Release(T element) => poolInternal.Release(element);

        public void Preheat()
        {
            using var arrHandle = ArrayPool<T>.Shared.RentHandled(DefaultCapacity);

            for (int i = 0; i < DefaultCapacity; i++)
                arrHandle.Value[i] = Get();

            foreach (var item in arrHandle.Value)
                Release(item);
        }

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
