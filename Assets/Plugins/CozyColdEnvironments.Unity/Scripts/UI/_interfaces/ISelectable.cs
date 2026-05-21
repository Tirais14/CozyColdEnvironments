#nullable enable
using R3;

namespace CCEnvs.UnityX.UI
{
    public interface ISelectable : ISwitchable
    {
        bool IsSelected { get; }

        void DoSelect();

        void DoDeselect();

        void SwitchSelectionState();

        Observable<bool> ObserveIsSelected();

        Observable<ISelectable> ObserveDoSelect();

        Observable<ISelectable> ObserveDoDeselect();
    }
}
