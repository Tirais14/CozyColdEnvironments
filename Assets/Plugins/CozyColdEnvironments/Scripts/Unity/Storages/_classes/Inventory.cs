using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UniRx;
using UnityEngine;
using ZLinq;
using static CCEnvs.FuncLanguage.LangOperator;

#pragma warning disable S3236
#nullable enable
namespace CCEnvs.Unity.Storages
{
    public class Inventory : IInventory
    {
        private readonly Dictionary<int, IItemContainer> collection = new();
        private readonly ReactiveProperty<Maybe<IItemContainer>> activeContainer = new();
        private readonly Dictionary<int, IDisposable> activeContainerSubscriptions = new(0);

        private Subject<(int id, IItemContainer value)>? addSubj;
        private Subject<(int id, IItemContainer value)>? removeSubj;
        private int nextSlotID;

        public IItemContainer this[int id] => Catch(() => collection[id]).Match(
                                                  some: cnt => cnt,
                                                  none: () => ItemContainer.Empty)
                                                  .AccessUnsafe();

        public event Action<(int id, IItemContainer value)>? OnAdd;
        public event Action<(int id, IItemContainer value)>? OnRemove;

        public int ContainerCount => collection.Count;
        public int Capacity {
            get => collection.Count;
            set => collection.EnsureCapacity(value);
        }
        public IEnumerable<int> IDs => collection.Keys;
        public IEnumerable<IItemContainer> Containers => collection.Values;
        public IReadOnlyReactiveProperty<Maybe<IItemContainer>> ActiveContainer => activeContainer;
        public bool IsEmpty => collection.Values.Any(x => x.ContainsItem());
        public bool IsFull => collection.Values.All(x => x.ContainsItem());

        IReadOnlyReactiveProperty<int> IItemContainerInfoItemless.ItemCount { get; } = new ReadOnlyReactiveProperty<int>(Observable.Empty<int>());
        Maybe<IInventory> IItemContainerInfoItemless.ParentInventory { get => null!; set => _ = value; }
        IReadOnlyReactiveProperty<bool> IItemContainerInfoItemless.IsActiveContainer { get; } = new ReadOnlyReactiveProperty<bool>(Observable.Empty<bool>());

        public Inventory(int initialContainerCount)
        {
            collection = new Dictionary<int, IItemContainer>(initialContainerCount);

            SetContainerCount<ItemContainer>(initialContainerCount);
        }

        public Inventory()
        {
        }

        public Inventory(IEnumerable<KeyValuePair<int, IItemContainer>> containers)
        {
            CC.Guard.IsNotNull(containers, nameof(containers));

            collection = new Dictionary<int, IItemContainer>(containers);
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
                                                  .Sum(x => x.ItemCount.Value);

            return containedCount >= count;
        }

        public bool ContainsContainer(IItemContainer itemContainer)
        {
            return collection.Values.Contains(itemContainer);
        }

        public bool ContainsContainer(int id) => collection.ContainsKey(id);

        public void AddContainer(IItemContainer itemContainer)
        {
            try
            {
                collection.Add(nextSlotID, itemContainer);
                itemContainer.ParentInventory = this.As<IInventory>().Maybe();

                var sub = itemContainer.IsActiveContainer.SubscribeWithState2(activeContainer, itemContainer, (_, prop, cnt) => prop.Value = cnt.Maybe());
                activeContainerSubscriptions.Add(nextSlotID, sub);

                Do.While(() => collection.ContainsKey(nextSlotID), () => nextSlotID++);

                addSubj?.OnNext((nextSlotID, itemContainer));
                OnAdd?.Invoke((nextSlotID, itemContainer));
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }
        public void AddContainer(GameObject toInstantiate)
        {
            UnityEngine.Object.Instantiate(toInstantiate)
                              .FindFor()
                              .InChildren()
                              .IncludeInactive()
                              .Model<IItemContainer>()
                              .Match(
                              some: x => AddContainer(x),
                              none: () => this.PrintError($"Cannot find {nameof(ItemContainer)} model.")
                              );
        }

        public void AddContainerCount(int count, UnityEngine.GameObject toInstantiate)
        {
            for (int i = 0; i < count; i++)
                AddContainer(toInstantiate);
        }

        public void AddContainerCount<T>(int count)
            where T : IItemContainer, new()
        {
            for (int i = 0; i < count; i++)
                AddContainer(new T());
        }

        public bool RemoveContainer(int id)
        {
            if (collection.Remove(id, out IItemContainer value))
            {
                value.ParentInventory = null!;
                activeContainerSubscriptions.Remove(id);

                try
                {
                    removeSubj?.OnNext((id, value));
                    OnRemove?.Invoke((id, value));
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }

                return true;
            }

            return false;
        }
        public bool RemoveContainer(IItemContainer itemContainer)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            KeyValuePair<int, IItemContainer> found = collection.FirstOrDefault(x => x.Value.Equals(itemContainer));
            if (found.IsDefault())
                return false;

            return RemoveContainer(found.Key);
        }

        public void RemoveContainerCount(int count)
        {
            for (int i = 0; i < count; i++)
                RemoveLast();
        }

