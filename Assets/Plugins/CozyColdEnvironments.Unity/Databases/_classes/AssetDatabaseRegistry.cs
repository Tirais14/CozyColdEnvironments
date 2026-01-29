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
namespace CCEnvs.Unity.Databases
{
    public abstract class AssetDatabaseRegistry<TThis>
        : CCBehaviourStaticPublic<TThis>,
        IAssetDatabaseRegistry

        where TThis : CCBehaviourStatic, IAssetDatabaseRegistry
    {
        private readonly CCDictionary<Identifier, IAssetDatabase> collection = new();
        private readonly AssetDatabaseQuery query = new();

        public Result<IAssetDatabase> this[Identifier key] {
            get => collection[key];
            set => collection[key] = value;
        }

        public IEnumerable<Identifier> Keys => collection.Keys;
        public IEnumerable<IAssetDatabase> Values => collection.Values;
        public int Count => collection.Count;

        bool ICollection<KeyValuePair<Identifier, IAssetDatabase>>.IsReadOnly => false;

        public AssetDatabaseQuery Query()
        {
            return query.Reset().From(this);
        }

        public void Add(Identifier key, IAssetDatabase value)
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

        public void Add(KeyValuePair<Identifier, IAssetDatabase> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Remove(Identifier key)
        {
            return collection.Remove(key);
        }

        public bool Remove(KeyValuePair<Identifier, IAssetDatabase> item)
        {
            return collection.Remove(item.Key);
        }

        public bool ContainsKey(Identifier key)
        {
            return collection.ContainsKey(key);
        }

        public bool Contains(KeyValuePair<Identifier, IAssetDatabase> item)
        {
            return collection.Contains(item);
        }

        public void CopyTo(KeyValuePair<Identifier, IAssetDatabase>[] array, int arrayIndex)
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
                collection.Values.DisposeEach();
                collection.Clear();
            }

            disposed = true;
        }

        public IEnumerator<KeyValuePair<Identifier, IAssetDatabase>> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
