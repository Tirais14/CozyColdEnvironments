using CCEnvs.Unity.UI.MVVM;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public class InventoryViewModel<T> : ViewModel<T>
        where T : IInventory
    {
        public InventoryViewModel(T model) : base(model)
        {
        }
    }
}
