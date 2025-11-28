using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.Diagnostics
{
    public class DebugComponentObserver : CCBehaviour
    {
        private bool state;
        [GetBySelf]
        private Graphic graphic = null!;

        protected override void Awake()
        {
            base.Awake();
            state = graphic.enabled;
        }

        private void Update()
        {
            if (state != graphic.enabled)
            {
                this.PrintError(StackTraceUtility.ExtractStackTrace());
            }

            state = graphic.enabled;
        }
    }
}
