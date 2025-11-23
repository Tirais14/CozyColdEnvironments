using CCEnvs.Diagnostics;
using CCEnvs.Unity.Injections;
using CCEnvs.Utils;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

#nullable enable

#pragma warning disable IDE1006
namespace CCEnvs.Unity.Components
{
    public class CCBehaviour : MonoBehaviour
    {
        /// <summary>Cached</summary>
        public LazyCC<Transform> cTransform { get; private set; } = null!;
        /// <summary>Cached</summary>
        public LazyCC<GameObject> cGameObject { get; private set; } = null!;
        /// <summary>
        /// Is true before update and after start
        /// </summary>
        public bool StartPassed { get; private set; }

        protected virtual void Awake()
        {
            cTransform = new LazyCC<Transform>(() => transform);
            cGameObject = new LazyCC<GameObject>(() => gameObject);

            //Sets component fields and props marked by specical attribute
            ComponentInjector.Inject(this);
        }

        protected virtual void Start()
        {
            MemberValidator.ValidateInstance(this);

            OnPreUpdateAction(() =>
            {
                StartPassed = true;
                this.PrintLog("Start Passed");
            });
        }

        public void OnPreUpdateAction<T>(T state, Action<T> action)
        {
            Guard.IsNotNull(action);

            UniTask.Create((action, @this: this, state), static async (input) =>
            {
                await UniTask.WaitUntil(() => input.@this.didAwake);
                await UniTask.Yield(PlayerLoopTiming.PreUpdate);
                input.action(input.state);
            }).AttachExternalCancellation(destroyCancellationToken)
            .SuppressCancellationThrow()
            .Forget();
        }
        public void OnPreUpdateAction(Action action)
        {
            Guard.IsNotNull(action);

            UniTask.Create((action, @this: this), static async (input) =>
            {
                await UniTask.WaitUntil(() => input.@this.didAwake);
                await UniTask.Yield(PlayerLoopTiming.PreUpdate);
                input.action();
            }).AttachExternalCancellation(destroyCancellationToken)
            .SuppressCancellationThrow()
            .Forget();
        }

        public void OnUpdateAction<T>(T state, Action<T> action)
        {
            Guard.IsNotNull(action);

            UniTask.Create((action, state), static async (input) =>
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
                input.action(input.state);
            }).AttachExternalCancellation(destroyCancellationToken)
            .SuppressCancellationThrow()
            .Forget();
        }
        public void OnUpdateAction(Action action)
        {
            Guard.IsNotNull(action);

            UniTask.Create(action, static async action =>
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
                action();
            }).AttachExternalCancellation(destroyCancellationToken)
            .SuppressCancellationThrow()
            .Forget();
        }

        public void OnEndOfFrameAction<T>(T state, Action<T> action)
        {
            Guard.IsNotNull(action);

            UniTask.Create((action, state), static async input =>
            {
                await UniTask.WaitForEndOfFrame();
                input.action(input.state);
            }).AttachExternalCancellation(destroyCancellationToken)
            .SuppressCancellationThrow()
            .Forget();
        }
        public void OnEndOfFrameAction(Action action)
        {
            Guard.IsNotNull(action);

            UniTask.Create(action, static async action =>
            {
                await UniTask.WaitForEndOfFrame();
                action();
            }).AttachExternalCancellation(destroyCancellationToken)
            .SuppressCancellationThrow()
            .Forget();
        }
    }
}