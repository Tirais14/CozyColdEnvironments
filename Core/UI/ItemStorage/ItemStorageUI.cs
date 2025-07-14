using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
namespace UTIRLib.UI
{
    public class ItemStorageUI : MonoX, IItemStorageUI
    {
        protected IItemSlotUI[] slots = null!;

        [SerializeField]
        protected GameObject slotPrefab = null!;

        [SerializeField]
        protected int slotCount;

        public int SlotCount => slotCount;
        public IItemSlotUI this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => slots[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IItemSlotUI GetItemStack(int index) => slots[index];

        protected override void OnAwake()
        {
            base.OnAwake();

            slots = GetComponentsInChildren<IItemSlotUI>();

            if (slots.IsEmpty())
                throw new System.Exception("Cannot find any slot in childrens.");
        }
    }
}
