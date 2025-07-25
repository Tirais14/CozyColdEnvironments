#nullable enable
using UTIRLib.Unity.Extensions;

namespace UTIRLib.UI.StorageSystem
{
    public class ItemStorageView<TViewModel, TStorage> : AView<TViewModel>
        where TViewModel : IViewModel<TStorage>
        where TStorage : IItemStorageUI
    {
        protected override void OnAwake()
        {
            base.OnAwake();

            IItemSlotUI[] slots = this.GetAssignedObjectsInChildren<IItemSlotUI>();


            viewModel = (TViewModel)StorageSystemServiceLocator.ItemStorageViewModelFactory.Create(slots);
        }
    }
}
