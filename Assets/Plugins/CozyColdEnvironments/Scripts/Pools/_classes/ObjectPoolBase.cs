using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable enable
namespace CCEnvs.Pools
{
    public abstract class ObjectPoolBase<T> : IObjectPoolBase<T>
        where T : class
    {
        protected readonly Queue<T> inactiveItems;
        protected readonly HashSet<PooledHandle<T>> activeItemHandles;

        protected ReactiveCommand<T>? getCmd;
        protected ReactiveCommand<T>? returnCmd;

        protected T? fastObject;

#if UNITY_EDITOR
        private bool _OnGetExecuting;
#endif

        public int Count => ActiveCount + InactiveCount;
        public int ActiveCount => activeItemHandles.Count;
        public int InactiveCount => inactiveItems.Count + (fastObject is not null ? 1 : 0);
        public abstract bool HasFactory { get; }

        protected bool IsPoolableObject { get; }
        protected int DefaultCapacity { get; }

        protected ObjectPoolBase(int capacity, int? maxSize)
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
            OnReturn(obj);
        }

        public Observable<T> ObserveReturn()
        {
            returnCmd ??= new ReactiveCommand<T>();

            return returnCmd;
        }

        public Observable<T> ObserveGet()
        {
            getCmd ??= new ReactiveCommand<T>();

            return getCmd;
        }

        private bool disposed;
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                activeItemHandles.DisposeEach();
                getCmd?.Dispose();
                returnCmd?.Dispose();
            }

            disposed = true;
        }

        protected virtual void OnGet(PooledHandle<T> handledObj)
        {
            if (IsPoolableObject)
            {
                var poolable = (IPoolable)handledObj.Value;

                if (poolable.PoolHandle.IsSome)
                    throw new InvalidOperationException($"Trying to get a poolable which has a {nameof(IPoolable.PoolHandle)}");

                poolable.PoolHandle = handledObj;

                try
                {
                    poolable.OnSpawned();
                }
                catch (Exception ex)
                {
                    poolable.PrintException(ex);
                }
            }

            activeItemHandles.Add(handledObj);
            getCmd?.Execute(handledObj.Value);
        }

        protected virtual void ReturnCore(T obj)
        {
            if (fastObject is null)
            {
                fastObject = obj;
                return;
            }

            inactiveItems.Enqueue(obj);
        }

        protected virtual void OnReturn(T obj)
        {
            if (IsPoolableObject)
            {
                var poolable = (IPoolable)obj;
                var poolablePoolHandle = poolable.PoolHandle;

                if (poolablePoolHandle.IsSome)
                {
                    if (poolablePoolHandle.Raw is not PooledHandle<T> poolHandleTyped)
                        throw new InvalidOperationException("Invalid pooled handle. Maybe is object controlls by other object pool");

                    if (!activeItemHandles.Remove(poolHandleTyped))
                    {
                        PooledHandle<T> handleToRemove = default;

                        foreach (var itemHandle in activeItemHandles.ToArrayPooled())
                        {
                            if (EqualityComparer<T?>.Default.Equals(itemHandle.Value, obj))
                                handleToRemove = itemHandle;
                        }

                        if (handleToRemove.IsDefault() || !activeItemHandles.Remove(handleToRemove))
                            this.PrintWarning($"Cannot remove object handle: {poolablePoolHandle}");
                    }
                    
                    poolable.PoolHandle = Maybe<IDisposable>.None;

                    try
                    {
                        poolable.OnDespawned();
                    }
                    catch (Exception ex)
                    {
                        poolable.PrintException(ex);
                    }
                }
            }

            returnCmd?.Execute(obj);
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
