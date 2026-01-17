using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Pools
{
    public abstract class AObjectPool<T> : IObjectPool
        where T : class
    {
        protected readonly Queue<T> inactiveItems;
        protected readonly HashSet<PooledHandle<T>> activeItemHandles;

        public int Count => ActiveCount + InactiveCount;
        public int ActiveCount => activeItemHandles.Count;
        public int InactiveCount => inactiveItems.Count + (FastObject is not null ? 1 : 0);
        public abstract bool HasFactory { get; }

        protected bool IsPoolableObject { get; }
        protected T? FastObject { get; private set; }
        protected int DefaultCapacity { get; }

        protected AObjectPool(int capacity, int? maxSize)
        {
            //TODO: Realize max size
            _ = maxSize;

            DefaultCapacity = capacity;
            inactiveItems = new Queue<T>(capacity);
            activeItemHandles = new HashSet<PooledHandle<T>>(capacity);

            IsPoolableObject = typeof(T).IsType<IPoolable>();
        }

        public void Clear()
        {
            inactiveItems.Clear();
        }

        public virtual void Return(T obj)
        {
            ReturnCore(obj);
        }

        private bool disposed;
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                activeItemHandles.DisposeEach();

            disposed = true;
        }

        protected virtual void OnGet(PooledHandle<T> handledObj)
        {
            if (IsPoolableObject)
            {
                var poolable = (IPoolable)handledObj.Value;

                poolable.PoolHandle = handledObj;
                poolable.OnSpawned();
            }

            activeItemHandles.Add(handledObj);
        }

        protected virtual void ReturnCore(T obj)
        {
            if (IsPoolableObject)
            {
                var poolable = (IPoolable)obj;
                var poolablePoolHandle = poolable.PoolHandle;

                if (poolablePoolHandle.IsSome)
                {
                    if (poolablePoolHandle.Raw is not PooledHandle<T>)
                        throw new InvalidOperationException("Invalid pooled handle. Maybe is object controlls by other object pool");

                    poolable.PoolHandle = Maybe<IDisposable>.None;
                }

                poolable.OnDespawned();
            }

            if (FastObject is null)
            {
                FastObject = obj;
                return;
            }

            inactiveItems.Enqueue(obj);
        }

        protected PooledHandle<T> CreateHandle(T obj)
        {
            return PooledHandle.Create(
                obj,
                this,
                static (obj, @this) =>
                {
                    @this.Return(obj);
                });
        }

        protected InvalidOperationException IsEmptyException()
        {
            return new System.InvalidOperationException("Pool is empty and a factory not found");
        }
    }
}
