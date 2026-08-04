using CCEnvs.Linq;
using CCEnvs.TypeMatching;
using ObservableCollections;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using CCEnvs.Threading;

#nullable enable
namespace CCEnvs.UnityX.UI.Menus
{
    public class ContextMenu : IContextMenu, IDisposable
    {
        private readonly ObservableDictionary<string, IContextMenuItem> items = new();

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        public IContextMenuItem this[string key] {
            get => items[key];
            set => items[key] = value;
        }

        public IEnumerable<string> Names => items.SelectKey();

        public IEnumerable<IContextMenuItem> Items => items.SelectValue();

        public int Count => items.Count;

        protected CancellationToken DisposeCancellationToken => disposeCancellationTokenSource.Token;

        bool ICollection<KeyValuePair<string, IContextMenuItem>>.IsReadOnly => false;

        ~ContextMenu() => Dispose();

        public void Add(IContextMenuItem item)
        {
            CC.Guard.IsNotNull(item, nameof(item));
            items.Add(item.Name, item);
        }

        public T GetValue<T>(string name) where T : IContextMenuItem
        {
            return items[name].CastTo<T>();
        }

        public bool TryGetValue<T>(
            string name,
            [NotNullWhen(true)] out T? result
            )
            where T : IContextMenuItem
        {
            if (!items.TryGetValue(name, out IContextMenuItem? item)
                ||
                item.IsNot<T>(out var typedItem))
            {
                result = default;
                return false;
            }

            result = typedItem;
            return true;
        }
        public bool TryGetValue(string key, [NotNullWhen(true)] out IContextMenuItem? value)
        {
            return items.TryGetValue(key, out value);
        }

        public void Clear() => items.Clear();

        public bool ContainsName(string key)
        {
            return items.ContainsKey(key);
        }

        public bool ContainsItem(IContextMenuItem otherItem)
        {
            foreach (var (_, item) in items)
                if (item.Equals(otherItem))
                    return true;

            return false;
        }

        public bool TryFind(
            string name,
            [NotNullWhen(true)] out IContextMenuItem? result,
            StringMatchSettings matchSettings = StringMatchSettings.Ordinal
            )
        {
            foreach (var (_, item) in items)
            {
                if (!item.Name.Match(name, matchSettings))
                    continue;

                result = item;
                return true;
            }

            result = null;
            return false;
        }

        public bool Remove(string key) => items.Remove(key);

        public Observable<IContextMenuItem> ObserveAdd()
        {
            return items.ObserveAdd(DisposeCancellationToken)
                .Select(ev => ev.Value.Value);
        }

        public Observable<IContextMenuItem> ObserveRemove()
        {
            return items.ObserveRemove(DisposeCancellationToken)
                .Select(ev => ev.Value.Value);
        }

        public Observable<PreviousCurrentPair<IContextMenuItem>> ObserveReplace()
        {
            return items.ObserveDictionaryReplace(DisposeCancellationToken)
                .Select(ev => PreviousCurrentPair.Create(ev.OldValue, ev.NewValue));
        }

        public Observable<Unit> ObserveClear()
        {
            return items.ObserveClear(DisposeCancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (disposing)
                disposeCancellationTokenSource?.CancelAndDispose();
        }

        public IEnumerator<KeyValuePair<string, IContextMenuItem>> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void ICollection<KeyValuePair<string, IContextMenuItem>>.Add(KeyValuePair<string, IContextMenuItem> item)
        {
            Add(item.Value);
        }

        bool ICollection<KeyValuePair<string, IContextMenuItem>>.Contains(KeyValuePair<string, IContextMenuItem> item)
        {
            return ((ICollection<KeyValuePair<string, IContextMenuItem>>)items).Contains(item);
        }

        void ICollection<KeyValuePair<string, IContextMenuItem>>.CopyTo(KeyValuePair<string, IContextMenuItem>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, IContextMenuItem>>)items).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, IContextMenuItem>>.Remove(KeyValuePair<string, IContextMenuItem> item)
        {
            return ((ICollection<KeyValuePair<string, IContextMenuItem>>)items).Remove(item);
        }
    }
}
