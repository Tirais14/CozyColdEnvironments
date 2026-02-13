using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using R3;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

#nullable enable
#pragma warning disable S3267
namespace CCEnvs.Pools
{
    public abstract class ObjectPoolBase<T> : IObjectPoolBase<T>
        where T : class
    {
        protected readonly ConcurrentStack<T> inactiveItems;

        private readonly ConcurrentDictionary<T, PooledHandle<T>> activeItems;

        protected ReactiveCommand<T>? getCmd;
        protected ReactiveCommand<T>? returnCmd;

        protected T? fastObject;

        public int Count => ActiveCount + InactiveCount;
        public int ActiveCount => activeItems.Count;
        public int InactiveCount => inactiveItems.Count + (fastObject is not null ? 1 : 0);

        public abstract bool HasFactory { get; }

        protected bool IsPoolableObject { get; }

        protected int DefaultCapacity { get; }

#if UNITY_2017_1_OR_NEWER
        protected bool IsUnityGameObject { get; }
        protected bool IsUnityComponent { get; }
        protected bool IsUnityObject { get; }
#endif

        protected ObjectPoolBase(int capacity, int? maxSize)
        {
            //TODO: Realize max size
            _ = maxSize;

            DefaultCapacity = capacity;

            inactiveItems = new ConcurrentStack<T>();

            activeItems = new ConcurrentDictionary<T, PooledHandle<T>>();

            Type objType = typeof(T);

            IsPoolableObject = objType.IsType<IPoolable>();

#if UNITY_2017_1_OR_NEWER
            IsUnityObject = objType.IsType<UnityEngine.Object>();

            if (IsUnityObject)
            {
                IsUnityComponent = objType.IsType<UnityEngine.Component>();

                if (!IsUnityComponent)
                    IsUnityGameObject = objType.IsType<UnityEngine.GameObject>();
            }
#endif
        }

        public void Clear()
        {
            inactiveItems.Clear();
        }

        public virtual void Return(T? obj)
        {
            if (!IsObjectValid(obj))
                return;

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
                DisposeInactiveItems();

                activeItems.Values.DisposeEach();
                activeItems.Clear();

                getCmd?.Dispose();
                returnCmd?.Dispose();

                if (fastObject != null)
                {
                    T? exchanged;

                    do
                    {
                        exchanged = Interlocked.Exchange(ref fastObject, null);
                    }
                    while (exchanged != null);
                }
            }

            disposed = true;
        }

        protected virtual void GetCore(PooledHandle<T> handledObj)
        {
            T obj = handledObj.Value;

#if UNITY_2017_1_OR_NEWER
            TryProcessUnityObjectOnGet(obj);
#endif

            TryProcessPoolableObjectOnGet(handledObj);

            activeItems.TryAdd(obj, handledObj);
        }

        protected virtual void OnGet(PooledHandle<T> handledObj)
        {
            getCmd?.Execute(handledObj.Value);
        }

        protected virtual void ReturnCore(T obj)
        {
            activeItems.TryRemove(obj, out _);

#if UNITY_2017_1_OR_NEWER

            TryProcessUnityObjectOnReturn(obj);
#endif

            TryProcessPoolableObjectOnReturn(obj);
        }

        protected virtual void OnReturn(T obj)
        {
            var tFastObject = fastObject;

            if (Interlocked.CompareExchange(ref fastObject, obj, tFastObject) != tFastObject)
                inactiveItems.Push(obj);

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

        protected void TryProcessPoolableObjectOnGet(PooledHandle<T> obj)
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
            return Interlocked.CompareExchange(ref fastObject, null, fastObject).Let(out result)
                   ||
                   inactiveItems.TryPop(out result);
        }

        protected bool IsObjectValid([NotNullWhen(true)] T? obj)
        {
            if (obj.IsNull())
                return false;

            if (!IsPoolableObject)
                return true;

            return ((IPoolable)obj).IsValid;
        }

        private void OnPoolableGet(IPoolable poolable, PooledHandle<T> handledObj)
        {
            if (poolable.PoolHandle.IsSome)
                throw new InvalidOperationException($"Object: {handledObj.Value} already has pool handle");

            poolable.PoolHandle = handledObj;

            poolable.OnSpawned();
        }

        private void OnPoolableReturn(IPoolable poolable)
        {
            if (poolable.PoolHandle.IsSome)
                poolable.PoolHandle.Raw.As<PooledHandle<T>>().Maybe().GetValueUnsafe(static () => throw new InvalidOperationException("Invalid pool handle. Maybe is object controlls by other pool."));

            poolable.PoolHandle = Maybe<IDisposable>.None;

            poolable.OnDespawned();
        }

        private void DisposeInactiveItems()
        {
            foreach (var item in inactiveItems)
            {
#if UNITY_2017_1_OR_NEWER
                if (TryGetGameObject(item, out var go))
                    UnityEngine.Object.Destroy(go);
#endif

                if (item is IDisposable objDispsoable)
                    objDispsoable.Dispose();
            }

            inactiveItems.Clear();
        }

#if UNITY_2017_1_OR_NEWER

        protected void TryProcessUnityObjectOnGet(T obj)
        {
            if (IsUnityObject
                &&
                TryGetGameObject(obj, out var go))
            {
                OnGameObjectGet(go);
            }
        }

        protected void TryProcessUnityObjectOnReturn(T obj)
        {
            if (IsUnityObject
                &&
                TryGetGameObject(obj, out var go))
            {
                OnGameObjectReturn(go);
            }
        }

        private void OnGameObjectReturn(UnityEngine.GameObject go)
        {
            go.transform.position = new UnityEngine.Vector3(0f, -100000f);
            go.SetActive(false);
        }

        private void OnGameObjectGet(UnityEngine.GameObject go)
        {
            go.transform.position = new UnityEngine.Vector3(0f, -100000f);
            go.SetActive(true);
        }

        private bool TryGetGameObject(T obj, [NotNullWhen(true)] out UnityEngine.GameObject? go)
        {
            go = null;

            if (IsUnityGameObject)
                go = obj.To<UnityEngine.GameObject>();
            else if (IsUnityComponent)
                go = obj.To<UnityEngine.Component>().gameObject;

            return go != null;
        }
#endif
    }
}
