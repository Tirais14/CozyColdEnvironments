using CCEnvs.Linq;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;

#if ZLINQ_PLUGIN
using ZLinq;
#endif

#pragma warning disable S3236
#nullable enable
namespace CCEnvs.UnityX.Items
{
    public class Inventory
        :
        InventoryBase<
            IItem, 
            IItemContainer,
            IItemContainer,
            IItemContainerInfo,
            ReadOnlyItemContainer,
            LargeReadOnlyItemContainer,
            InventoryContainerAddEvent,
            InventoryContainerRemoveEvent,
            InventoryContainerReplaceEvent,
            InventoryPutItemEvent,
            InventoryTakeItemEvent
            >,
        IInventory,
        IDisposable
    {
        public static Inventory World { get; } = new() 
        {
            AutoSize = true,
            ContainerSample = new ItemContainer() 
        };

        public Inventory(
            int collectionCapacity = 4,
            IEqualityComparer<IItemContainer?>? containerComparer = null,
            IEnumerable<IItemContainer>? initialContainers = null
            )
            :
            base(
                collectionCapacity,
                containerComparer,
                initialContainers)
        {
        }

        public Inventory(
            ICollection<IItemContainer> initialContainers,
            IEqualityComparer<IItemContainer?>? containerComparer = null
            )
            :
            base(
                initialContainers.Count,
                containerComparer,
                initialContainers
                )
        {
        }

        public static Inventory CreateWith<TItemContainer>(
            int containerCount,
            IEqualityComparer<IItemContainer?>? containerComparer = null,
            bool autoSize = false,
            IItemContainer? containerSample = null
            )
            where TItemContainer : IItemContainer, new()
        {
            return new Inventory(
                containerCount,
                containerComparer,
                Enumerable.Range(0, containerCount)
                    .Select(_ => new TItemContainer())
                    .Cast<IItemContainer>()
                )
            {
                AutoSize = autoSize,
                ContainerSample = containerSample
            };
        }

        ~Inventory() => Dispose();

        public IInventory ShallowClone()
        {
            var clone = new Inventory(ContainerCount, ContainerComaprer)
            {
                AutoSize = AutoSize,
                ContainerSample = ContainerSample
            };

            foreach (var (id, container) in Containers)
                clone.AddContainer(container, id);

            return clone;
        }

        protected override ReadOnlyItemContainer CreateReadOnlyItemContainer()
        {
            return ReadOnlyItemContainer.Empty;
        }
        protected override ReadOnlyItemContainer CreateReadOnlyItemContainer(IItem? item, int itemCount)
        {
            return new ReadOnlyItemContainer(item, itemCount);
        }

        protected override LargeReadOnlyItemContainer CreateLargeReadOnlyItemContainer()
        {
            return LargeReadOnlyItemContainer.Empty;
        }
        protected override LargeReadOnlyItemContainer CreateLargeReadOnlyItemContainer(IItem? item, long itemCount)
        {
            return new LargeReadOnlyItemContainer(item, itemCount);
        }

        protected override ReadOnlyItemContainer ConvertLargeToNormalReadOnlyContainer(LargeReadOnlyItemContainer largeContainer)
        {
            return (ReadOnlyItemContainer)largeContainer;
        }

        protected override InventoryContainerAddEvent CreateContainerAddEvent(int id, IItemContainer container)
        {
            return new InventoryContainerAddEvent 
            {
                ID = id,
                Container = container
            };
        }

        protected override InventoryContainerRemoveEvent CreateContainerRemoveEvent(int id, IItemContainer container)
        {
            return new InventoryContainerRemoveEvent 
            {
                ID = id,
                Container = container
            };
        }

        protected override InventoryContainerReplaceEvent CreateContainerReplaceEvent(int id, IItemContainer oldContainer, IItemContainer newContainer)
        {
            return new InventoryContainerReplaceEvent
            {
                ID = id, 
                OldContainer = oldContainer,
                NewContainer = newContainer
            };
        }

        protected override IItem? GetItemFromReadOnlyContainer(LargeReadOnlyItemContainer largeReadOnlyContainer)
        {
            return largeReadOnlyContainer.Item;
        }

        protected override long GetItemCountFromReadOnlytContainer(LargeReadOnlyItemContainer largeReadOnlyContainer)
        {
            return largeReadOnlyContainer.ItemCount;
        }

        protected override InventoryPutItemEvent CreatePutItemEvent(
            IItem item,
            int itemCount,
            IItemContainer container
            )
        {
            return new InventoryPutItemEvent(
                new ItemAccessorPutItemEvent(item, itemCount),
                container
                );
        }

        protected override InventoryTakeItemEvent CreateTakeItemEvent(
            IItem item,
            int itemCount,
            IItemContainer container
            )
        {
            return new InventoryTakeItemEvent(
                new ItemAccessorTakeItemEvent(item, itemCount), 
                container
                );
        }
    }

