using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public interface IInventoryViewModel
        :
        IViewModel,
        IReactiveDictionaryViewModel<int, IItemContainer>
    {
    }
}
