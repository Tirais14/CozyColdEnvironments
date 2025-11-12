#nullable enable
using System;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface ISelectable 
    {
        bool IsEnabled { get; set; }
        bool IsSelected { get; }

        void DoSelect();

        void DoDeselect();

        void SwitchSelectionState();

        IObservable<bool> ObserveIsSelected();
    }
}
