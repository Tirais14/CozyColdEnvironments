using System;
using UTIRLib.Diagnostics;
using UTIRLib.GameSystems.ItemStorageSystem;

#nullable enable
namespace UTIRLib.GameSystems
{
    public static class ItemSlotHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static int CalculateAddItemCount(IItemSlot slot,
                                                int count)
        {
            if (slot.IsNull())
                throw new ArgumentNullException(nameof(slot));
            if (count < 1)
                throw new ArgumentException($"Count cannot be {count}.");
            if (slot.IsFull)
                return 0;

            int maxItemCount;

            if (slot.HasCapacityLimit)
                maxItemCount = slot.CapacityLimit;
            else
                maxItemCount = slot.ItemStack.MaxItemCount;

            int restCapacity = slot.ItemStack.ItemCount - maxItemCount;

            return Math.Min(restCapacity, count);
        }
    }
}