    public class Inventory<TItem, TContainer>
        :
        InventoryBase<
            TItem,
            TContainer,
            IItemContainer<TItem>,
            IItemContainerInfo<TItem>,
            ReadOnlyItemContainer<TItem>, 
            LargeReadOnlyItemContainer<TItem>,
            InventoryContainerAddEvent<TContainer>,
            InventoryContainerRemoveEvent<TContainer>,
            InventoryContainerReplaceEvent<TContainer>,
            InventoryPutItemEvent<TItem, TContainer>,
            InventoryTakeItemEvent<TItem, TContainer>
            >,
        IInventory<TItem, TContainer>,
        IDisposable

        where TItem : class, IItem
        where TContainer : class, IItemContainer<TItem>
    {
        public Inventory(
            int collectionCapacity = 4,
            IEqualityComparer<TContainer?>? containerComparer = null,
            IEnumerable<TContainer>? initialContainers = null
            )
            :
            base(
                collectionCapacity,
                containerComparer,
                initialContainers)
        {
        }

        public Inventory(
            ICollection<TContainer> initialContainers,
            IEqualityComparer<TContainer?>? containerComparer = null
            )
            :
            base(
                initialContainers.Count,
                containerComparer,
                initialContainers
                )
        {
        }

        public static Inventory<TItem, TContainer> CreateWith<TCreatableItemContainer>(
            int containerCount,
            IEqualityComparer<IItemContainer?>? containerComparer = null,
            bool autoSize = false,
            TContainer? containerSample = null
            )
            where TCreatableItemContainer : TContainer, new()
        {
            return new Inventory<TItem, TContainer>(
                containerCount,
                containerComparer,
                Enumerable.Range(0, containerCount)
                    .Select(_ => new TCreatableItemContainer())
                    .Cast<TContainer>()
                )
            {
                AutoSize = autoSize,
                ContainerSample = containerSample
            };
        }

        ~Inventory() => Dispose();

        public IInventory ShallowClone()
        {
            var clone = new Inventory<TItem, TContainer>(
                ContainerCount,
                ContainerComaprer
                )
            {
                AutoSize = AutoSize,
                ContainerSample = ContainerSample
            };

            foreach (var (id, container) in Containers)
                clone.AddContainer(container, id);

            return clone;
        }

        protected override InventoryContainerAddEvent<TContainer> CreateContainerAddEvent(
            int id,
            TContainer container
            )
        {
            return new InventoryContainerAddEvent<TContainer> 
            {
                ID = id,
                Container = container 
            };
        }

        protected override InventoryContainerRemoveEvent<TContainer> CreateContainerRemoveEvent(
            int id,
            TContainer container
            )
        {
            return new InventoryContainerRemoveEvent<TContainer>
            {
                ID = id,
                Container = container 
            };
        }

        protected override InventoryContainerReplaceEvent<TContainer> CreateContainerReplaceEvent(
            int id,
            TContainer oldContainer,
            TContainer newContainer
            )
        {
            return new InventoryContainerReplaceEvent<TContainer>
            {
                ID = id,
                OldContainer = oldContainer,
                NewContainer = newContainer
            };
        }

        protected override ReadOnlyItemContainer<TItem> CreateReadOnlyItemContainer()
        {
            return ReadOnlyItemContainer<TItem>.Empty;
        }

        protected override ReadOnlyItemContainer<TItem> CreateReadOnlyItemContainer(TItem? item, int itemCount)
        {
            return new ReadOnlyItemContainer<TItem>(item, itemCount);
        }

        protected override LargeReadOnlyItemContainer<TItem> CreateLargeReadOnlyItemContainer()
        {
            return LargeReadOnlyItemContainer<TItem>.Empty;
        }

        protected override LargeReadOnlyItemContainer<TItem> CreateLargeReadOnlyItemContainer(TItem? item, long itemCount)
        {
            return new LargeReadOnlyItemContainer<TItem>(item, itemCount);
        }

        protected override ReadOnlyItemContainer<TItem> ConvertLargeToNormalReadOnlyContainer(LargeReadOnlyItemContainer<TItem> largeContainer)
        {
            return largeContainer.ToNormal();
        }

        protected override TItem? GetItemFromReadOnlyContainer(LargeReadOnlyItemContainer<TItem> largeReadOnlyContainer)
        {
            return largeReadOnlyContainer.Item;
        }

        protected override long GetItemCountFromReadOnlytContainer(LargeReadOnlyItemContainer<TItem> largeReadOnlyContainer)
        {
            return largeReadOnlyContainer.ItemCount;
        }

        protected override InventoryPutItemEvent<TItem, TContainer> CreatePutItemEvent(
            TItem item,
            int itemCount,
            TContainer container
            )
        {
            return new InventoryPutItemEvent<TItem, TContainer>(
                new ItemAccessorPutItemEvent<TItem>(item, itemCount), 
                container
                );
        }

        protected override InventoryTakeItemEvent<TItem, TContainer> CreateTakeItemEvent(
            TItem item,
            int itemCount,
            TContainer container
            )
        {
            return new InventoryTakeItemEvent<TItem, TContainer>(
                new ItemAccessorTakeItemEvent<TItem>(item, itemCount),
                container
                );
        }
    }
}
