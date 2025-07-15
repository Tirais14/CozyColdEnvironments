using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.UI
{
    public class ItemStorageUI : MonoX, IItemStorageUI
    {
        protected readonly List<IItemSlotUI> slots = new();

        public int SlotCount => slots.Count;
        public IItemSlotUI this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => slots[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IItemSlotUI GetItemSlot(int index) => slots[index];

        protected override void OnStart()
        {
            base.OnStart();

            TryInitSlots();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void AddItemSlot(IItemSlotUI itemSlot)
        {
            if (itemSlot.IsNull())
                throw new ArgumentNullException(nameof(itemSlot));

            TryReParentItemSlot(itemSlot);

            slots.Add(itemSlot);
        }

        public void RemoveItemSlotAt(int index)
        {
            slots.RemoveAt(index);
        }

        private void TryInitSlots()
        {
            var foundSlots = GetComponentsInChildren<IItemSlotUI>();

            if (slots.IsNotEmpty())
                slots.AddRange(foundSlots);
        }

        private void TryReParentItemSlot(IItemSlotUI itemSlot)
        {
            if (itemSlot is Component component)
                component.transform.parent = transform;
        }
    }
}
