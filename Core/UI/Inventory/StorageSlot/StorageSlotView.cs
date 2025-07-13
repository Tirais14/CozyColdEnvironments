#nullable enable

using UnityEngine;

namespace UTIRLib.UI
{
    [RequireComponent(typeof(StorageSlotViewModel))]
    public class StorageSlotView : View<StorageSlotViewModel>, IDragInteractable<IItemStack>
    {
        public void Drag(IItemStack itemStack) => viewModel.DragItemStack(itemStack);

        protected override void BindToViewModel()
        { }
    }
}