using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using Cysharp.Threading.Tasks;
using SuperLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UniRx;
using ZLinq;

#pragma warning disable S3236
#nullable enable
namespace CCEnvs.Unity.Items
{
    public class Inventory : ReactiveDictionary<int, IItemContainer>, IInventory
    {
        public bool IsEmpty => Values.Any(cnt => !cnt.IsEmpty);
        public bool IsFull => Values.All(cnt => cnt.IsFull);

        protected int NextSlotID {
            get
            {
                if (Do.TryFindHoleInRange(0, Count, Keys, out int hole))
                    return hole;

                return Count;
            }
        }

        int IItemContainerInfoItemless.ItemCount => throw new NotImplementedException();
        Maybe<IInventory> IItemContainerInfoItemless.ParentInventory { get => null!; set => _ = value; }
        int IItemContainerInfoItemless.Capacity {
            get => Count;
            set => throw new NotSupportedException();
        }

        public Inventory()
        {
        }

        public Inventory(IEqualityComparer<int> comparer) : base(comparer)
        {
        }

        public Inventory(Dictionary<int, IItemContainer> innerDictionary) : base(innerDictionary)
        {
        }

        public bool ContainsItem()
        {
            return Values.ZL().Any(x => x.ContainsItem());
        }

        public bool ContainsItem(IItem? item)
        {
            return Values.ZL().Any(x => x.ContainsItem(item));
        }

        public bool ContainsItem(IItem? item, int count)
        {
            int containedCount = Values.ZL()
                .Where(x => x.ContainsItem(item))
                .Sum(x => x.ItemCount);

            return containedCount >= count;
        }

        public void ResetItemContainers()
        {
            foreach (var cnt in Values)
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
            foreach (var cnt in from cnt in this.ZL() //Searching for the container with same item or first empty container
                                where cnt.Value.IsEmpty || (cnt.Value.ContainsItem(item) && !cnt.Value.IsFull)
                                select (cnt, priority: cnt.Value.ContainsItem(item) ? cnt.Key - 1 : cnt.Key) into pair
                                orderby pair.priority
                                select pair.cnt)
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
            foreach (var cnt in Values.ZL().Where(x => x.ContainsItem(item)))
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

        void IItemAccessor.Reset() => Clear();

        Maybe<int> IItemContainerInfoItemless.GetContainerID() => Maybe<int>.None;

        void IItemAccessor.CopyFrom(IItemContainerInfo itemContainer)
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
