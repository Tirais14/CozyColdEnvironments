using UTIRLib.Patterns.Factory;

#nullable enable
namespace UTIRLib.UI.StorageSystem.Factories
{
    public class ItemStackViewModelFactory : IFactory<IItemStackUIViewModel>
    {
        public IItemStackUIViewModel Create()
        {
            IItemStackUIReactive stack = StorageSystemServiceLocator.ItemStackFactory.Create();

            return new ItemStackUIViewModel(stack);
        }
    }
}
