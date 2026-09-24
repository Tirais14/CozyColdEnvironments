using CCEnvs.UnityX.UI;
using ObservableCollections;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Items.UI
{
    public interface IInventoryViewModel
        :
        IViewModel
    {
        IReadOnlyObservableDictionary<IItemContainer, GameObject> ContainerViews { get; }
    }
}
