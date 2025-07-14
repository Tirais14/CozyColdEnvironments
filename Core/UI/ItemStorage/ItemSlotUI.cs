using UTIRLib.Injector;

#nullable enable
namespace UTIRLib.UI
{
    public class ItemSlotUI : MonoX, IItemSlotUI
    {
        [GetComponentInChildrenIfNull]
        public IItemStackUI ItemStack { get; private set; } = null!;
    }
}
