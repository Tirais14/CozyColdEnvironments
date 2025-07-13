using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UTIRLib.DB
{
    public interface IDatabaseGroup
    {
        int Count { get; }

        void AddDatabase(object databaseKey, IDatabase database);

        void AddDatabase(IValuePair<object, IDatabase> keyAndDatabasePair);

        void AddDatabaseRange(object[] databaseKeys, IDatabase[] databases);

        void AddDatabaseRange(IValuePair<object, IDatabase>[] KeyAndDatabasePairs);

        bool ContainsKey(object databaseKey);

        bool Contains(IDatabase database);

        IDatabase GetDatabase(object databaseKey);

        bool TryGetDatabase(object databaseKey, [NotNullWhen(true)] out IDatabase? database);
    }

    public interface IDatabaseGroup<TDatabaseKey, TDatabase> : IDatabaseGroup, IReadOnlyCollection<TDatabase>
        where TDatabase : class, IDatabase
    {
        TDatabase this[TDatabaseKey databaseKey] { get; }

        void AddDatabase(TDatabaseKey databaseKey, TDatabase database);

        void AddDatabase(IValuePair<TDatabaseKey, TDatabase> keyAndDatabasePair);

        void AddDatabaseRange(TDatabaseKey[] databaseKeys, TDatabase[] databases);

        void AddDatabaseRange(IValuePair<TDatabaseKey, TDatabase>[] KeyAndDatabasePairs);

        bool ContainsKey(TDatabaseKey databaseKey);

        bool Contains(TDatabase database);

        TDatabase GetDatabase(TDatabaseKey databaseKey);

        bool TryGetDatabase(TDatabaseKey databaseKey, [NotNullWhen(true)] out TDatabase? database);
    }
}