using CCEnvs.Diagnostics;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity
{
    public abstract class ALoadingManager : CCBehaviourStatic
    {
        private TimeSpan delayTimer;

        protected abstract TimeSpan DelayBeforeReady { get; }

        protected override void OnAwake()
        {
            base.OnAwake();

            this.PrintLog("Awaked");
        }

        protected abstract bool IsReady();

        protected abstract void OnReady();

        private void LateUpdate()
        {
            if (!IsReady())
                return;

            if (delayTimer >= DelayBeforeReady)
            {
                OnReady();
                Destroy(gameObject);
            }

            delayTimer += TimeSpan.FromSeconds(Time.deltaTime);
        }
    }
}
