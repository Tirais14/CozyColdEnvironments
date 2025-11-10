using CCEnvs.Diagnostics;
using CCEnvs.Unity.Injections;
using CCEnvs.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.LowLevel;

#nullable enable

#pragma warning disable IDE1006
namespace CCEnvs.Unity.Components
{
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

            onEndFrame += () => StartPassed = true;
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