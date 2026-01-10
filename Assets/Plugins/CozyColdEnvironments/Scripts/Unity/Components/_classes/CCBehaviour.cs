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
            this.DoActionAsync(static async @this =>
            {
                await UniTask.Yield(PlayerLoopTiming.PreUpdate, @this.destroyCancellationToken);

                @this.StartPassed = true;
            });
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

        public IDisposable BindDisposable(IDisposable disposable)
        {
            CC.Guard.IsNotNull(disposable, nameof(disposable));

            disposables.Add(disposable);
            return disposable;
        }
    }

    public static class CCBehaviourExtensions
    {
        ///// <summary>
        ///// Disposes when <see cref="MonoBehaviour"/> destroyed
        ///// </summary>
        ///// <returns>self</returns>
        //public static IDisposable AddTo(this IDisposable? source, MonoBehaviour beh)
        //{
        //    if (source.IsNull())
        //        return Disposable.Empty;

        //    CC.Guard.IsNotNull(beh, nameof(beh));

        //    source.AddTo(beh.destroyCancellationToken);

        //    return source;
        //}

        /// <summary>
        /// Disposes when <see cref="CCBehaviour"/> destroyed
        /// </summary>
        /// <returns>self</returns>
        public static IDisposable AddToBehaviour(this IDisposable? source, CCBehaviour beh)
        {
            if (source.IsNull())
                return Disposable.Empty;

            CC.Guard.IsNotNull(beh, nameof(beh));

            return beh.BindDisposable(source);
        }
    }
}