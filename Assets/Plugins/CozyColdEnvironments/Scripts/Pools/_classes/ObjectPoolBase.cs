using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using R3;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
#pragma warning disable S3267
namespace CCEnvs.Pools
{
    public abstract class ObjectPoolBase<T> : IObjectPoolBase<T>
        where T : class
    {
        protected readonly Stack<T> inactiveItems;
        protected readonly Dictionary<T, PooledHandle<T>> activeItemHandles;

        protected ReactiveCommand<T>? getCmd;
        protected ReactiveCommand<T>? returnCmd;

        protected T? fastObject;

        public int Count => ActiveCount + InactiveCount;
        public int ActiveCount => activeItemHandles.Count;
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
            inactiveItems = new Stack<T>(capacity);
            activeItemHandles = new Dictionary<T, PooledHandle<T>>(capacity, new ReferenceEqualityComparer<T>());

            Type objType = typeof(T);
            IsPoolableObject = objType.IsType<IPoolable>();

#if UNITY_2017_1_OR_NEWER
            IsUnityGameObject = objType.IsType<UnityEngine.GameObject>();

            if (!IsUnityGameObject)
                IsUnityComponent = objType.IsType<UnityEngine.Component>();

            IsUnityObject = IsUnityComponent || IsUnityGameObject;
#endif
        }

        public void Clear()
        {
            inactiveItems.Clear();
        }

        public virtual void Return(T? obj)
        {
            if (obj is null)
                return;

            if (obj.Equals(null))
            {
                activeItemHandles.Remove(obj!);
                return;
            }

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
                activeItemHandles.Values.DisposeEach();
                activeItemHandles.Clear();
                getCmd?.Dispose();
                returnCmd?.Dispose();
                fastObject = null;
            }

            disposed = true;
        }

        protected virtual void OnGet(PooledHandle<T> handledObj)
        {
            T obj = handledObj.Value;

#if UNITY_2017_1_OR_NEWER
            TryProcessUnityObjectOnGet(obj);
#endif

            if (IsPoolableObject)
            {
                var poolable = (IPoolable)obj;

                poolable.PoolHandle.IfSome(static _ => throw new InvalidOperationException($"Trying to get a poolable which has a {nameof(IPoolable.PoolHandle)}"));
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

            getCmd?.Execute(obj);
        }

        protected virtual void ReturnCore(T obj)
        {
            if (fastObject is null)
            {
                fastObject = obj;
                return;
            }

            inactiveItems.Push(obj);
        }

        protected virtual void OnReturn(T obj)
        {
#if UNITY_2017_1_OR_NEWER

            TryProcessUnityObjectOnReturn(obj);
#endif

            if (IsPoolableObject)
            {
                var poolable = (IPoolable)obj;

                if (poolable.PoolHandle.IsSome)
                    poolable.PoolHandle.Raw.As<PooledHandle<T>>().GetValueUnsafe(static () => throw new InvalidOperationException("Invalid pool handle. Maybe is object controlls by other pool."));

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

            RemoveFromActive(obj);
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

        private void RemoveFromActive(T obj)
        {
            activeItemHandles.Remove(obj);
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
