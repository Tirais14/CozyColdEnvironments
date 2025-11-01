using CCEnvs.Unity.Storages;
using CCEnvs.Unity.UI.MVVM;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public class InventoryViewModel<T> : ViewModel<T>
        where T : IInventory
    {
        public InventoryViewModel(T model, GameObject gameObject) 
            :
            base(model, gameObject)
        {
        }
    }
}
