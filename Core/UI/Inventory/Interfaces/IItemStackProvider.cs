#nullable enable

namespace UTIRLib.UI
{
    public interface IItemStackProvider : IReadOnlyItemStackProvider
    {
        new IItemStack ItemStack { get; }
    }
}