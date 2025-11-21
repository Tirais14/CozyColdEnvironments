using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI.MVVM;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public interface IInventoryViewModel<T> 
        : IViewModel<T>, 
        IReactiveDictionaryViewModel<T, int, IItemContainer>

        where T : IInventory
    {
    }
}
