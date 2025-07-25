using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.UI.StorageSystem
{
    public class ItemSlotUIView : AView
    {
        protected override void OnAwake()
        {
            base.OnAwake();

            var itemStackView = GetComponentInChildren<IView<IItemStackUIViewModel>>();

            if (itemStackView.IsNull())
                throw new ObjectNotFoundException(typeof(IView));

            IItemStackUI itemStack = itemStackView.GetViewModel().GetModel();

            var itemSlot = new ItemSlotUI(itemStack);

            viewModel = new ItemSlotUIViewModel(itemSlot);
        }
    }
}
