#nullable enable
using System;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface ISelectable
    {
        IReadOnlyReactiveProperty<bool> IsSelected { get; }

        void SelectIt();

        void DeselectIt();

        void SwitchSelectionState();

        IObservable<Unit> ObserveSelectOperation();

        IObservable<Unit> ObserveDeselectOperation();
    }
}
