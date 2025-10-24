using CCEnvs.Diagnostics;
using CCEnvs.Language;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.UI;
using CCEnvs.Unity.UI.Elements;
using CCEnvs.Unity.UI.MVVM;
using CCEnvs.Unity.UI.Storages;
using System;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public abstract class InventoryView<TViewModel, TInventory>
        : View<TViewModel, TInventory>,
        IItemContainerSelectableController

        where TViewModel : ViewModel<TInventory>
        where TInventory : IInventory
    {
        private Subject<Ghost<IItemContainer>>? selectionSubj;

        public event Action<Ghost<IItemContainer>>? OnSelectionChanged;

        [field: GetByChildren]
        public GameObjectBag SlotBag { get; private set; } = null!;

        public Ghost<IItemContainer> SelectionValue { get; private set; }
        public int SelectionKey { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            model.ObserveAdd()
                 .Subscribe(AddToList)
                 .AddTo(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            selectionSubj?.Dispose();
        }

        private void AddToList((int id, IItemContainer value) pair)
        {
            pair.value.gameObject.Match(
                x => SlotBag.Add(x!),
                () => this.PrintError($"Cannot find {nameof(IItemContainer)}.{nameof(IItemContainer.gameObject)}")
                );
        }

        public IObservable<Ghost<IItemContainer>> ObserveSelection()
        {
            selectionSubj ??= new Subject<Ghost<IItemContainer>>();

            return selectionSubj;
        }

        public void Select(int key)
        {
            if (SelectionKey == key)
                return;

            SelectionValue = model[key].ToGhost()!;
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
    public class InventoryView : InventoryView<InventoryViewModel<Inventory>, Inventory>
    {
    }
}
