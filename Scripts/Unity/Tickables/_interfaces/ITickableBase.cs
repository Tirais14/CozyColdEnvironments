#nullable enable
using System;

namespace CCEnvs.Unity.Tickables
{
    public interface ITickableBase
    {
        bool IsTickableEnabled { get; set; }

        IDisposable RegisterBy(Type tickerType)
        {
            TickablesManager.Re
        }

        void UnregisterBy(Type tickerType);

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
