#nullable enable
using System;

namespace CCEnvs
{
    public interface IActivatableNode
    {
        bool IsActive { get; }

        void ActivateNode();

        void DeactivateNode();

        bool SwitchNodeActiveState();

        IObservable<bool> ObserveActivateNode();

        IObservable<bool> ObserveDeactivateNode();

        IObservable<bool> ObserveNodeActiveState();
    }
}
