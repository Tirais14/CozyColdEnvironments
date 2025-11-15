using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using System;
using UnityEngine;

#nullable enable
#pragma warning disable S2933
namespace CCEnvs.Unity.Items
{
    public abstract class AMaterializedItemContainer : CCBehaviour, IItemContainer
    {
        private IItemContainer itemContainerInternal = null!;

        public Maybe<IItem> Item => itemContainerInternal.Item;
        public int ItemCount => itemContainerInternal.ItemCount;
        public Maybe<IInventory> ParentInventory {
            get => itemContainerInternal.ParentInventory;
            set => itemContainerInternal.ParentInventory = value;
        }
        public bool IsActiveContainer => itemContainerInternal.IsActiveContainer;
        public int Capacity {
            get => itemContainerInternal.Capacity;
            set => itemContainerInternal.Capacity = value;
        }
        public bool IsEmpty => itemContainerInternal.IsEmpty;
        public bool IsFull => itemContainerInternal.IsFull;

        Maybe<GameObject> IGameObjectBindable.gameObject => gameObject; 

        public void SetInternalItemContainer(IItemContainer itemContainer)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            itemContainerInternal = itemContainer;
            OnSetInternalItemContainer();
        }

        public void ActivateContainer()
        {
            itemContainerInternal.ActivateContainer();
        }

        public void Clear()
        {
            itemContainerInternal.Clear();
        }

        public bool ContainsItem()
        {
            return itemContainerInternal.ContainsItem();
        }
        public bool ContainsItem(IItem? item)
        {
            return itemContainerInternal.ContainsItem(item);
        }
        public bool ContainsItem(IItem? item, int count)
        {
            return itemContainerInternal.ContainsItem(item, count);
        }

        public void CopyFrom(IItemContainerInfo itemContainer)
        {
            itemContainerInternal.CopyFrom(itemContainer);
        }

        public void DeactivateContainer()
        {
            itemContainerInternal.DeactivateContainer();
        }

        public Maybe<int> GetContainerID()
        {
            return itemContainerInternal.GetContainerID();
        }

        public IObservable<bool> ObserveIsActiveContainer()
        {
            return itemContainerInternal.ObserveIsActiveContainer();
        }

        public IObservable<Maybe<IItem>> ObserveItem()
        {
            return itemContainerInternal.ObserveItem();
        }

        public IObservable<int> ObserveItemCount()
        {
            return itemContainerInternal.ObserveItemCount();
        }

        public Maybe<IItemContainer> PutItem(IItem? item, int count = 1)
        {
            return itemContainerInternal.PutItem(item, count);
        }

        public Maybe<IItemContainer> PutItemFrom(IItemContainer itemContainer, int count)
        {
            return itemContainerInternal.PutItemFrom(itemContainer, count);
        }

        public Maybe<IItemContainer> PutItemFrom(IItemContainer itemContainer)
        {
            return itemContainerInternal.PutItemFrom(itemContainer);
        }

        public IItemContainer ShallowClone()
        {
            return itemContainerInternal.ShallowClone();
        }

        public bool SwitchContainerActiveState()
        {
            return itemContainerInternal.SwitchContainerActiveState();
        }

        public Maybe<IItemContainer> TakeItem(int count)
        {
            return itemContainerInternal.TakeItem(count);
        }

        public Maybe<IItemContainer> TakeItem()
        {
            return itemContainerInternal.TakeItem();
        }

        public Maybe<IItemContainer> TakeItem(IItem item, int count)
        {
            return itemContainerInternal.TakeItem(item, count);
        }

        protected abstract void OnSetInternalItemContainer();
    }
}
