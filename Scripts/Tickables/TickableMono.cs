using System;
using UTIRLib.Diagnostics;

#nullable enable
#pragma warning disable IDE1006
namespace UTIRLib.Tickables
{
    public class TickableMono
        :
        MonoX,
        ITickableBase
    {
        private ITicker? ticker;
        private bool isTickableEnabled;

        protected ITicker Ticker {
            get
            {
                if (ticker.IsNull())
                    throw new NullReferenceException("Tickable not registered in any ticker.");

                return ticker;
            }
        }

        public bool IsTickableEnabled {
            get => isTickableEnabled && enabled;
            set
            {
                isTickableEnabled = value;
                enabled = value;
            }
        }

        protected virtual void OnRegisterInternal()
        {
        }

        protected virtual void OnUnregisterInternal()
        {

        }

        void ITickableBase.OnRegister()
        {
            OnRegisterInternal();
            ticker = Ticker;
        }

        void ITickableBase.OnUnregister()
        {
            OnUnregisterInternal();
            ticker = null;
        }
    }
}
