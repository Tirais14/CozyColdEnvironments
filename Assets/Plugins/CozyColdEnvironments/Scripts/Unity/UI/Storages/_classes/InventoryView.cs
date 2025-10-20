using CCEnvs.Unity.UI.MVVM;

namespace CCEnvs.Unity
{
    public class InventoryView<TViewModel, TInventory> : View<TViewModel, TInventory>
        where TViewModel : ViewModel<TInventory>
        where TInventory : IInventory
    {
    }
}
