using UnityEngine.EventSystems;
using CozyColdEnvironments.GameSystems.ItemStorageSystem;

#nullable enable
namespace CozyColdEnvironments.UI
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
