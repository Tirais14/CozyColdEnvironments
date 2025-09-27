using CCEnvs.Diagnostics;
using CCEnvs.Unity.Injections;
using CCEnvs.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable

namespace CCEnvs.Unity.Components
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class CCBehaviour : MonoBehaviour
    {
        private SingleUseAction? onEndFrame;
        private bool onEndFrameAsyncStarted;

        public event SingleUseAction? OnEndFrame {
            add
            {
                onEndFrame += value;

                if (!onEndFrameAsyncStarted && onEndFrame is not null)
                    OnEndFrameAsync().TimeoutWithoutException(TimeSpan.FromSeconds(30))
                                     .Forget(ex => this.PrintException(ex));
            }
            remove => onEndFrame -= value;
        }

        /// <summary>Cached</summary>
        public LazyCC<Transform> cTransform { get; private set; } = null!;
        /// <summary>Cached</summary>
        public LazyCC<GameObject> cGameObject { get; private set; } = null!;

        protected virtual void OnAwake()
        {

        }

        protected virtual void OnStart()
        {
        }

        protected void Awake()
        {
            cTransform = new LazyCC<Transform>(() => transform);
            cGameObject = new LazyCC<GameObject>(() => gameObject);

            //Sets component fields and props marked by specical attribute
            ComponentInjector.Inject(this);

            OnAwake();
        }

        protected void Start()
        {
            OnStart();
            //Checks field and props marked by RequiredAttribute
            MemberValidator.ValidateInstance(this);
        }

        private async UniTask OnEndFrameAsync()
        {
            onEndFrameAsyncStarted = true;

            await UniTask.WaitForEndOfFrame();

            onEndFrame!();

            onEndFrameAsyncStarted = false;
        }
    }
}