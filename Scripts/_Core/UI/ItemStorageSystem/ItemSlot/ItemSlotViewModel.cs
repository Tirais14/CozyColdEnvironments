using System;
using UnityEngine.EventSystems;
using UTIRLib.GameSystems.ItemStorageSystem;
using UTIRLib.UI.MVVM;
using UTIRLib.Unity.TypeMatching;

#nullable enable
namespace UTIRLib.UI.ItemStorageSystem
{
    public class ItemSlotViewModel<T> : AViewModel<T>, IItemSlotViewModel<T>
        where T : IItemSlot
    {
        public ItemSlotViewModel(T model) : base(model)
        {
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void OnViewDrop(PointerEventData eventData)
        {
            if (eventData is null)
                throw new ArgumentNullException(nameof(eventData));

            if (eventData.pointerDrag.GetAssignedModel<IItemStack>()
                .IsNot<IItemStack>(out var itemStack)
                )
                return;

            if (!itemStack.HasItem)
                return;

            if (model.HasItem && !model.IsSameItem(itemStack.Item))
                return;

            if (model.Equals(itemStack))
                return;

            //TODO: Replace this to swap stacks
            if (model.IsContainerFull)
                return;

            model.AddItemFrom(itemStack, itemStack.ItemCount);
        }
    }
}
