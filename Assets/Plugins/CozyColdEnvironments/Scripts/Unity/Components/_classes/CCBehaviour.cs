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

            PreUpdateAction(() =>
            {
                StartPassed = true;
                this.PrintLog("Start Passed");
            });
        }

        public void PreUpdateAction(Action action)
        {
            Guard.IsNotNull(action);

            UniTask.Create(action, static async (action) =>
            {
                await UniTask.Yield(PlayerLoopTiming.PreUpdate);
                action();
            }).AttachExternalCancellation(destroyCancellationToken)
            .SuppressCancellationThrow()
            .Forget();
        }

        public void OnEndFrame(Action action)
        {
            OnEndFrameInternal(action).Forget(ex => this.PrintException(ex));
        }

        private async UniTask OnEndFrameInternal(Action action)
        {
            if (action is null)
            {
                this.PrintError("Action is null.");
                return;
            }

            await UniTask.WaitForEndOfFrame();

            action();
        }
    }
}