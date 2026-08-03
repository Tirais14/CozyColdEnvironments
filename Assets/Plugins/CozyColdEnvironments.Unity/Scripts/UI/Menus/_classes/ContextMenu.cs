using CCEnvs.TypeMatching;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.UnityX.UI.Menus
{
    public class ContextMenu : IContextMenu
    {
        private readonly Dictionary<string, IContextMenuItem> items = new();

        public IContextMenuItem this[string key] {
            get => items[key];
            set => items[key] = value;
        }

        public ICollection<string> Names => items.Keys;

        public ICollection<IContextMenuItem> Items => items.Values;

        public int Count => items.Count;

        bool ICollection<KeyValuePair<string, IContextMenuItem>>.IsReadOnly => false;

        void Add(IContextMenuItem item)
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
            if (!items.TryGetValue(name, out IContextMenuItem item)
                ||
                item.IsNot<T>(out var typedItem))
            {
                result = default;
                return false;
            }

            result = typedItem;
            return true;
        }
        public bool TryGetValue(string key, out IContextMenuItem value)
        {
            return items.TryGetValue(key, out value);
        }

        public void Clear() => items.Clear();

        public bool ContainsName(string key)
        {
            return items.ContainsKey(key);
        }

        public bool ContainsItem(IContextMenuItem item)
        {
            return items.ContainsValue(item);
        }

        public bool TryFind(
            string name,
            [NotNullWhen(true)] out IContextMenuItem? result,
            StringMatchSettings matchSettings = StringMatchSettings.Ordinal
            )
        {
            foreach (var item in items.Values)
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

        public IEnumerator<KeyValuePair<string, IContextMenuItem>> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
