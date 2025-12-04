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

        IObservable<ISelectable> ObserveDoSelect();

        IObservable<ISelectable> ObserveDoDeselect();
    }
}
