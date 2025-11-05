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
            CC.Guard.IsNotNull(tickerType, nameof(tickerType));

            IDisposable susbcription = TickablesManager.RegisterTickable(this, tickerType);

            this.Reflect()
                .NonPublic()
                .Name(Tickable.TICKER_SUBSCRIPTION_PROPERTY_NAME)
                .Property()
                .Strict()
                .SetValue(this, susbcription);

            return susbcription;
        }
        IDisposable Register()
        {
            return Register(Tickable.GetTickerType(this));
        }

        bool Unregister()
        {
            if (this.Reflect()
                .NonPublic()
                .Name(Tickable.TICKER_SUBSCRIPTION_PROPERTY_NAME)
                .Property()
                .Strict()
                .GetValue(this) is not IDisposable subscription
                )
                return TickablesManager.UnregisterTickable(this);

            subscription.Dispose();
            return true;
        }
        bool Unregister(Type fromTickerType)
        {
            CC.Guard.IsNotNull(fromTickerType, nameof(fromTickerType));

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
