using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.Components;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.UnityX.Diagnostics
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
