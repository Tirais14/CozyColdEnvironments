using System;
using CCEnvs.Diagnostics;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.Tickables
{
    public class MonoCCTickable
        :
        MonoCC,
        ITickableBase
    {
        private ITicker? ticker;
        private bool isTickableEnabled;

        protected ITicker Ticker {
            get
            {
                if (ticker.IsNull())
                    throw new DataAccessException("Tickable not registered in any ticker.");

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

        protected override void OnAwake()
        {
            base.OnAwake();
            TickablesCore.RegisterTickable(this);
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
