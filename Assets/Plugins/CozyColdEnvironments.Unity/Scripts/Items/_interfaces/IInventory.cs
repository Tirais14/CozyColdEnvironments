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
        IItemContainerInfoItemless
    {
        bool AutoSize { get; set; }

        IReadOnlyDictionary<int, IItemContainer> Containers { get; }

        int ContainerCount { get; }

        IItemContainer this[int id] { get; }

        bool TryGetContainer(int id, [NotNullWhen(true)] out IItemContainer? container);

        void ResetContainers();

        void AddContainer(IItemContainer itemContainer);

        bool RemoveContainer(int id);

        void EnsureFreeSpace(
            int targetSpace,
            IItem? item = default,
            IItemContainer? cloneExample = default
            );

        int GetItemCount(IItem? item);

        int GetFreeSpace(IItem? item);

        Maybe<int> GetContainerID(IItemContainer cnt);

        void InstantiateContainers(int count, IItemContainer? cloneExample = default);
        void InstantiateContainers(int count, out IList<IItemContainer> results, IItemContainer? cloneExample = default);

        void SetContainerCount(int count, IItemContainer? cloneExample = default);
        void SetContainerCount(int count, out IList<IItemContainer> changed, IItemContainer? cloneExample = default);

        void RemoveCount(int count);
        void RemoveCount(int count, out IList<IItemContainer> removed);

        Observable<(int ID, IItemContainer Value)> ObserveContainerAdd();

        Observable<(int ID, IItemContainer Value)> ObserveContainerRemove();

        Observable<Unit> ObserveClear();
    }

    public interface IInventory<TItem, TItemContainer, TItemContainerInfo> 
        :
        IInventory,
        IItemAccessor<TItem, TItemContainerInfo>,
        IItemContainerInfoItemless

        where TItem : IItem
        where TItemContainer : IItemContainer
        where TItemContainerInfo : IItemContainerInfo
    {
        new IReadOnlyDictionary<int, TItemContainer> Containers { get; }

        new TItemContainer this[int id] { get; }

        bool TryGetContainer(int id, [NotNullWhen(true)] out TItemContainer? container);

        void AddContainer(TItemContainer itemContainer);

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

        new Observable<(int ID, TItemContainer Value)> ObserveContainerAdd();

        new Observable<(int ID, TItemContainer Value)> ObserveContainerRemove();

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

        Observable<(int ID, IItemContainer Value)> IInventory.ObserveContainerAdd()
        {
            return ObserveContainerAdd().Select(container => (container.ID, (IItemContainer)container.Value));
        }

        Observable<(int ID, IItemContainer Value)> IInventory.ObserveContainerRemove()
        {
            return ObserveContainerRemove().Select(container => (container.ID, (IItemContainer)container.Value));
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
