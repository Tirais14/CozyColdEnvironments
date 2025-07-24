#nullable enable
using UTIRLib.UI.ItemSystem;

namespace UTIRLib.UI.StorageSystem
{
    public interface IItemSlotUI
    {
        IItemStackUI ItemStack { get; }
    }
}
