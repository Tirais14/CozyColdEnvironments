using CCEnvs.Collections;
using CCEnvs.Linq;
using CCEnvs.Unity.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ZLinq;

#nullable enable
#pragma warning disable S3881
#pragma warning disable IDE1006
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public abstract class AddressablesDatabaseRegistry<TThis>
        : CCBehaviourStaticPublic<TThis>,
        IAddressablesDatabaseRegistry

        where TThis : CCBehaviourStatic, IAddressablesDatabaseRegistry
    {
        private readonly CCDictionary<Identifier, IAddressablesDatabase> collection = new();
        private readonly AddressablesDatabaseSearch search = new();

        public Result<IAddressablesDatabase> this[Identifier key] {
            get => collection[key];
            set => collection[key] = value;
        }

        public IEnumerable<Identifier> Keys => collection.Keys;
        public IEnumerable<IAddressablesDatabase> Values => collection.Values;
        public int Count => collection.Count;

        bool ICollection<KeyValuePair<Identifier, IAddressablesDatabase>>.IsReadOnly => false;

        public AddressablesDatabaseSearch Search()
        {
            return search.Reset().From(this);
        }

        public void Add(Identifier key, IAddressablesDatabase value)
        {
            if (key.Number.IsNone)
            {
                key = key.WithNumber(0);

                if (collection.ContainsKey(key))
                {
                    int nextNumber = Keys.Where(other => other.Text == key.Text).Max(key => key.Number.Raw) + 1;
                    key = key.WithNumber(nextNumber);
                }
            }

            collection.Add(key, value);
        }

        public void Add(KeyValuePair<Identifier, IAddressablesDatabase> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Remove(Identifier key)
        {
            return collection.Remove(key);
        }

        public bool Remove(KeyValuePair<Identifier, IAddressablesDatabase> item)
        {
            return collection.Remove(item.Key);
        }

        public bool ContainsKey(Identifier key)
        {
            return collection.ContainsKey(key);
        }

        public bool Contains(KeyValuePair<Identifier, IAddressablesDatabase> item)
        {
            return collection.Contains(item);
        }

        public void CopyTo(KeyValuePair<Identifier, IAddressablesDatabase>[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        public void Clear()
        {
            collection.Clear();
        }

        private bool disposed;
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                collection.Values.DisposeAll();
                collection.Clear();
            }

            disposed = true;
        }

        public IEnumerator<KeyValuePair<Identifier, IAddressablesDatabase>> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
