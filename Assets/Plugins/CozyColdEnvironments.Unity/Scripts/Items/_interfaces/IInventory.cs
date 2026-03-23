#nullable enable
using CCEnvs.FuncLanguage;
using ObservableCollections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs.Unity.Items
{
    public interface IInventory
        : IItemAccessor,
        IItemContainerInfoItemless,
        IEnumerable<IItemContainer>
    {
        bool AutoSize { get; set; }

        IReadOnlyObservableDictionary<int, IItemContainer> Containers { get; }

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

        Maybe<int> GetContainerID(IItemContainer cnt);

        IList<IItemContainer> InstantiateContainers(int count, IItemContainer? cloneExample = null);

        IList<IItemContainer> SetContainerCount(int count, IItemContainer? cloneExample = null);

        IList<IItemContainer> RemoveCount(int count);
    }
}
