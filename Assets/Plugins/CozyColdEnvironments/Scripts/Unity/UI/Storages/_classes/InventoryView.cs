using CCEnvs.Language;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.UI.MVVM;
using CCEnvs.Unity.UI.Storages;
using System;
using UniRx;

#nullable enable
namespace CCEnvs.Unity
{
    public class InventoryView<TViewModel, TInventory>
        : View<TViewModel, TInventory>,
        IItemContainerSelectableController

        where TViewModel : ViewModel<TInventory>
        where TInventory : IInventory
    {
        private Subject<Liquid<IItemContainer?>>? selectionSubj;

        public event Action<Liquid<IItemContainer?>>? OnSelectionChanged;

        public Liquid<IItemContainer?> SelectionValue { get; private set; }
        public int SelectionKey { get; private set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            selectionSubj?.Dispose();
        }

        public IObservable<Liquid<IItemContainer?>> ObserveSelection()
        {
            selectionSubj ??= new Subject<Liquid<IItemContainer?>>();

            return selectionSubj;
        }

        public void Select(int key)
        {
            if (SelectionKey == key)
                return;

            SelectionValue = GetModel()[key].ToLiquid()!;
            SelectionKey = key;

            selectionSubj?.OnNext(SelectionValue);
            OnSelectionChanged?.Invoke(SelectionValue);
        }

        public void Deselect(int key)
        {
            SelectionKey = default;
        }

        public void SwitchSelectionState(int key)
        {
            if (SelectionKey == key)
            {
                Deselect(key);
                return;
            }

            Select(key);
        }
    }
}
