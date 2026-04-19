using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using R3;

#if CC_DEBUG_ENABLED
using System.Linq;
#endif

#nullable enable
#pragma warning disable S3267
namespace CCEnvs.Pools
{
    public abstract class ObjectPoolBase<T> : IObjectPoolBase<T>
        where T : class
    {
        protected static Action<T, object> ReturnAction { get; } = static (obj, pool) =>
        {
            ((ObjectPoolBase<T>)pool).Return(obj);
        };

        protected static InvalidOperationException PoolEmptyException { get; } = new("Pool is empty and a factory not found");

        protected readonly ConcurrentStack<T> inactiveItems;

        protected readonly ConcurrentDictionary<T, PooledObject<T>> activeItems = new(ReferenceEqualityComparer<T>.Instance);

        protected ReactiveCommand<T>? getCmd;
        protected ReactiveCommand<T>? returnCmd;

        protected T? fastObject;

        public int Count => ActiveCount + InactiveCount;
        public int ActiveCount => activeItems.Count;
        public int InactiveCount {
            get
            {
                if (fastObject is not null)
                    return inactiveItems.Count + 1;

                return inactiveItems.Count;
            }
        }

        public abstract bool HasFactory { get; }

        protected bool IsPoolableObject { get; }

        protected ObjectPoolBase(int capacity, int? maxSize)
        {
            //TODO: Realize max size
            _ = maxSize;

            inactiveItems = new ConcurrentStack<T>();

            activeItems = new ConcurrentDictionary<T, PooledObject<T>>();

            Type objType = typeof(T);

            IsPoolableObject = objType.IsType<IPoolable>();
        }

        ~ObjectPoolBase() => Dispose();

        public void Clear()
        {
            inactiveItems.Clear();
        }

        public virtual void Return(T? obj)
        {
            if (!IsObjectValid(obj))
                return;

#if CC_DEBUG_ENABLED

            if (ReferenceEqualityComparer<T?>.Instance.Equals(obj, fastObject)
                ||
                inactiveItems.Where(static x => x.IsNotNull()).Contains(obj, ReferenceEqualityComparer<T>.Instance))
            {
                throw new InvalidOperationException($"Object: {obj} is already pooled");
            }

#endif

            ReturnCore(obj);
            OnReturn(obj);
        }

        public bool IsActiveObject(T obj)
        {
            if (IsPoolableObject)
                return ((IPoolable)obj).PoolHandle.IsSome;

            return activeItems.ContainsKey(obj);
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
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                DisposeInactiveItems();

                activeItems.Values.DisposeEach();
                activeItems.Clear();

                getCmd?.Dispose();
                returnCmd?.Dispose();
                fastObject = null;
            }

            disposed = true;
        }

        protected virtual void GetCore(PooledObject<T> handledObj)
        {
            T obj = handledObj.Value;

            TryProcessPoolableObjectOnGet(handledObj);

            activeItems.TryAdd(obj, handledObj);
        }

        protected virtual void OnGet(PooledObject<T> handledObj)
        {
            getCmd?.Execute(handledObj.Value);
        }

        protected virtual void ReturnCore(T obj)
        {
            activeItems.TryRemove(obj, out _);

            TryProcessPoolableObjectOnReturn(obj);
        }

        protected virtual void OnReturn(T obj)
        {
            if (Interlocked.CompareExchange(ref fastObject, obj, null) is not null)
                inactiveItems.Push(obj);

            returnCmd?.Execute(obj);
        }

        protected PooledObject<T> CreateHandle(T obj)
        {
            return PooledObject.Create(
                obj,
                this,
                ReturnAction
                );
        }

        protected void TryProcessPoolableObjectOnGet(PooledObject<T> obj)
        {
            if (IsPoolableObject)
                OnPoolableGet((IPoolable)obj.Value, obj);
        }

        protected void TryProcessPoolableObjectOnReturn(T obj)
        {
            if (IsPoolableObject)
                OnPoolableReturn((IPoolable)obj);
        }

        protected bool TryGetFromInactive([NotNullWhen(true)] out T? result)
        {
            var succes = Interlocked.CompareExchange(ref fastObject, null, fastObject).Is(out result)
                         ||
                         inactiveItems.TryPop(out result);

            return succes && IsPoolableValid(result);
        }

        protected bool IsObjectValid([NotNullWhen(true)] T? obj)
        {
            if (obj.IsNull())
                return false;

            return IsPoolableValid(obj);
        }

        protected bool IsPoolableValid(T obj)
        {
            if (!IsPoolableObject)
                return true;

            return ((IPoolable)obj).IsValid;
        }

        private void OnPoolableGet(IPoolable poolable, PooledObject<T> handledObj)
        {
            if (poolable.PoolHandle.IsSome)
                throw new InvalidOperationException($"Object: {handledObj.Value} already has pool handle");

            poolable.PoolHandle = (PooledObject)handledObj;

            poolable.OnSpawned();
        }

        private void OnPoolableReturn(IPoolable poolable)
        {
            //if (poolable.PoolHandle.IsSome)
            //    throw new InvalidOperationException("Invalid pool handle. Maybe is object controlls by other pool.");

            poolable.PoolHandle = Maybe<PooledObject>.None;

            poolable.OnDespawned();
        }

        private void DisposeInactiveItems()
        {
            foreach (var item in inactiveItems)
            {
                if (item is IDisposable objDispsoable)
                    objDispsoable.Dispose();
            }

            inactiveItems.Clear();
        }
    }
}
