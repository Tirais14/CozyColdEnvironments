using System;
using UnityEngine.EventSystems;
using CozyColdEnvironments.GameSystems;
using CozyColdEnvironments.GameSystems.ItemStorageSystem;
using CozyColdEnvironments.UI.MVVM;
using CozyColdEnvironments.Unity.TypeMatching;

#nullable enable
namespace CozyColdEnvironments.UI.ItemStorageSystem
{
    public class ItemSlotViewModel<T> 
        :
        AViewModel<T>,
        IItemSlotViewModel<T>

        where T : IItemSlot, IItemContainerReactive
    {
        public ItemSlotViewModel(T model) : base(model)
        {
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData is null)
                throw new ArgumentNullException(nameof(eventData));

            if (eventData.pointerDrag.GetAssignedModel<IItemContainer>()
                .IsNot<IItemContainer>(out var itemContainer)
                )
                return;

            if (!itemContainer.HasItem)
                return;

            if (model.HasItem && !model.IsSameItem(itemContainer.Item))
                return;

            if (model.Equals(itemContainer))
                return;

            //TODO: Replace this to swap stacks
            if (model.IsContainerFull)
                return;

            model.AddItemFrom(itemContainer);
        }
    }
}
