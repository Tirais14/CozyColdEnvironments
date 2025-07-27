using UTIRLib.GameSystems.Storage;

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
