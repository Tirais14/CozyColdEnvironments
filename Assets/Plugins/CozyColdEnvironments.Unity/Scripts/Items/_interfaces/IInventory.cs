#nullable enable
using ObservableCollections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs.Unity.Items
{
    public interface IInventory
        : IItemAccessor,
        IItemContainerInfoItemless,
        IEnumerable<KeyValuePair<int, IItemContainer>>
    {
        bool AutoSize { get; set; }

        IEnumerable<int> IDs { get; }
        IEnumerable<IItemContainer> Containers { get; }

        IReadOnlyObservableDictionary<int, IItemContainer> ContainersRX { get; }

        int ContainerCount { get; }

        IItemContainer this[int id] { get; }

        bool TryGetContainer(int id, [NotNullWhen(true)] out IItemContainer? container);

        void ResetContainers();

        void AddContainer(IItemContainer itemContainer);

        bool RemoveContainer(int id);

        void EnsureFreeSpace(
            int targetSpace,
            IItem? item = null,
            IItemContainer? cloneExample = null
            );

        int GetItemCount(IItem? item);

        int GetFreeSpace(IItem? item);

        IList<IItemContainer> InstantiateContainers(int count, IItemContainer? cloneExample = null);

        IList<IItemContainer> SetContainerCount(int count, IItemContainer? cloneExample = null);

        IList<IItemContainer> RemoveCount(int count);
    }
}
