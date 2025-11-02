using CCEnvs.Unity.Components;
using System;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public class Selectable : CCBehaviour, ISelectable
    {
        private readonly ReactiveProperty<bool> isSelected = new();

        public IReadOnlyReactiveProperty<bool> IsSelected => isSelected;

        public void DoSelect()
        {
            isSelected.Value = true;
        }

        public void DoDeselect()
        {
            isSelected.Value = false;
        }

        public void SwitchSelectionState()
        {
            isSelected.Value = !isSelected.Value;
        }

        public IObservable<Unit> ObserveSelect()
        {
            return isSelected.Where(x => x).AsUnitObservable();
        }

        public IObservable<Unit> ObserveDeselect()
        {
            return isSelected.Where(x => !x).AsUnitObservable();
        }
    }
}
