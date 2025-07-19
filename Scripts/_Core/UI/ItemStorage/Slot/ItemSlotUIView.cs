using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.UI
{
    public class ItemSlotUIView : View
    {
        protected override void OnAwake()
        {
            base.OnAwake();

            var itemStackView = GetComponentInChildren<IView<IItemStackUIViewModel>>();

            if (itemStackView.IsNull())
                throw new ObjectNotFoundException(typeof(IView<IItemStackUIViewModel>));

            IItemStackUI itemStack = itemStackView.ViewModel.Model;

            var itemSlot = new ItemSlotUI(itemStack);

            viewModel = new ItemSlotUIViewModel(itemSlot);
        }
    }
}
