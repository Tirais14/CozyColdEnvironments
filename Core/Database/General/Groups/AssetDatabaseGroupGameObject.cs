using UnityEngine;

#nullable enable

namespace UTIRLib.DB
{
    public sealed class AssetDatabaseGroupGameObject : AssetDatabaseGroup<
        AssetDatabaseGameObject,
        AssetDatabaseItemGameObject,
        GameObject>
    {
        public AssetDatabaseGroupGameObject() : base()
        { }

        public AssetDatabaseGroupGameObject(string databaseKey, AssetDatabaseGameObject database)
            : base(databaseKey, database)
        { }

        public AssetDatabaseGroupGameObject(IValuePair<string, AssetDatabaseGameObject>[] keyAndDatabases)
            : base(keyAndDatabases)
        { }
    }
}