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

        public void SelectIt()
        {
            isSelected.Value = true;
        }

        public void DeselectIt()
        {
            isSelected.Value = false;
        }

        public void SwitchSelectionState()
        {
            isSelected.Value = !isSelected.Value;
        }

        public IObservable<Unit> ObserveSelectOperation()
        {
            return isSelected.Where(x => x).AsUnitObservable();
        }

        public IObservable<Unit> ObserveDeselectOperation()
        {
            return isSelected.Where(x => !x).AsUnitObservable();
        }
    }
}
