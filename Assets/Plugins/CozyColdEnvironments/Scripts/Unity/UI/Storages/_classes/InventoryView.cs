using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.UI;
using CCEnvs.Unity.UI.Elements;
using CCEnvs.Unity.UI.MVVM;
using CCEnvs.Unity.UI.Storages;
using System;
using System.Collections.Generic;
using UniRx;

#nullable enable
namespace CCEnvs.Unity
{
    public abstract class InventoryView<TViewModel, TInventory>
        : View<TViewModel, TInventory>,
        IItemContainerSelectableController

        where TViewModel : ViewModel<TInventory>
        where TInventory : IInventory
    {
        private readonly Lazy<Subject<SelectionChangedEvent<int, IItemContainer>>> selectionSubj = new(() => new Subject<SelectionChangedEvent<int, IItemContainer>>());
        private SelectionController<int, IItemContainer> selectableController;

        public event Action<SelectionChangedEvent<int, IItemContainer>>? OnSelectionChanged;

        [field: GetByChildren]
        public GameObjectBag SlotBag { get; private set; } = null!;

        public IReadOnlyReactiveProperty<KeyValuePair<int, Maybe<IItemContainer>>> Selection => selectableController.Selection;

        protected override void Awake()
        {
            base.Awake();

            selectableController = new SelectionController<int, IItemContainer>((key) => model[key]);
            selectableController.AddTo(this);

            model.ObserveAdd()
                 .Subscribe(AddToList)
                 .AddTo(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            selectionSubj.Value.Dispose();
        }

        private void AddToList((int id, IItemContainer value) pair)
        {
            pair.value.gameObject.Match(
                x => SlotBag.Add(x!),
                () => this.PrintError($"Cannot find {nameof(IItemContainer)}.{nameof(IItemContainer.gameObject)}")
                );
        }

        public IObservable<SelectionChangedEvent<int, IItemContainer>> ObserveSelection()
        {
            return selectionSubj.Value;
        }

        public void SelectIt(int key)
        {
            selectableController.SelectIt(key);
        }

        public void DeselectIt(int key)
        {
            selectableController.DeselectIt(key);
        }

        public void SwitchSelectionState(int key)
        {
            selectableController.SwitchSelectionState(key);
        }
    }
    public class InventoryView : InventoryView<InventoryViewModel<Inventory>, Inventory>
    {
    }
}
