#nullable enable
using System.Collections.Generic;
using ObservableCollections;
using R3;

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

        int ContainerCount { get; }

        Result<IItemContainer> this[int id] { get; }

        void ResetContainers();

        void AddContainer(int id, IItemContainer itemContainer);
        void AddContainer(IItemContainer itemContainer);

        bool RemoveContainer(int id);

        T[] AddCount<T>(int count) where T : IItemContainer, new();

        IItemContainer[] SetCount<T>(int count) where T : IItemContainer, new();

        IItemContainer[] RemoveCount(int count);

        Observable<DictionaryAddEvent<int, IItemContainer>> ObserveAddContainer();

        Observable<DictionaryRemoveEvent<int, IItemContainer>> ObserveRemoveContainer();

        Observable<DictionaryReplaceEvent<int, IItemContainer>> ObserveReplaceContainer();

        Observable<CollectionResetEvent<KeyValuePair<int, IItemContainer>>> ObserveReset();
    }
}
