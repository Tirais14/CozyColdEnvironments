#nullable enable
using System;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface ISelectable : ISwitchable
    {
        bool IsSelected { get; }

        void DoSelect();

        void DoDeselect();

        void SwitchSelectionState();

        IObservable<bool> ObserveIsSelected();

        IObservable<Unit> ObserveDoSelect();

        IObservable<Unit> ObserveDoDeselect();
    }
}
