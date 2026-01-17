using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Pools;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using System;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

#nullable enable
#pragma warning disable S927
namespace CCEnvs.Unity.Pools
{
    public class ObjectPoolWrapped<T> : IObjectPool<T>, IDisposable
        where T : class, IPoolable
    {
        protected readonly ObjectPool<T> poolInternal;
        private readonly Dictionary<T, IDisposable> handles = new();

        public int CountInactive => ((UnityEngine.Pool.IObjectPool<T>)poolInternal).CountInactive;
        public int DefaultCapacity { get; }
        public bool IsPreheated { get; private set; }

        public ObjectPoolWrapped(
            Func<T> factory,
            Action<T>? onDestroy = null,
            int defaultCapacity = 10,
            bool collectionCheck = true,
            int maxSize = 100000)
        {
            poolInternal = new ObjectPool<T>(
                createFunc: factory,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: onDestroy,
                defaultCapacity: defaultCapacity,
                collectionCheck: collectionCheck,
                maxSize: maxSize
                );

            DefaultCapacity = defaultCapacity;
        }

        private static void CreateHandle(T obj, ObjectPoolWrapped<T> instance)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));
            Guard.IsNotNull(instance, nameof(instance));

            instance.Release(obj);
        }

        public void Clear() => poolInternal.Clear();

        public T Get() => poolInternal.Get();

        public PooledObject<T> Get(out T v) => poolInternal.Get(out v);

        public void Release(T obj)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            bool hasPoolHandle = obj.PoolHandle.IsSome;

            try
            {
                poolInternal.Release(obj);
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException
                    &&
                    !hasPoolHandle)
                {
                    this.PrintWarning("Trying to release item which already released");
                    return;
                }

                throw;
            }
        }

        public void Preheat()
        {
            if (IsPreheated)
                return;

            using var arrHandle = ArrayPool<T>.Shared.RentHandled(DefaultCapacity);

            for (int i = 0; i < DefaultCapacity; i++)
                arrHandle.Value[i] = Get();

            foreach (var item in arrHandle.Value.GetArraySegment(DefaultCapacity))
                Release(item);

            IsPreheated = true;
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

        protected virtual void OnGet(T obj)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            var handle = PooledHandle.Create(obj, this, CreateHandle);

            obj.PoolHandle = handle;
            handles.TryAdd(obj, handle);

            obj.OnSpawned();
            UniTask.Create(obj,
                static async obj =>
                {
                    await UniTask.NextFrame(timing: PlayerLoopTiming.Initialization);
                    obj.OnSpawnedLate();
                })
                .Forget();
        }

        protected virtual void OnRelease(T obj)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            obj.PoolHandle = Maybe<IDisposable>.None;
            obj.OnDespawned();

            handles.Remove(obj);
        }
    }
}
