using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UniRx;
using UnityEngine;
using UTIRLib.Attributes;
using UTIRLib.Diagnostics;

#nullable enable

namespace UTIRLib.UI
{
    public class StorageModel : MonoX, IStorage
    {
#if UNITY_EDITOR

        [Min(0)]
        [SerializeField]
        protected int slotQuantityInternal;

#endif

        protected readonly ReactiveCollection<StorageSlotModel> slotsProp = new();
        protected readonly ReactiveProperty<bool> isOpenedProp = new();
        protected IItemStack[]? savedItems;

        [SerializeField]
        [RequiredMember]
        protected GameObject slotPrefab = null!;

        public IReadOnlyReactiveCollection<StorageSlotModel> StorageSlotsProp => slotsProp;
        public IReadOnlyReactiveProperty<bool> IsOpenedProp => isOpenedProp;
        public bool IsOpened => isOpenedProp.Value;

        public int SlotQuantity {
            get => slotsProp.Count;
            set => SetSlotQuantity(value);
        }

        public IStorageSlot this[int id] => GetSlot(id);

        int IReadOnlyCollection<IStorageSlot>.Count => SlotQuantity;

        protected override void OnStart()
        {
            base.OnStart();
            RebuildSlots(slotQuantityInternal);
        }

        public virtual void Open() => isOpenedProp.Value = true;

        public virtual void Close() => isOpenedProp.Value = false;

        public IItemStack AddItem(IItem item, int quantity)
        {
            if (item.IsNull())
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (quantity < 1)
            {
                return ReadOnlyItemStack.empty;
            }

            IStorageSlot? slot = null;
            IItemStack? remainingItems = null;
            LoopPredicate cyclePredicate = new(() => slot != null);
            do
            {
                if (!TryGetSuitableSlot(item, out slot))
                {
                    slot = GetEmptySlot();

                    if (slot.IsNull()) break;
                }

                remainingItems = slot.Put(item, quantity);

                if (remainingItems.IsNotNull() && remainingItems.IsNotEmpty)
                {
                    item = remainingItems.Item!;
                    quantity = remainingItems.Quantity;
                }
            }
            while (cyclePredicate.Invoke());

            return remainingItems.IsNull() ? ReadOnlyItemStack.empty : remainingItems;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void AddItemStack(IItemStack itemStack)
        {
            if (itemStack.IsNull())
            {
                throw new ArgumentNullException(nameof(itemStack));
            }
            if (itemStack.IsEmpty)
            {
                return;
            }

            IItemStack takedItems = itemStack.TakeAll();

            AddItem(takedItems.Item!, takedItems.Quantity);

            itemStack.Put(itemStack); //return remaining items to source stack
        }

        public IStorageSlot GetSlot(int id) => slotsProp[id];

        public IStorageSlot? GetEmptySlot()
        {
            for (int i = 0; i < slotsProp.Count; i++)
            {
                if (slotsProp[i].IsEmpty)
                {
                    return slotsProp[i];
                }
            }

            return null;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public IStorageSlot? GetSuitableSlot(IItem item)
        {
            if (item.IsNull())
            {
                throw new ArgumentNullException(nameof(item));
            }

            IStorageSlot slot;
            for (int i = 0; i < slotsProp.Count; i++)
            {
                slot = slotsProp[i];
                if (!slot.IsEmpty && slot.Contains(item) && !slot.IsFull)
                {
                    return slot;
                }
            }

            return null;
        }

        public bool TryGetSuitableSlot(IItem item, [NotNullWhen(true)] out IStorageSlot? slot)
        {
            slot = GetSuitableSlot(item);

            return slot.IsNotNull();
        }

        public void SetSlotQuantity(int quantity)
        {
            quantity = quantity < 0 ? 0 : quantity;

            if (quantity == SlotQuantity)
            {
                return;
            }

            RebuildSlots(quantity);
        }

        public void AddSlots(int quantity = 1)
        {
            quantity = quantity < 0 ? 0 : quantity;

            if (quantity == 0)
            {
                return;
            }

            RebuildSlots(SlotQuantity + quantity);
        }

        public void RemoveSlots(int quantity = 1)
        {
            quantity = quantity < 0 ? 0 : quantity;

            if (quantity == 0)
            {
                return;
            }

            RebuildSlots(SlotQuantity - quantity);
        }

        /// <summary>
        /// Deletes slots
        /// </summary>
        public void Clear()
        {
            foreach (var slot in slotsProp.ToList())
            {
                Destroy(slot.gameObject);
                slotsProp.Remove(slot);
            }
        }

        /// <summary>
        /// Deletes all items, not slots
        /// </summary>
        public void ClearSlots()
        {
            for (int i = 0; i < slotsProp.Count; i++)
            {
                slotsProp[i].Clear();
            }
        }

        public IEnumerator<IStorageSlot> GetEnumerator() => slotsProp.GetEnumerator();

        protected void RestoreItemsFromSaved()
        {
            if (savedItems.IsNullOrEmpty()) return;

            int count = Math.Min(savedItems.Length, slotsProp.Count);

            int itemStacksAdded = 0;
            for (int i = 0; i < count; i++)
            {
                slotsProp[i].Put(savedItems[i]);
            }

            if (itemStacksAdded < savedItems.Length)
            {
                int remainingItemStackCount = savedItems.Length - itemStacksAdded;
                var remainingItemStacks = new IItemStack[remainingItemStackCount];

                Array.Copy(savedItems, remainingItemStackCount - 1, remainingItemStacks, 0, remainingItemStackCount);

                savedItems = remainingItemStacks;
            }
            else savedItems = null;
        }

        protected virtual void RebuildSlots(int newSlotQuantity)
        {
            SaveStoredItems();

            Clear();
            CreateSlots(newSlotQuantity);

            RestoreItemsFromSaved();
        }

        protected virtual StorageSlotModel CreateSlot() =>
            Instantiate(slotPrefab, transform).
            GetComponent<StorageSlotModel>().
            ThrowIfNotFound();

        protected void SaveStoredItems()
        {
            List<IItemStack> itemStacks = new(slotsProp.Count);
            for (int i = 0; i < slotsProp.Count; i++)
            {
                if (!slotsProp[i].IsEmpty)
                {
                    itemStacks.Add(slotsProp[i].TakeAll());
                }
            }

            savedItems = itemStacks.ToArray();
        }

        private void CreateSlots(int quantity)
        {
            if (slotPrefab == null)
            {
                throw new NullReferenceException("Slot prefab not setted.");
            }

            for (int i = 0; i < quantity; i++)
            {
                slotsProp.Add(CreateSlot());
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (didStart && Application.isPlaying)
            {
                SlotQuantity = slotQuantityInternal;
            }
        }

#endif

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}