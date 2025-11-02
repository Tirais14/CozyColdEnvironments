#nullable enable
using System;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface ISelectable
    {
        IReadOnlyReactiveProperty<bool> IsSelected { get; }

        void DoSelect();

        void DoDeselect();

        void SwitchSelectionState();

        IObservable<Unit> ObserveSelect();

        IObservable<Unit> ObserveDeselect();
    }
}
