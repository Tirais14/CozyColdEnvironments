#nullable enable
using CCEnvs.Reflection;
using System;

namespace CCEnvs.Unity.Tickables
{
    public interface ITickableBase
    {
        bool IsTickableEnabled { get; set; }

        IDisposable Register(Type tickerType)
        {
            CC.Guard.NullArgument(tickerType, nameof(tickerType));

            IDisposable susbcription = TickablesManager.RegisterTickable(this, tickerType);

            this.AsReflected()
                .Property<IDisposable>(Tickable.TICKER_SUBSCRIPTION_PROPERTY_NAME)
                .SetValue(susbcription);

            return susbcription;
        }
        IDisposable Register()
        {
            return Register(Tickable.GetTickerType(this));
        }

        bool Unregister()
        {
            if (this.AsReflected()
                    .Property<IDisposable>(Tickable.TICKER_SUBSCRIPTION_PROPERTY_NAME)
                    .GetValue() is not IDisposable subscription
                     )
                return TickablesManager.UnregisterTickable(this);

            subscription.Dispose();
            return true;
        }
        bool Unregister(Type fromTickerType)
        {
            CC.Guard.NullArgument(fromTickerType, nameof(fromTickerType));

            return TickablesManager.UnregisterTickable(this, fromTickerType);
        }

        void OnRegister()
        {
        }

        void OnUnregister()
        {
        }
    }
#pragma warning disable S2326
    public interface ITickableBase<in T> : ITickableBase
        where T : ITicker
    {
    }
}
