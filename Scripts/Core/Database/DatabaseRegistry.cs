using System.Collections;
using System.Collections.Generic;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;
using UTIRLib.Linq;

#nullable enable

namespace UTIRLib.DB
{
    /// <summary>
    /// Used to load and access the game's databases
    /// </summary>
    public class DatabaseRegistry<TKey> : IDatabaseRegistry<TKey>
    {
        protected readonly Dictionary<TKey, IDatabaseGroup> databaseGroups = new();

        public int Count => databaseGroups.Count;
        public IDatabaseGroup this[TKey key] => databaseGroups[key];

        public DatabaseRegistry()
        { }

        public DatabaseRegistry(TKey[] keys, IDatabaseGroup[] databaseGroups) =>
            AddGroups(keys, databaseGroups);

        public DatabaseRegistry(params IValuePair<TKey, IDatabaseGroup>[] keyDatabaseGroupPairs)
        {
            if (keyDatabaseGroupPairs.IsEmpty()) return;

            AddGroups(keyDatabaseGroupPairs);
        }

        public void AddGroup(TKey key, IDatabaseGroup databaseGroup) => databaseGroups.Add(key, databaseGroup);

        public void AddGroup(IValuePair<TKey, IDatabaseGroup> keyDatabaseGroupPair) =>
            AddGroup(keyDatabaseGroupPair.First, keyDatabaseGroupPair.Second);

        /// <exception cref="CollectionArgumentException"></exception>
        /// <exception cref="WrongCollectionException"></exception>
        public void AddGroups(TKey[] keys, IDatabaseGroup[] databaseGroups)
        {
            if (keys.IsNullOrEmpty())
            {
                throw new CollectionArgumentException(nameof(keys), keys);
            }
            else if (databaseGroups.IsNullOrEmpty())
            {
                throw new CollectionArgumentException(nameof(databaseGroups), databaseGroups);
            }
            else if (keys.Length != databaseGroups.Length)
            {
                throw new WrongCollectionException("Arrays must be the same length.");
            }

            for (int i = 0; i < keys.Length; i++)
            {
                AddGroup(keys[i], databaseGroups[i]);
            }
        }

        /// <exception cref="NullOrEmptyCollectionException"></exception>
        public void AddGroups(IValuePair<TKey, IDatabaseGroup>[] keyDatabaseGroupPairs)
        {
            if (keyDatabaseGroupPairs.IsNullOrEmpty())
            {
                throw new CollectionArgumentException(nameof(keyDatabaseGroupPairs), keyDatabaseGroupPairs);
            }

            for (int i = 0; i < keyDatabaseGroupPairs.Length; i++)
            {
                AddGroup(keyDatabaseGroupPairs[i]);
            }
        }

        public bool Contains(TKey key) => databaseGroups.ContainsKey(key);

        public IDatabaseGroup GetDatabaseGroup(TKey key) => databaseGroups[key];

        public T? GetDatabaseGroup<T>(TKey key) => databaseGroups[key].IsQ<T>();

        public IEnumerator<IDatabaseGroup> GetEnumerator() => databaseGroups.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}