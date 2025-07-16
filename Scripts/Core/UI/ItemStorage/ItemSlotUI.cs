#nullable enable
using UTIRLib.Diagnostics;

namespace UTIRLib.UI
{
    public class ItemSlotUI : MonoX, IItemSlotUI
    {
        public IItemStackUI ItemStack { get; private set; } = null!;

        protected override void OnAwake()
        {
            var itemStackView = GetComponentInChildren<IView<IItemStackUIViewModel>>();

            if (itemStackView.IsNull())
                throw new ObjectNotFoundException(typeof(IView<IItemStackUIViewModel>));

            var itemStackViewModel = (IViewModel<IItemStackUI>)itemStackView.ViewModel;

            ItemStack = itemStackViewModel.Model;
        }
    }
}
