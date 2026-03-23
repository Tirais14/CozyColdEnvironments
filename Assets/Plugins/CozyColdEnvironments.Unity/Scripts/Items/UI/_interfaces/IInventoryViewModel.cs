using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using ObservableCollections;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public interface IInventoryViewModel
        :
        IViewModel
    {
        IReadOnlyObservableDictionary<int, IItemContainer> Containers { get; }
    }
}
