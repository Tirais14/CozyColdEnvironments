#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using R3;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CCEnvs.UnityX.Items
{
    public interface IInventory
        :
        IItemAccessor,
        IItemContainerInfoItemless,
        IShallowCloneable<IInventory>
    {
        IItemContainer this[int id] { get; }

        bool AutoSize { get; set; }

        IReadOnlyDictionary<int, IItemContainer> Containers { get; }

        int ContainerCount { get; }
        int EmptyContainerCount { get; }
        int OccupiedContainerCount { get; }

        /// <summary>
        /// Used for cloning when <see cref="AutoSize"/> is true
        /// </summary>
        IItemContainer? ContainerSample { get; set; }

        bool TryGetContainer(int id, [NotNullWhen(true)] out IItemContainer? container);

        void ResetContainers();

        void AddContainer(IItemContainer container);

        void AddContainers(IEnumerable<IItemContainer> containers);

        bool RemoveContainer(int id);

        void EnsureFreeSpace(
            int targetSpace,
            IItem? item = default,
            IItemContainer? cloneExample = default
            );

        int GetItemCount(IItem? item);

        int GetFreeSpace(IItem? item);

        IList<ReadOnlyItemContainer> GetCompactedContainers();

        IList<IItemContainer> GetOccupiedContainers();

        Maybe<int> GetContainerID(IItemContainer container);

        void InstantiateContainers(int count, IItemContainer? cloneExample = default);
        void InstantiateContainers(int count, out IList<IItemContainer> results, IItemContainer? cloneExample = default);

        void SetContainerCount(int count, IItemContainer? cloneExample = default);
        void SetContainerCount(int count, out IList<IItemContainer> changed, IItemContainer? cloneExample = default);

        void RemoveCount(int count);
        void RemoveCount(int count, out IList<IItemContainer> removed);

        Observable<InventoryContainerAddEvent> ObserveContainerAdd();

        Observable<InventoryContainerRemoveEvent> ObserveContainerRemove();

        Observable<InventoryContainerReplaceEvent> ObserveContainerReplace();

        Observable<Unit> ObserveClear();
    }

    public interface IInventory<TItem, TItemContainer>
        :
        IInventory,
        IItemAccessor<TItem>,
        IItemContainerInfoItemless

        where TItem : IItem
        where TItemContainer : IItemContainer
    {
        new TItemContainer this[int id] { get; }

        new IReadOnlyDictionary<int, TItemContainer> Containers { get; }

        new TItemContainer? ContainerSample { get; set; }

        IItemContainer? IInventory.ContainerSample {
            get => ContainerSample;
            set => ContainerSample.As<TItemContainer>();
        }

        bool TryGetContainer(int id, [NotNullWhen(true)] out TItemContainer? container);

        void AddContainer(TItemContainer itemContainer);

        void AddContainers(IEnumerable<TItemContainer> containers);

        void EnsureFreeSpace(
            int targetSpace,
            TItem? item = default,
            TItemContainer? cloneExample = default
            );

        int GetItemCount(TItem? item);

        int GetFreeSpace(TItem? item);

        Maybe<int> GetContainerID(TItemContainer cnt);

        void InstantiateContainers(int count, TItemContainer? cloneExample = default);
        void InstantiateContainers(int count, out IList<TItemContainer> results, TItemContainer? cloneExample = default);

        void SetContainerCount(int count, TItemContainer? cloneExample = default);
        void SetContainerCount(int count, out IList<TItemContainer> changed, TItemContainer? cloneExample = default);

        void RemoveCount(int count, out IList<TItemContainer> removed);

        new IList<ReadOnlyItemContainer<TItem>> GetCompactedContainers();

        new IList<TItemContainer> GetOccupiedContainers();

        new Observable<InventoryContainerAddEvent<TItemContainer>> ObserveContainerAdd();

        new Observable<InventoryContainerRemoveEvent<TItemContainer>> ObserveContainerRemove();

        new Observable<InventoryContainerReplaceEvent<TItemContainer>> ObserveContainerReplace();

        bool IInventory.TryGetContainer(int id, [NotNullWhen(true)] out IItemContainer? container)
        {
            if (!TryGetContainer(id, out TItemContainer? typedContainer))
            {
                container = null;
                return false;
            }

            container = typedContainer;
            return true;
        }

        void IInventory.AddContainer(IItemContainer itemContainer)
        {
            AddContainer((TItemContainer)itemContainer);
        }

        void IInventory.AddContainers(IEnumerable<IItemContainer> containers)
        {
            CC.Guard.IsNotNull(containers, nameof(containers));
            AddContainers(containers.OfType<TItemContainer>());
        }

        void IInventory.EnsureFreeSpace(
            int targetSpace,
            IItem? item,
            IItemContainer? cloneExample
            )
        {
            EnsureFreeSpace(
                targetSpace,
                item.As<TItem>(),
                cloneExample.As<TItemContainer>()
                );
        }

        int IInventory.GetItemCount(IItem? item)
        {
            return GetItemCount(item.As<TItem>());
        }

        int IInventory.GetFreeSpace(IItem? item)
        {
            return GetFreeSpace(item.As<TItem>());
        }

        Maybe<int> IInventory.GetContainerID(IItemContainer cnt)
        {
            if (cnt.IsNot<TItemContainer>(out var typedContainer))
                return Maybe<int>.None;

            return GetContainerID(typedContainer);
        }

        void IInventory.InstantiateContainers(int count, IItemContainer? cloneExample)
        {
            InstantiateContainers(count, cloneExample.As<TItemContainer>());
        }
        void IInventory.InstantiateContainers(int count, out IList<IItemContainer> results, IItemContainer? cloneExample)
        {
            InstantiateContainers(
                count,
                out IList<TItemContainer> typedResults,
                cloneExample.As<TItemContainer>()
                );

            results = typedResults.Cast<IItemContainer>().ToArray();
        }

        void IInventory.SetContainerCount(int count, IItemContainer? cloneExample)
        {
            SetContainerCount(count, cloneExample.As<TItemContainer>());
        }
        void IInventory.SetContainerCount(int count, out IList<IItemContainer> changed, IItemContainer? cloneExample)
        {
            SetContainerCount(
                count,
                out IList<TItemContainer> typedChanged,
                cloneExample.As<TItemContainer>()
                );

            changed = typedChanged.Cast<IItemContainer>().ToArray();
        }

        void IInventory.RemoveCount(int count, out IList<IItemContainer> removed)
        {
            RemoveCount(count, out IList<TItemContainer> typedRemoved);

            removed = typedRemoved.Cast<IItemContainer>().ToArray();
        }

        IList<ReadOnlyItemContainer> IInventory.GetCompactedContainers()
        {
            return GetCompactedContainers().Select(container => (ReadOnlyItemContainer)container).ToArray();
        }

        IList<IItemContainer> IInventory.GetOccupiedContainers()
        {
            return GetOccupiedContainers().Cast<IItemContainer>().ToArray();
        }

        Observable<InventoryContainerAddEvent> IInventory.ObserveContainerAdd()
        {
            return ObserveContainerAdd()
                .Select(ev => new InventoryContainerAddEvent { ID = ev.ID, Container = ev.Container });
        }

        Observable<InventoryContainerRemoveEvent> IInventory.ObserveContainerRemove()
        {
            return ObserveContainerRemove()
                .Select(ev => new InventoryContainerRemoveEvent { ID = ev.ID, Container = ev.Container });
        }

        Observable<InventoryContainerReplaceEvent> IInventory.ObserveContainerReplace()
        {
            return ObserveContainerReplace()
                .Select(ev => new InventoryContainerReplaceEvent { ID = ev.ID, OldContainer = ev.OldContainer, NewContainer = ev.NewContainer });
        }
    }

    public static class IInventoryExtensions
    {
        public static IEnumerator<IItemContainer> GetEnumerator(this IInventory source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.Containers.Values.GetEnumerator();
        }

        public static IEnumerator<TItemContainer> GetEnumerator<TItem, TItemContainer>(
            this IInventory<TItem, TItemContainer> source
            )

            where TItem : IItem
            where TItemContainer : IItemContainer
        {
            CC.Guard.IsNotNullSource(source);
            return source.Containers.Values.GetEnumerator();
        }
    }
}
