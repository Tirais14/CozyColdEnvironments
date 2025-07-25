using UTIRLib.Patterns.Factory;

#nullable enable
namespace UTIRLib.UI.StorageSystem.Factories
{
    public class ItemStackFactory : IFactory<IItemStackUIReactive>
    {
        public IItemStackUIReactive Create()
        {
            return new ItemStackUIReactive(int.MaxValue);
        }
    }
}
