#nullable enable
namespace UTIRLib.UI
{
    public class ItemStorageView : View
    {
        protected override void OnAwake()
        {
            base.OnAwake();

            IItemSlotUI[] slots = GetComponentsInChildren<IItemSlotUI>();

            var storage = new ItemStorageUI(slots);

            viewModel = new ItemStorageViewModel(storage);
        }
    }
}
