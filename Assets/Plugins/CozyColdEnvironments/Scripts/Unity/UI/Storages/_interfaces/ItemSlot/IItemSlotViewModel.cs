using CCEnvs.Unity.GameSystems.Storages;
using UnityEngine.EventSystems;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public interface IItemSlotViewModel
        :
        IViewModel,
        IDropHandler
    {
    }
    public interface IItemSlotViewModel<T> 
        :
        IItemSlotViewModel,
        IViewModel<T>

        where T : IItemSlot
    {
    }
}
