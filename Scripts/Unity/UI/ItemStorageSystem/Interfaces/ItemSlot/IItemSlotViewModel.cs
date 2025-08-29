using UnityEngine.EventSystems;
using UTIRLib.GameSystems.ItemStorageSystem;

#nullable enable
namespace UTIRLib.UI
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
