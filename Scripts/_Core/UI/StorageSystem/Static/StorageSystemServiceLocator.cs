using UTIRLib.Patterns.Factory;
using UTIRLib.UI.StorageSystem.Factories;

#nullable enable
namespace UTIRLib.UI.StorageSystem
{
    public static class StorageSystemServiceLocator
    {
        public static IFactory<IItemStackUIReactive> ItemStackFactory { get; set; } = new ItemStackFactory();
        public static IFactory<IItemStackUIViewModel> ItemStackViewModelFactory { get; set; } = new ItemStackViewModelFactory();
    }
}