        public bool RemoveLast() => RemoveContainer(collection.Keys.Max());

        public void Clear()
        {
            foreach (var id in collection.Keys.ToArray())
                collection.Remove(id);
        }

        public void SetContainerCount<T>(int count)
            where T : IItemContainer, new()
        {
            if (count == ContainerCount)
                return;

            int delta = count - ContainerCount;
            if (delta < 0)
                RemoveContainerCount(Math.Abs(delta));
            else
                AddContainerCount<T>(delta);
        }
        public void SetContainerCount(int count, GameObject toInstantiate)
        {
            CC.Guard.IsNotNull(toInstantiate, nameof(toInstantiate));

            if (count == ContainerCount)
                return;

            int delta = count - ContainerCount;
            if (delta < 0)
                RemoveContainerCount(Math.Abs(delta));
            else
                AddContainerCount(delta, toInstantiate);
        }

        public IObservable<(int id, IItemContainer value)> ObserveAddContainer()
        {
            addSubj ??= new Subject<(int id, IItemContainer value)>();

            return addSubj;
        }

        public IObservable<(int id, IItemContainer value)> ObserveRemoveContainer()
        {
            removeSubj ??= new Subject<(int id, IItemContainer value)>();

            return removeSubj;
        }

        public Maybe<IItemContainer> PutItem(IItem? item, int count = 1)
        {
            if (item.IsNull() || count <= 0)
            {
                this.PrintLog($"Item: {item.Maybe().Map(x => x!.ToString()).Access("null")}, count: {count}. Is not added.");
                return null!;
            }

            int rest = count;
            Maybe<IItemContainer> restItems;
            foreach (var cnt in from it in collection.ZL() //Searching for container with item or first empty container
                                where it.Value.IsEmpty || (it.Value.ContainsItem(item) && !it.Value.IsFull)
                                select (it, priority: it.Value.ContainsItem(item) ? it.Key - 1 : it.Key) into pair
                                orderby pair.priority
                                select pair.it)
            {
                restItems = cnt.Value.PutItem(item, rest);

                this.PrintLog($"Item: {item}, count: {count}, item container id: {cnt.Key}. Is added.");

                rest = restItems.Map(x => x.ItemCount.Value).Access(0);

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

        public Maybe<IItemContainer> PutItem(IItemContainer itemContainer, int count)
        {
            return (from cnt in itemContainer.TakeItem(count)
                    where cnt.Item.Value.IsSome
                    let item = cnt.Item.Value.AccessUnsafe()
                    select (cnt, rest: PutItem(item, count)))
                    .IfRight(x => x.rest.IfSome(
                        rest => itemContainer.PutItem(rest)).Target)
                    .RightTarget
                    .Maybe()!;
        }

        [DebuggerStepThrough]
        public Maybe<IItemContainer> PutItem(IItemContainer itemContainer)
        {
            return PutItem(itemContainer.Item.Value.Access(), itemContainer.ItemCount.Value);
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
                count -= taked.AccessUnsafe().ItemCount.Value;

                if (count <= 0)
                    break;
            }

            return default!;
        }

        public bool IsContainerActive(int id)
        {
            return ActiveContainer.Value.Map(cnt => cnt.GetContainerID()).ItIs(cntID => cntID == id);
        }

        public bool IsContainerActive(IItemContainer itemContainer)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            return ActiveContainer.Value.Map(cnt => cnt == itemContainer).Target;
        }

        public void ActivateContainer(int id)
        {
            if (activeContainer.Value.ItIs(x => x.GetContainerID() == id))
                return;

            Catch(() => this[id]).Match(
                some: cnt => cnt.ActivateContainer(),
                none: () => this.PrintError($"Cannot find {nameof(ItemContainer)} with id: {id}"));
        }

        public void DeactivateContainer()
        {
            if (activeContainer.Value.IsNone)
                return;

            activeContainer.Value.IfSome(cnt => cnt.DeactivateContainer());
        }

        public bool SwitchContainerActiveState(int id)
        {
            return this[id].SwitchContainerActiveState();
        }

        public MaybeStruct<int> GetContainerID(IItemContainer itemContainer)
        {
            Guard.IsNotNull(itemContainer, nameof(itemContainer));

            return collection.FirstOrDefault(x => x.Value.Equals(itemContainer))
                             .Maybe()
                             .Map(x => x.Key.Maybe(x => x >= 0))
                             .Access();
        }

        Maybe<IItemContainer> IItemAccessor.TakeItem(int count) => null!;

        Maybe<IItemContainer> IItemAccessor.TakeItem() => null!;

        void IItemAccessor.CopyFrom(IItemContainerInfo itemContainer)
        {
            this.PrintLog($"{nameof(IItemAccessor.CopyFrom)} not supported and was be mocked.");
        }

        MaybeStruct<int> IItemContainerInfoItemless.GetContainerID() => (-1, false);

        void IItemContainerInfoItemless.ActivateContainer() => Do.Nothing();

        void IItemContainerInfoItemless.DeactivateContainer() => Do.Nothing();

        bool IItemContainerInfoItemless.SwitchContainerActiveState() => Do.Nothing<bool>();
    }
}
