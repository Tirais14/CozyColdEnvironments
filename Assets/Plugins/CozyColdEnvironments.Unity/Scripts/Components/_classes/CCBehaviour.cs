#if UNITY_2017_1_OR_NEWER
using CCEnvs.Unity.Injections;
using Cysharp.Threading.Tasks;
using R3;
using System;
using UnityEngine;

#nullable enable

#pragma warning disable IDE1006
namespace CCEnvs.Unity.Components
{
    public class CCBehaviour : MonoBehaviour
    {
        private readonly CompositeDisposable disposables = new();

        private Transform m_CTransform = null!;

        private GameObject m_CGameObject = null!;

        private bool cacheInited;

        /// <summary>Cached</summary>
        public Transform cTransform {
            get
            {
                if (!cacheInited)
                    return transform;

                return m_CTransform;
            }
        }
        /// <summary>Cached</summary>
        public GameObject cGameObject {
            get
            {
                if (!cacheInited)
                    return gameObject;

                return m_CGameObject;
            }
        }
        /// <summary>
        /// Is true before update and after start
        /// </summary>
        public bool StartPassed { get; private set; }

        public bool IsDestroyed { get; private set; }

        protected virtual void Awake()
        {
            m_CTransform = transform;
            m_CGameObject = gameObject;

            cacheInited = true;

            //Sets component fields and props marked by specical attribute
            ComponentInjector.Inject(this);
        }

        protected virtual void Start()
        {
            UniTask.Create(this,
                static async @this =>
                {
                    @this.destroyCancellationToken.ThrowIfCancellationRequested();

                    await UniTask.Yield(
                        PlayerLoopTiming.PreUpdate,
                        @this.destroyCancellationToken
                        );

                    @this.StartPassed = true;
                })
                .Forget();
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void OnDestroy()
        {
            disposables.Dispose();
            IsDestroyed = true;
        }

        public T RegisterDisposable<T>(T disposable)
            where T : IDisposable
        {
            CC.Guard.IsNotNull(disposable, nameof(disposable));

            disposables.Add(disposable);
            return disposable;
        }
    }

    public static class CCBehaviourExtensions
    {
        /// <summary>
        /// Disposes when <see cref="CCBehaviour"/> destroyed
        /// </summary>
        /// <returns>self</returns>
        public static T AddDisposableTo<T>(this T source, CCBehaviour beh)
            where T : IDisposable
        {
            return beh.RegisterDisposable(source);
        }
    }
}
#endif