using CCEnvs.Diagnostics;
using CCEnvs.Language;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.UI.MVVM;
using CCEnvs.Unity.UI.Storages;
using System;
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
        private Subject<Ghost<IItemContainer?>>? selectionSubj;

        public event Action<Ghost<IItemContainer?>>? OnSelectionChanged;

        [GetByChildren]
        public ItemContainerViewList SlotList { get; private set; } = null!;

        public Ghost<IItemContainer?> SelectionValue { get; private set; }
        public int SelectionKey { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            //Reparent ItemContainerView
            GetModel().ObserveAdd()
                      .Subscribe(x => x.value.gameObject.Match(
                x => x!.transform.SetParent(transform),
                () => this.PrintError($"Cannot find {nameof(IItemContainer)}.{nameof(IItemContainer.gameObject)}")))
                      .AddTo(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            selectionSubj?.Dispose();
        }

        public IObservable<Ghost<IItemContainer?>> ObserveSelection()
        {
            selectionSubj ??= new Subject<Ghost<IItemContainer?>>();

            return selectionSubj;
        }

        public void Select(int key)
        {
            if (SelectionKey == key)
                return;

            SelectionValue = GetModel()[key].AsGhost()!;
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
