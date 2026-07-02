#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.TypeMatching;
using R3;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CCEnvs.UnityX.Items
{
    public interface IInventory
        :
        IShallowCloneable<IInventory>
    {
        IItemContainer this[int id] { get; }

        /// <summary>
        /// Automatically add container if item not fit in existing containers
        /// </summary>
        bool AutoSize { get; set; }

        IEnumerable<KeyValuePair<int, IItemContainer>> Containers { get; }

        int ContainerCount { get; }
        int EmptyContainerCount { get; }
        int OccupiedContainerCount { get; }

        /// <summary>
        /// Used for cloning when <see cref="AutoSize"/> is true
        /// </summary>
        IItemContainer? ContainerSample { get; set; }

        bool TryGetContainer(int id, [NotNullWhen(true)] out IItemContainer? container);

        void ResetContainers();

        int AddContainer(IItemContainer container, int? id = null);

        void AddContainers(IEnumerable<IItemContainer> containers);
        void AddContainers(IEnumerable<(IItemContainer Value, int? ID)> containers);
        void AddContainers(IEnumerable<IItemContainer> containers, out IList<int> ids);
        void AddContainers(IEnumerable<(IItemContainer Value, int? ID)> containers, out IList<int> ids);

        bool RemoveContainer(int id);

        bool ContainsContainer(IItemContainer container);
        bool ContainsContainer(int? id);

        LargeReadOnlyItemContainer TakeItem(IItem? item, long itemCount);
        LargeReadOnlyItemContainer TakeItem(IItem? item);

        LargeReadOnlyItemContainer PutItem(IItem? item, long itemCount = 1);
        ReadOnlyItemContainer PutItem(IItemContainerInfo containerInfo);
        ReadOnlyItemContainer PutItem<TItemContainerInfo>(TItemContainerInfo containerInfo)
            where TItemContainerInfo : struct, IItemContainerInfo;

        ReadOnlyItemContainer PutItemFrom(IItemContainer container);
        LargeReadOnlyItemContainer PutItemFrom(IInventory inventory, IItem? item, long itemCount);
        LargeReadOnlyItemContainer PutItemFrom(IInventory inventory, IItem? item);

        void EnsureFreeSpace(
            long targetSpace,
            IItem? item = default,
            IItemContainer? cloneExample = default
            );

        long GetItemCount(IItem? item);

        long GetFreeSpace(IItem? item);

        IList<ReadOnlyItemContainer> GetCompactedContainers();

        IList<IItemContainer> GetOccupiedContainers();

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

        ReadOnlyItemContainer IItemAccessor.TakeItem()
        {
            if (IsEmpty
                ||
                Containers.SelectValue()
                    .FirstOrDefault(container => !container.IsEmpty)
                    .IsNot<IItemContainer>(out var firstContainer)
                    )
            {
                return ReadOnlyItemContainer.Empty;
            }

            int takeCount = Math.Min(firstContainer.ItemCount, Math.Abs(new Random().Next()));
            return TakeItem(firstContainer.Item, takeCount);
        }

        ReadOnlyItemContainer IItemAccessor.TakeItem(int count)
        {
            if (IsEmpty
                ||
                Containers.SelectValue()
                    .FirstOrDefault(container => !container.IsEmpty)
                    .Maybe()
                    .Map(container => container.Item)
                    .TryGetValue(out IItem? firstItem)
                    )
            {
                return ReadOnlyItemContainer.Empty;
            }

            return TakeItem(firstItem, count);
        }
    }

    public interface IInventory<TItem, TItemContainer>
        :
        IInventory,
        IItemAccessor<TItem>,
        IItemContainerInfoItemless<TItem>

        where TItem : IItem
        where TItemContainer : IItemContainer<TItem>
    {
        new TItemContainer this[int id] { get; }

        new IEnumerable<KeyValuePair<int, TItemContainer>> Containers { get; }

        new TItemContainer? ContainerSample { get; set; }

        IItemContainer IInventory.this[int id] => this[id];

        IItemContainer? IInventory.ContainerSample {
            get => ContainerSample;
            set => ContainerSample.As<TItemContainer>();
        }

        IEnumerable<KeyValuePair<int, IItemContainer>> IInventory.Containers {
            get => Containers.Select(x => KeyValuePair.Create(x.Key, (IItemContainer)x.Value));
        }

        bool TryGetContainer(int id, [NotNullWhen(true)] out TItemContainer? container);

        int AddContainer(TItemContainer itemContainer, int? id = null);

        void AddContainers(IEnumerable<TItemContainer> containers);
        void AddContainers(IEnumerable<(TItemContainer Value, int? ID)> containers);
        void AddContainers(
            IEnumerable<TItemContainer> containers,
            out IList<int> ids
            );
        void AddContainers(
            IEnumerable<(TItemContainer Value, int? ID)> containers,
            out IList<int> ids
            );

        void EnsureFreeSpace(
            long targetSpace,
            TItem? item = default,
            TItemContainer? cloneExample = default
            );

        long GetItemCount(TItem? item);

        long GetFreeSpace(TItem? item);

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

        int IInventory.AddContainer(IItemContainer itemContainer, int? id)
        {
            return AddContainer(itemContainer.CastTo<TItemContainer>());
        }

        void IInventory.AddContainers(IEnumerable<IItemContainer> containers)
        {
            CC.Guard.IsNotNull(containers, nameof(containers));
            AddContainers(containers.OfType<TItemContainer>());
        }
        void IInventory.AddContainers(IEnumerable<(IItemContainer Value, int? ID)> containers)
        {
            CC.Guard.IsNotNull(containers, nameof(containers));

            IEnumerable<(TItemContainer Value, int? ID)> typedContainers = containers.Where(container => container.Value.Is<TItemContainer>())
                .Select(container => (container.Value.CastTo<TItemContainer>(), container.ID));

            AddContainers(typedContainers);
        }
        void IInventory.AddContainers(IEnumerable<IItemContainer> containers, out IList<int> ids)
        {
            CC.Guard.IsNotNull(containers, nameof(containers));
            AddContainers(containers.OfType<TItemContainer>(), out ids);
        }
        void IInventory.AddContainers(IEnumerable<(IItemContainer Value, int? ID)> containers, out IList<int> ids)
        {
            CC.Guard.IsNotNull(containers, nameof(containers));

            IEnumerable<(TItemContainer Value, int? ID)> typedContainers = containers.Where(container => container.Value.Is<TItemContainer>())
                .Select(container => (container.Value.CastTo<TItemContainer>(), container.ID));

            AddContainers(
                typedContainers,
                out ids
                );
        }

        void IInventory.EnsureFreeSpace(
            long targetSpace,
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

        long IInventory.GetItemCount(IItem? item)
        {
            return GetItemCount(item.As<TItem>());
        }

        long IInventory.GetFreeSpace(IItem? item)
        {
            return GetFreeSpace(item.As<TItem>());
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
            return GetCompactedContainers().Select(container => container.ToUntyped()).ToArray();
        }

        IList<IItemContainer> IInventory.GetOccupiedContainers()
        {
            return GetOccupiedContainers().Cast<IItemContainer>().ToArray();
        }

        Observable <InventoryContainerAddEvent> IInventory.ObserveContainerAdd()
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
            return source.Containers.SelectValue().GetEnumerator();
        }

        public static IEnumerator<TItemContainer> GetEnumerator<TItem, TItemContainer>(
            this IInventory<TItem, TItemContainer> source
            )

            where TItem : IItem
            where TItemContainer : IItemContainer<TItem>
        {
            CC.Guard.IsNotNullSource(source);
            return source.Containers.SelectValue().GetEnumerator();
        }
    }
}
