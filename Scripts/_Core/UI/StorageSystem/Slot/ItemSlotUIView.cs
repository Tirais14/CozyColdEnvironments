using UTIRLib.Diagnostics;
using UTIRLib.UI.ItemSystem;

#nullable enable
namespace UTIRLib.UI.StorageSystem
{
    public class ItemSlotUIView : View
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
