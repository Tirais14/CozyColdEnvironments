using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

#nullable enable

namespace UTIRLib.DB
{
    public class DatabaseGroup<TDatabaseKey, TDatabase> : IDatabaseGroup<TDatabaseKey, TDatabase>
        where TDatabase : class, IDatabase
    {
        protected readonly Dictionary<TDatabaseKey, TDatabase> dbs = new();

        public int Count => dbs.Count;
        public TDatabase this[TDatabaseKey databaseKey] => GetDatabase(databaseKey);

        public void AddDatabase(object databaseKey, IDatabase database) =>
            dbs.Add(databaseKey.Convert<TDatabaseKey>() ?? throw new NullReferenceException(),
            database as TDatabase ?? throw new NullReferenceException());

        public void AddDatabase(IValuePair<object, IDatabase> keyAndDatabasePair) =>
            AddDatabase(keyAndDatabasePair.First, keyAndDatabasePair.Second);

        public void AddDatabase(TDatabaseKey databaseKey, TDatabase database) => dbs.Add(databaseKey, database);

        public void AddDatabase(IValuePair<TDatabaseKey, TDatabase> keyAndDatabasePair) =>
            AddDatabase(keyAndDatabasePair.First, keyAndDatabasePair.Second);

        /// <exception cref="CollectionArgumentException"></exception>
        /// <exception cref="WrongCollectionException"></exception>
        public void AddDatabaseRange(object[] databaseKeys, IDatabase[] databases)
        {
            if (databaseKeys.IsNullOrEmpty())
            {
                throw new CollectionArgumentException(nameof(databaseKeys), databaseKeys);
            }
            else if (databases.IsNullOrEmpty())
            {
                throw new CollectionArgumentException(nameof(databases), databases);
            }
            else if (databaseKeys.Length != databases.Length)
            {
                throw new WrongCollectionException("Key and database arrays must be the same length.");
            }

            for (int i = 0; i < databaseKeys.Length; i++)
            {
                AddDatabase(databaseKeys[i], databases[i]);
            }
        }

        public void AddDatabaseRange(IValuePair<object, IDatabase>[] KeyAndDatabasePairs)
        {
            if (KeyAndDatabasePairs.IsNullOrEmpty())
            {
                throw new CollectionArgumentException(nameof(KeyAndDatabasePairs), KeyAndDatabasePairs);
            }

            for (int i = 0; i < KeyAndDatabasePairs.Length; i++)
            {
                AddDatabase(KeyAndDatabasePairs[i]);
            }
        }

        public void AddDatabaseRange(TDatabaseKey[] databaseKeys, TDatabase[] databases)
        {
            if (databaseKeys.IsNullOrEmpty())
            {
                throw new CollectionArgumentException(nameof(databaseKeys), databaseKeys);
            }
            else if (databases.IsNullOrEmpty())
            {
                throw new CollectionArgumentException(nameof(databases), databases);
            }
            else if (databaseKeys.Length != databases.Length)
            {
                throw new ArgumentException("Key and database arrays must be the same length.");
            }

            for (int i = 0; i < databaseKeys.Length; i++)
            {
                AddDatabase(databaseKeys[i], databases[i]);
            }
        }

        public void AddDatabaseRange(IValuePair<TDatabaseKey, TDatabase>[] KeyAndDatabasePairs)
        {
            if (KeyAndDatabasePairs.IsNullOrEmpty())
            {
                throw new CollectionArgumentException(nameof(KeyAndDatabasePairs), KeyAndDatabasePairs);
            }

            for (int i = 0; i < KeyAndDatabasePairs.Length; i++)
            {
                AddDatabase(KeyAndDatabasePairs[i]);
            }
        }

        public TDatabase GetDatabase(TDatabaseKey databaseKey) => dbs[databaseKey];

        public IDatabase GetDatabase(object databaseKey) => GetDatabase((TDatabaseKey)databaseKey);

        public bool TryGetDatabase(TDatabaseKey databaseKey, out TDatabase database) => dbs.TryGetValue(databaseKey, out database);

        public bool TryGetDatabase(object databaseKey, [NotNullWhen(true)] out IDatabase? database)
        {
            if (dbs.TryGetValue((TDatabaseKey)databaseKey, out TDatabase databaseTyped))
            {
                database = databaseTyped;
                return true;
            }

            database = null;
            return false;
        }

        public bool ContainsKey(TDatabaseKey databaseKey) => dbs.ContainsKey(databaseKey);

        public bool ContainsKey(object databaseKey)
        {
            if (databaseKey is not TDatabaseKey)
            {
                throw new ArgumentException($"Key must be convertible to {typeof(TDatabase).Name}.");
            }

            return ContainsKey((TDatabaseKey)databaseKey);
        }

        public bool Contains(TDatabase database) => dbs.ContainsValue(database);

        public bool Contains(IDatabase database)
        {
            if (database is not TDatabase)
            {
                throw new ArgumentException($"Database must be convertible to {typeof(TDatabase).Name}.");
            }

            return Contains((TDatabase)database);
        }

        public IEnumerator<TDatabase> GetEnumerator() => dbs.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}