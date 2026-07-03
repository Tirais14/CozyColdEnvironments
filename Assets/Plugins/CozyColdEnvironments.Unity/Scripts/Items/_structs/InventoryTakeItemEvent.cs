#nullable enable
using System;
using System.Collections.Generic;

namespace CCEnvs.UnityX.Items
{
    public readonly struct InventoryTakeItemEvent : IEquatable<InventoryTakeItemEvent>
    {
        public ItemAccessorTakeItemEvent Data { get; }

        public IItemContainer Container { get; }

        public InventoryTakeItemEvent(ItemAccessorTakeItemEvent data, IItemContainer container)
        {
            Data = data;
            Container = container;
        }

        public static bool operator ==(InventoryTakeItemEvent left, InventoryTakeItemEvent right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InventoryTakeItemEvent left, InventoryTakeItemEvent right)
        {
            return !(left == right);
        }

        public InventoryTakeItemEvent<TItem, TContainer> Convert<TItem, TContainer>()
            where TItem : IItem
            where TContainer : IItemContainer<TItem>
        {
            return new InventoryTakeItemEvent<TItem, TContainer>(
                Data.Convert<TItem>(),
                Container.CastTo<TContainer>()
                );
        }

        public override bool Equals(object? obj)
        {
            return obj is InventoryTakeItemEvent @event && Equals(@event);
        }

        public bool Equals(InventoryTakeItemEvent other)
        {
            return Data.Equals(other.Data) &&
                   EqualityComparer<IItemContainer>.Default.Equals(Container, other.Container);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Data, Container);
        }
    }


    public readonly struct InventoryTakeItemEvent<TItem, TContainer> : IEquatable<InventoryTakeItemEvent<TItem, TContainer>> where TItem : IItem
        where TContainer : IItemContainer
    {
        public ItemAccessorTakeItemEvent<TItem> Data { get; }

        public TContainer Container { get; }

        public InventoryTakeItemEvent(ItemAccessorTakeItemEvent<TItem> data, TContainer container)
        {
            Data = data;
            Container = container;
        }

        public static implicit operator InventoryTakeItemEvent(InventoryTakeItemEvent<TItem, TContainer> instance)
        {
            return instance.AsUntyped();
        }

        public static bool operator ==(InventoryTakeItemEvent<TItem, TContainer> left, InventoryTakeItemEvent<TItem, TContainer> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InventoryTakeItemEvent<TItem, TContainer> left, InventoryTakeItemEvent<TItem, TContainer> right)
        {
            return !(left == right);
        }

        public InventoryTakeItemEvent AsUntyped()
        {
            return new InventoryTakeItemEvent(Data, Container);
        }

        public InventoryTakeItemEvent<TOutItem, TOutContainer> Convert<TOutItem, TOutContainer>()
            where TOutItem : TItem
            where TOutContainer : IItemContainer<TOutItem>
        {
            return new InventoryTakeItemEvent<TOutItem, TOutContainer>(
                Data.Convert<TOutItem>(),
                Container.CastTo<TOutContainer>()
                );
        }

        public override bool Equals(object? obj)
        {
            return obj is InventoryTakeItemEvent<TItem, TContainer> @event && Equals(@event);
        }

        public bool Equals(InventoryTakeItemEvent<TItem, TContainer> other)
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
