#nullable enable
using System;

namespace CCEnvs
{
    public interface IActivatable
    {
        bool IsActive { get; }

        void Activate();

        void Deactivate();

        bool SwitchActiveState();

        IObservable<bool> ObserveActivate();

        IObservable<bool> ObserveDeactivate();

        IObservable<bool> ObserveActiveState();
    }
}
