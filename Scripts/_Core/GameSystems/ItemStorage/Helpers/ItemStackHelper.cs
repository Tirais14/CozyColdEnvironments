using System;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.GameSystems.ItemStorageSystem
{
    public static class ItemStackHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static int CalculateTakeItemCount(IItemStack itemStack, int wantedQuantity)
        {
            if (itemStack.IsNull())
                throw new ArgumentNullException(nameof(itemStack));
            if (itemStack.IsEmpty)
                return 0;
            if (wantedQuantity > itemStack.ItemCount)
                return itemStack.ItemCount;

            return wantedQuantity;
        }

        public static int CalulcateAddItemCount(IItemStack itemStack, int toAddQuantity)
        {
            if (itemStack.IsNull())
                throw new ArgumentNullException(nameof(itemStack));
            if (toAddQuantity <= 0)
                throw new ArgumentException($"{nameof(toAddQuantity)} cannot be less than zero.");
            if (itemStack.IsFull)
                return 0;

            int restSize = itemStack.ItemCount - itemStack.MaxItemCount;

            return Math.Min(restSize, toAddQuantity);
        }
    }
}
