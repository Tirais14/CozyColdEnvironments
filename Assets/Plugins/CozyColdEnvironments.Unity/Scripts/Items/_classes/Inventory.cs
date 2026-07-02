using CCEnvs.Linq;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine.UIElements;

#if ZLINQ_PLUGIN
using ZLinq;
#endif

#pragma warning disable S3236
#nullable enable
namespace CCEnvs.UnityX.Items
{
    public class Inventory
        :
        InventoryBase<IItem, IItemContainer, IItemContainer, IItemContainerInfo, ReadOnlyItemContainer, InventoryContainerAddEvent, InventoryContainerRemoveEvent, InventoryContainerReplaceEvent>,
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
                ContainerSample = containerSample.IfNull(static () => new ItemContainer())
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

        ReadOnlyItemContainer IItemAccessor.TakeItem()
        {
            return TakeItemInternal();
        }

        ReadOnlyItemContainer IItemAccessor.TakeItem(int count)
        {
            return TakeItemInternal(count);
        }
    }

    public class Inventory<TItem, TItemContainer>
        :
        InventoryBase<TItem, TItemContainer, IItemContainer<TItem>, IItemContainerInfo<TItem>, ReadOnlyItemContainer<TItem>, InventoryContainerAddEvent<TItemContainer>, InventoryContainerRemoveEvent<TItemContainer>, InventoryContainerReplaceEvent<TItemContainer>>,
        IInventory<TItem, TItemContainer>,
        IDisposable

        where TItem : class, IItem
        where TItemContainer : class, IItemContainer<TItem>
    {
        public Inventory(
            int collectionCapacity = 4,
            IEqualityComparer<TItemContainer?>? containerComparer = null,
            IEnumerable<TItemContainer>? initialContainers = null
            )
            :
            base(
                collectionCapacity,
                containerComparer,
                initialContainers)
        {
        }

        public Inventory(
            ICollection<TItemContainer> initialContainers,
            IEqualityComparer<TItemContainer?>? containerComparer = null
            )
            :
            base(
                initialContainers.Count,
                containerComparer,
                initialContainers
                )
        {
        }

        public static Inventory<TItem, TItemContainer> CreateWith<TCreatableItemContainer>(
            int containerCount,
            IEqualityComparer<IItemContainer?>? containerComparer = null,
            bool autoSize = false,
            TItemContainer? containerSample = null
            )
            where TCreatableItemContainer : TItemContainer, new()
        {
            return new Inventory<TItem, TItemContainer>(
                containerCount,
                containerComparer,
                Enumerable.Range(0, containerCount)
                    .Select(_ => new TCreatableItemContainer())
                    .Cast<TItemContainer>()
                )
            {
                AutoSize = autoSize,
                ContainerSample = containerSample.IfNull(static () => new TCreatableItemContainer())
            };
        }

        ~Inventory() => Dispose();

        public IInventory ShallowClone()
        {
            throw new NotImplementedException();
        }

        protected override InventoryContainerAddEvent<TItemContainer> CreateContainerAddEvent(
            int id,
            TItemContainer container
            )
        {
            return new InventoryContainerAddEvent<TItemContainer> 
            {
                ID = id,
                Container = container 
            };
        }

        protected override InventoryContainerRemoveEvent<TItemContainer> CreateContainerRemoveEvent(
            int id,
            TItemContainer container
            )
        {
            return new InventoryContainerRemoveEvent<TItemContainer>
            {
                ID = id,
                Container = container 
            };
        }

        protected override InventoryContainerReplaceEvent<TItemContainer> CreateContainerReplaceEvent(
            int id,
            TItemContainer oldContainer,
            TItemContainer newContainer
            )
        {
            return new InventoryContainerReplaceEvent<TItemContainer>
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

        ReadOnlyItemContainer<TItem> IItemAccessor<TItem>.TakeItem()
        {
            return TakeItemInternal();
        }

        ReadOnlyItemContainer<TItem> IItemAccessor<TItem>.TakeItem(int count)
        {
            return TakeItemInternal(count);
        }
    }
}
