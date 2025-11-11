using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CommunityToolkit.Diagnostics;
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
namespace CCEnvs.Unity.Storages
{
    public class Inventory : IInventory, IDisposable
    {
        private readonly Dictionary<int, IItemContainer> collection = new();
        private readonly ReactiveProperty<Maybe<IItemContainer>> activeContainer = new();

        private Subject<(int id, IItemContainer value)>? addSubj;
        private Subject<(int id, IItemContainer value)>? removeSubj;
        private int nextSlotID;

        public IItemContainer this[int id] => collection[id];

        public int ContainerCount => collection.Count;
        public int Capacity {
            get => collection.Count;
            set => collection.EnsureCapacity(value);
        }
        public IEnumerable<int> IDs => collection.Keys;
        public IEnumerable<IItemContainer> Containers => collection.Values;
        public Maybe<IItemContainer> ActiveContainer {
            get => activeContainer.Value;
            set => activeContainer.Value = value;
        }
        public bool IsEmpty => collection.Values.Any(x => x.ContainsItem());
        public bool IsFull => collection.Values.All(x => x.ContainsItem());

        int IItemContainerInfoItemless.ItemCount => throw new NotImplementedException();
        Maybe<IInventory> IItemContainerInfoItemless.ParentInventory { get => null!; set => _ = value; }
        bool IItemContainerInfoItemless.IsActiveContainer => throw new NotImplementedException();

        public Inventory(int initialContainerCount)
        {
            collection = new Dictionary<int, IItemContainer>(initialContainerCount);

            SetContainerCount<ItemContainer>(initialContainerCount);
        }

        public Inventory(int initialContainerCount, GameObject toInstantiate)
        {
            collection = new Dictionary<int, IItemContainer>(initialContainerCount);

            SetContainerCount(initialContainerCount, toInstantiate);
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
                                                  .Sum(x => x.ItemCount);

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

                Do.While(() => collection.ContainsKey(nextSlotID), () => nextSlotID++);

                addSubj?.OnNext((nextSlotID, itemContainer));
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        public void AddContainer(GameObject toInstantiate)
        {
            CC.Guard.IsNotNull(toInstantiate, nameof(toInstantiate));

            var cnt = UnityEngine.Object.Instantiate(toInstantiate)
                .AppealTo()
                .ByChildren()
                .IncludeInactive()
                .Model<IItemContainer>()
                .Strict();

            AddContainer(cnt);
        }

        public void AddContainerCount(int count, GameObject toInstantiate)
        {
            UnityEngine.Object.InstantiateAsync(toInstantiate, count).ToUniTask().ContinueWith(loaded =>
            {
                loaded.Select(go => go.AppealTo()
                        .ByChildren()
                        .IncludeInactive()
                        .Model<IItemContainer>()
                        .Strict())
                    .ForEach(AddContainer);
            })
            .Forget(ex => this.PrintException(ex));
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

                try
                {
                    removeSubj?.OnNext((id, value));
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
            foreach (var cnt in this.ToArray())
                RemoveContainer(cnt);
        }

        public void ClearContainers()
        {
            foreach (var cnt in this)
                cnt.Clear();
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

        public bool IsContainerActive(int id)
        {
            return ActiveContainer.Map(cnt => cnt.GetContainerID()).Contains(cntID => cntID == id);
        }

        public bool IsContainerActive(IItemContainer itemContainer)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            return ActiveContainer.Map(cnt => cnt == itemContainer).Raw;
        }

        public void ActivateContainer(int id)
        {
            if (activeContainer.Value.Contains(x => x.GetContainerID() == id))
                return;

            new Catched(onError: (_, _) => this.PrintError($"Cannot find {nameof(ItemContainer)} with id: {id}"))
                .Do(this, id, static (@this, id) => @this[id].ActivateContainer());
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

        public Maybe<int> GetContainerID(IItemContainer itemContainer)
        {
            Guard.IsNotNull(itemContainer, nameof(itemContainer));

            return collection.FirstOrDefault(x => x.Value.Equals(itemContainer))
                             .Maybe()
                             .Map(x => x.Key.Maybe(x => x >= 0))
                             .GetValue();
        }

        public IObservable<Maybe<IItemContainer>> ObserveActiveItemContainer() => activeContainer;

        public IEnumerator<IItemContainer> GetEnumerator()
        {
            return collection.Values.GetEnumerator();
        }

        public void Dispose() => Dispose(disposing: true);

        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                activeContainer.Dispose();
            }

            disposed = true;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        Maybe<IItemContainer> IItemAccessor.TakeItem(int count) => null!;

        Maybe<IItemContainer> IItemAccessor.TakeItem() => null!;

        Maybe<int> IItemContainerInfoItemless.GetContainerID() => Maybe<int>.None;

        void IItemAccessor.CopyFrom(IItemContainerInfo itemContainer)
        {
            throw new NotImplementedException();
        }

        void IItemContainerInfoItemless.ActivateContainer()
        {
            throw new NotImplementedException();
        }

        void IItemContainerInfoItemless.DeactivateContainer()
        {
            throw new NotImplementedException();
        }

        bool IItemContainerInfoItemless.SwitchContainerActiveState()
        {
            throw new NotImplementedException();
        }

        IObservable<bool> IItemContainerInfoItemless.ObserveIsActiveContainer()
        {
            return Observable.Empty<bool>();
        }

        IObservable<int> IItemContainerInfoItemless.ObserveItemCount()
        {
            return Observable.Empty<int>();
        }

    }
}
