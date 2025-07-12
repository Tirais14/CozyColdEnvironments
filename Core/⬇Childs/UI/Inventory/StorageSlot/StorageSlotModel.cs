using UnityEngine;
using UTIRLib.Injector;

#nullable enable

namespace UTIRLib.UI
{
    public class StorageSlotModel : MonoX, IStorageSlot
    {
        [SerializeField]
        [GetComponentInChildrenIfNull]
        protected ItemStackModelBase itemStack = null!;

        public IItemStack ItemStack => itemStack;
        public int Quantity => itemStack.Quantity;
        public bool IsEmpty => itemStack.IsEmpty;
        public bool IsFull => itemStack.IsFull;

        IReadOnlyItemStack IReadOnlyItemStackProvider.ItemStack => ItemStack;

        public IItemStack Put(IItem item, int quantity = 1) => itemStack.Put(item, quantity);

        public void Put(IItemStack itemStack) => itemStack.Put(itemStack);

        public IItemStack Take(int quantity = 1) => itemStack.Take(quantity);

        public IItemStack TakeAll() => itemStack.TakeAll();

        public bool Contains(IItem item) => ItemStack.Item == item;

        public void Clear() => itemStack.Clear();
    }
}