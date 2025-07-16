using System;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.UI
{
    public static class ItemStackUIHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static int CalculateToTakeCount(IItemStackUI itemStack, int wantedQuantity)
        {
            if (itemStack.IsNull())
                throw new ArgumentNullException(nameof(itemStack));
            if (itemStack.IsEmpty)
                return 0;
            if (wantedQuantity > itemStack.ItemCount)
                return itemStack.ItemCount;

            return wantedQuantity;
        }

        public static int CalulcateToAddCount(IItemStackUI itemStack, int toAddQuantity)
        {
            if (itemStack.IsNull())
                throw new ArgumentNullException(nameof(itemStack));
            if (toAddQuantity <= 0)
                throw new ArgumentException($"{nameof(toAddQuantity)} cannot be less than zero.");
            if (itemStack.IsFull)
                return 0;
            if (toAddQuantity > itemStack.Item.MaxStackCount)
                return toAddQuantity;

            return toAddQuantity;
        }
    }
}
