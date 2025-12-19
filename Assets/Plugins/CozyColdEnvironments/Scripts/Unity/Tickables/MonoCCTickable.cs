using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using System;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.Tickables
{
    public class MonoCCTickable
        :
        CCBehaviour,
        ITickableBase
    {
        private ITicker? ticker;
        private bool isTickableEnabled;

        protected ITicker Ticker {
            get
            {
                if (ticker.IsNull())
                    throw new ArgumentException("Tickable not registered in any ticker.");

                return ticker;
            }
        }

        public bool IsTickableEnabled {
            get => didStart && isTickableEnabled && enabled;
            set
            {
                isTickableEnabled = value;
                enabled = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            TickablesManager.RegisterTickable(this);
        }

        protected virtual void OnRegisterInternal()
        {
        }

        protected virtual void OnUnregisterInternal()
        {
        }

        void ITickableBase.OnRegister() => OnRegisterInternal();

        void ITickableBase.OnUnregister() => OnUnregisterInternal();
    }
}
