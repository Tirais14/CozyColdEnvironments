using System;
using UTIRLib.Diagnostics;

#nullable enable
#pragma warning disable IDE1006
namespace UTIRLib.Tickables
{
    public class ATickableMono
        :
        MonoX,
        ITickableBase
    {
        private ITickerBase? m_Ticker;
        private bool isEnabled;

        protected ITickerBase ticker {
            get
            {
                if (m_Ticker.IsNull())
                    throw new InvalidOperationException("Tickable not registered in any ticker.");

                return m_Ticker;
            }
        }

        public bool IsEnabled {
            get => isEnabled && enabled;
            set
            {
                isEnabled = value;
                enabled = value;
            }
        }

        protected virtual void OnRegisterInternal()
        {
        }

        protected virtual void OnUnregisterInternal()
        {

        }

        void ITickableBase.OnRegister(ITickerBase ticker)
        {
            OnRegisterInternal();
            m_Ticker = ticker;
        }

        void ITickableBase.OnUnregister(ITickerBase ticker)
        {
            OnUnregisterInternal();
            m_Ticker = null;
        }
    }
}
