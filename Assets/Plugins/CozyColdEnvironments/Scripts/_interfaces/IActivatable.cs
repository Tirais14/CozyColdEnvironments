#nullable enable
using System;
using R3;

namespace CCEnvs
{
    public interface IActivatable
    {
        bool IsActive { get; }

        void Activate();

        void Deactivate();

        bool SwitchActiveState();

        Observable<bool> ObserveActivate();

        Observable<bool> ObserveDeactivate();

        Observable<bool> ObserveActiveState();
    }
}
