#nullable enable

using System.Collections.Generic;

namespace UTIRLib.DB
{
    public interface IDatabaseRegistry : IReadOnlyCollection<IDatabaseGroup>
    {
    }

    public interface IDatabaseRegistry<TKey> : IDatabaseRegistry
    {
        IDatabaseGroup this[TKey key] { get; }

        void AddGroup(TKey key, IDatabaseGroup databaseGroup);

        void AddGroup(IValuePair<TKey, IDatabaseGroup> keyDatabaseGroupPair);

        void AddGroups(TKey[] keys, IDatabaseGroup[] databaseGroups);

        void AddGroups(IValuePair<TKey, IDatabaseGroup>[] keyDatabaseGroupPairs);

        bool Contains(TKey key);

        IDatabaseGroup GetDatabaseGroup(TKey key);

        T? GetDatabaseGroup<T>(TKey key);
    }
}