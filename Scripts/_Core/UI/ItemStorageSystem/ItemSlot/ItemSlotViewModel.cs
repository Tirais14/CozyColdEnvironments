using UTIRLib.GameSystems.ItemStorageSystem;
using UTIRLib.UI.MVVM;

#nullable enable
namespace UTIRLib.UI.ItemStorageSystem
{
    public class ItemSlotViewModel<T> : AViewModel<T>
        where T : IItemSlot
    {
        public ItemSlotViewModel(T model) : base(model)
        {
        }
    }
}
