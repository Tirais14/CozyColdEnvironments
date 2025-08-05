using System;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.GameSystems.ItemStorageSystem
{
    public static class ItemContainerHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static int CalculateTakeItemCount(IItemContainerInfo itemCointainer,
                                                 int takeCount)
        {
            if (itemCointainer.IsNull())
                throw new ArgumentNullException(nameof(itemCointainer));
            if (!itemCointainer.HasItem)
                return 0;
            if (takeCount > itemCointainer.ItemCount)
                return itemCointainer.ItemCount;

            return takeCount;
        }

        public static int CalulcateAddItemCount(IItemContainerInfo itemCointainer,
                                                int addCount)
        {
            if (itemCointainer.IsNull())
                throw new ArgumentNullException(nameof(itemCointainer));
            if (addCount <= 0)
                throw new ArgumentException($"{nameof(addCount)} < 0.");
            if (itemCointainer.IsContainerFull)
                return 0;

            int restSize = itemCointainer.MaxItemCount - itemCointainer.ItemCount;

            return Math.Min(restSize, addCount);
        }
    }
}
