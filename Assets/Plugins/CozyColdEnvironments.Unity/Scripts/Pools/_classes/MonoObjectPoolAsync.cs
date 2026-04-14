using CCEnvs.Patterns.Factories;
using CCEnvs.Pools;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Patterns.Factory;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Pools
{
    public abstract class MonoObjectPoolAsync<T, TCore, TFactory>
        :
        CCBehaviour,
        IObjectPoolAsync<T>

        where T : UnityEngine.Object
        where TCore : IObjectPoolAsync<T>
        where TFactory : MonoBehaviour, IFactory<CancellationToken, ValueTask<T>>
    {
        [Header("Pool Settings")]
        [Space(6f)]

        [SerializeField]
        protected TFactory factory;

        [SerializeField, Min(0f)]
        protected int capacity = 4;

        [Header("Preheat Settings")]
        [Space(6f)]

        [SerializeField]
        protected int preheatCount;

        [SerializeField, Min(1f)]
        protected int preheatBatchSize;

        [SerializeField, Min(0f)]
        protected int preheatDelayFrameCountBetweenBatches;

        private TCore core = default!;

        public int ActiveCount => initedSelf.core.ActiveCount;
        public int InactiveCount => initedSelf.core.InactiveCount;
        public int Count => initedSelf.core.Count;

        public bool HasFactory => initedSelf.core.HasFactory;

        protected MonoObjectPoolAsync<T, TCore, TFactory> initedSelf {
            get
            {
                if (core is null)
                    throw new InvalidOperationException($"Mono pool is not inited. Pool: {this}");

                return this;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            core = CreatePool();

            if (preheatCount > 0)
            {
                var preheatOp = new ObjectPoolPreheatOperation<T>(
                    core,
                    preheatCount,
                    batchSize: preheatBatchSize,
                    delayFrameCountBetweenBatches: preheatDelayFrameCountBetweenBatches
                    );

                preheatOp.ExecuteAsync().ForgetByPrintException(destroyCancellationToken);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            core.Dispose();
        }

    #if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (preheatCount > capacity)
                capacity = preheatCount;
        }
    #endif

        public void Clear() => initedSelf.core.Clear();

        public async ValueTask<PooledObject<T>> GetAsync(CancellationToken cancellationToken = default)
        {
            return await initedSelf.core.GetAsync(cancellationToken);
        }

        public void Return(T? obj) => initedSelf.core.Return(obj);

        public bool IsActiveObject(T obj) => initedSelf.core.IsActiveObject(obj);

        public Observable<T> ObserveGet() => initedSelf.core.ObserveGet();

        public Observable<T> ObserveReturn() => initedSelf.core.ObserveReturn();

        protected abstract TCore CreatePool();

        void IDisposable.Dispose() => core.Dispose();
    }
}
