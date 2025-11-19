using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Unity.Collections;
using Cysharp.Threading.Tasks;
using SuperLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UniRx;
using UnityEngine;
using ZLinq;

#pragma warning disable S3236
#nullable enable
namespace CCEnvs.Unity.Items
{
    public class Inventory 
        : ActivatableNodeCollection<int,
        IItemContainer>,
        IInventory
    {
        public int Capacity {
            get => collection.Count;
            set => collection.EnsureCapacity(value);
        }
        public bool IsEmpty => collection.Values.Any(x => x.ContainsItem());
        public bool IsFull => collection.Values.All(x => x.ContainsItem());

        protected int NextSlotID {
            get
            {
                if (Do.TryFindHoleInRange(0, Count, Keys, out int hole))
                    return hole;

                return Count;
            }
        }

        bool IActivatable.IsActive => throw new NotImplementedException();
        int IItemContainerInfoItemless.ItemCount => throw new NotImplementedException();
        Maybe<IInventory> IItemContainerInfoItemless.ParentInventory { get => null!; set => _ = value; }
        bool IItemContainerInfoItemless.IsActive => throw new NotImplementedException();

        public Inventory()
        {
            KeyFactory = (_, _) => NextSlotID;
        }

        public Inventory(int capacity) : base(capacity)
        {
            KeyFactory = (_, _) => NextSlotID;
        }

        public Inventory(IEnumerable<KeyValuePair<int, IItemContainer>> nodes)
            :
            base(nodes)
        {
            KeyFactory = (_, _) => NextSlotID;
        }

        public Inventory(int nodeCount, GameObject nodePrefab)
            :
            base(nodeCount, nodePrefab)
        {
            KeyFactory = (_, _) => NextSlotID;
        }

        public bool ContainsItem()
        {
            return collection.Values.ZL().Any(x => x.ContainsItem());
        }

        public bool ContainsItem(IItem? item)
        {
            return collection.Values.ZL().Any(x => x.ContainsItem(item));
        }

        public bool ContainsItem(IItem? item, int count)
        {
            int containedCount = collection.Values.ZL()
                                                  .Where(x => x.ContainsItem(item))
                                                  .Sum(x => x.ItemCount);

            return containedCount >= count;
        }

        public void ResetItemContainers()
        {
            foreach (var cnt in Nodes)
                cnt.Reset();
        }

        public Maybe<IItemContainer> PutItem(IItem? item, int count = 1)
        {
            if (item.IsNull() || count <= 0)
            {
                this.PrintLog($"Item: {item.Maybe().Map(x => x!.ToString()).GetValue("null")}, count: {count}. Is not added.");
                return null!;
            }

            int rest = count;
            Maybe<IItemContainer> restItems;
            foreach (var cnt in from it in collection.ZL() //Searching for the container with same item or first empty container
                                where it.Value.IsEmpty || (it.Value.ContainsItem(item) && !it.Value.IsFull)
                                select (it, priority: it.Value.ContainsItem(item) ? it.Key - 1 : it.Key) into pair
                                orderby pair.priority
                                select pair.it)
            {
                restItems = cnt.Value.PutItem(item, rest);

                this.PrintLog($"Item: {item}, count: {count}, item container id: {cnt.Key}. Is added.");

                rest = restItems.Map(x => x.ItemCount).GetValue(0);

                if (rest <= 0)
                    break;
            }

            if (rest > 0)
            {
                return new ItemContainer(item, rest)
                {
                    UnlockCapacity = true
                };
            }

            return null!;
        }

        public Maybe<IItemContainer> PutItemFrom(IItemContainer itemContainer, int count)
        {
            return (from cnt in itemContainer.TakeItem(count)
                    where cnt.Item.IsSome
                    let item = cnt.Item.GetValueUnsafe()
                    select (cnt, rest: PutItem(item, count)))
                    .IfRight(x => x.rest.IfSome(
                        rest => itemContainer.PutItemFrom(rest)).Raw)
                    .RightTarget
                    .Maybe()!;
        }

        [DebuggerStepThrough]
        public Maybe<IItemContainer> PutItemFrom(IItemContainer itemContainer)
        {
            return PutItem(itemContainer.Item.GetValue(), itemContainer.ItemCount);
        }

        public Maybe<IItemContainer> TakeItem(IItem item, int count)
        {
            CC.Guard.IsNotNull(item, nameof(item));
            if (count <= 0)
                return null!;

            Maybe<IItemContainer> taked;
            foreach (var cnt in collection.Values.ZL().Where(x => x.ContainsItem(item)))
            {
                taked = cnt.TakeItem(count);
                count -= taked.GetValueUnsafe().ItemCount;

                if (count <= 0)
                    break;
            }

            return default!;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        Maybe<IItemContainer> IItemAccessor.TakeItem(int count) => null!;

        Maybe<IItemContainer> IItemAccessor.TakeItem() => null!;

        void IItemAccessor.Reset()
        {
            throw new NotImplementedException();
        }

        Maybe<int> IItemContainerInfoItemless.GetContainerID() => Maybe<int>.None;

        void IItemAccessor.CopyFrom(IItemContainerInfo itemContainer)
        {
            throw new NotImplementedException();
        }

        void IActivatable.Activate()
        {
            throw new NotImplementedException();
        }

        void IActivatable.Deactivate()
        {
            throw new NotImplementedException();
        }

        bool IActivatable.SwitchActiveState()
        {
            throw new NotImplementedException();
        }

        IObservable<bool> IActivatable.ObserveActiveState()
        {
            throw new NotImplementedException();
        }

        IObservable<bool> IActivatable.ObserveDeactivate()
        {
            throw new NotImplementedException();
        }

        IObservable<bool> IActivatable.ObserveActivate()
        {
            throw new NotImplementedException();
        }

        IObservable<Pair<int>> IItemContainerInfoItemless.ObserveDecreasedItemCount()
        {
            throw new NotImplementedException();
        }

        IObservable<Pair<int>> IItemContainerInfoItemless.ObserveIncreaseItemCount()
        {
            throw new NotImplementedException();
        }

        IObservable<Pair<int>> IItemContainerInfoItemless.ObserveItemCount()
        {
            throw new NotImplementedException();
        }
    }
}
