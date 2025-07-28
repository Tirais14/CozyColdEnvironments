using UTIRLib.GameSystems.Storage;
using UTIRLib.UI.MVVM;

#nullable enable
namespace UTIRLib.UI.ItemStorage
{
    public class ItemSlotViewModel<T> : AViewModel<T>
        where T : IItemSlot
    {
        public ItemSlotViewModel(T model) : base(model)
        {
        }
    }
}
