using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public readonly struct InventoryPutItemEvent : IEquatable<InventoryPutItemEvent>
    {
        public ItemAccessorPutItemEvent Data { get; }

        public IItemContainer Container { get; }

        public InventoryPutItemEvent(ItemAccessorPutItemEvent data, IItemContainer container)
        {
            Guard.IsNotDefault(data);
            CC.Guard.IsNotNull(container, nameof(container));

            Data = data;
            Container = container;
        }

        public static bool operator ==(InventoryPutItemEvent left, InventoryPutItemEvent right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InventoryPutItemEvent left, InventoryPutItemEvent right)
        {
            return !(left == right);
        }

        public InventoryPutItemEvent<TItem, TContainer> Convert<TItem, TContainer>()
            where TItem : IItem
            where TContainer : IItemContainer<TItem>
        {
            return new InventoryPutItemEvent<TItem, TContainer>(Data.Convert<TItem>(), Container.CastTo<TContainer>());
        }

        public override bool Equals(object? obj)
        {
            return obj is InventoryPutItemEvent @event && Equals(@event);
        }

        public bool Equals(InventoryPutItemEvent other)
        {
            return Data.Equals(other.Data) &&
                   EqualityComparer<IItemContainer>.Default.Equals(Container, other.Container);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Data, Container);
        }
    }

    public readonly struct InventoryPutItemEvent<TItem, TContainer> : IEquatable<InventoryPutItemEvent<TItem, TContainer>>
        where TItem : IItem
        where TContainer : IItemContainer<TItem>
    {
        public ItemAccessorPutItemEvent<TItem> Data { get; }

        public TContainer Container { get; }

        public InventoryPutItemEvent(ItemAccessorPutItemEvent<TItem> data, TContainer container)
        {
            Data = data;
            Container = container;
        }

        public static implicit operator InventoryPutItemEvent(InventoryPutItemEvent<TItem, TContainer> instance)
        {
            return instance.AsUntyped();
        }

        public static bool operator ==(InventoryPutItemEvent<TItem, TContainer> left, InventoryPutItemEvent<TItem, TContainer> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InventoryPutItemEvent<TItem, TContainer> left, InventoryPutItemEvent<TItem, TContainer> right)
        {
            return !(left == right);
        }

        public InventoryPutItemEvent AsUntyped()
        {
            return new InventoryPutItemEvent(Data, Container);
        }

        public InventoryPutItemEvent<TOutItem, TOutContainer> Convert<TOutItem, TOutContainer>()
            where TOutItem : TItem
            where TOutContainer : IItemContainer<TOutItem>
        {
            return new InventoryPutItemEvent<TOutItem, TOutContainer>(
                Data.Convert<TOutItem>(),
                Container.CastTo<TOutContainer>()
                );
        }

        public override bool Equals(object? obj)
        {
            return obj is InventoryPutItemEvent<TItem, TContainer> @event && Equals(@event);
        }

        public bool Equals(InventoryPutItemEvent<TItem, TContainer> other)
        {
            return Data.Equals(other.Data) &&
                   EqualityComparer<TContainer>.Default.Equals(Container, other.Container);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Data, Container);
        }
    }
}
