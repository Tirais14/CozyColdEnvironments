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

        /// <summary>Cached</summary>
        public Lazy<Transform> cTransform { get; private set; } = null!;
        /// <summary>Cached</summary>
        public Lazy<GameObject> cGameObject { get; private set; } = null!;
        /// <summary>
        /// Is true before update and after start
        /// </summary>
        public bool StartPassed { get; private set; }
        public bool IsDestroyed { get; private set; }

        protected virtual void Awake()
        {
            cTransform = new Lazy<Transform>(() => transform);
            cGameObject = new Lazy<GameObject>(() => gameObject);

            //Sets component fields and props marked by specical attribute
            ComponentInjector.Inject(this);
        }

        protected virtual void Start()
        {
            UniTask.Create(this,
                static async @this =>
                {
                    await UniTask.Yield(PlayerLoopTiming.PreUpdate, @this.destroyCancellationToken);

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

        public IDisposable RegisterDisposable(IDisposable disposable)
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
        public static IDisposable RegisterDisposableTo(this IDisposable? source, CCBehaviour beh)
        {
            if (source.IsNull())
                return Disposable.Empty;

            CC.Guard.IsNotNull(beh, nameof(beh));

            return beh.RegisterDisposable(source);
        }
    }
}
#endif