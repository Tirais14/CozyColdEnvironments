#nullable enable

namespace UTIRLib.DB
{
    public sealed class AssetDatabaseGroupGeneric : AssetDatabaseGroup<
        AssetDatabaseGeneric,
        AssetDatabaseItemGeneric,
        UnityEngine.Object>
    {
        public AssetDatabaseGroupGeneric() : base()
        { }

        public AssetDatabaseGroupGeneric(string databaseKey, AssetDatabaseGeneric database)
            : base(databaseKey, database)
        { }

        public AssetDatabaseGroupGeneric(IValuePair<string, AssetDatabaseGeneric>[] keyAndDatabases)
            : base(keyAndDatabases)
        { }
    }
}