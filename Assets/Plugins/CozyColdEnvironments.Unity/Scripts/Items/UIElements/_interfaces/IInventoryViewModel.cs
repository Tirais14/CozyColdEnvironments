using ObservableCollections;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.Items.UIElements
{
    public interface IInventoryViewModel : UI.IInventoryViewModel
    {
        IReadOnlyObservableDictionary<IItemContainer, VisualElement> ContainerRendererRoots { get; }
    }
}
