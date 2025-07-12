using UnityEngine;

#nullable enable

namespace UTIRLib.DB
{
    public sealed class AssetDatabaseGroupScriptableObject : AssetDatabaseGroup<
        AssetDatabaseScriptableObject,
        AssetDatabaseItemScriptableObject,
        ScriptableObject>
    {
        public AssetDatabaseGroupScriptableObject() : base()
        { }

        public AssetDatabaseGroupScriptableObject(string databaseKey, AssetDatabaseScriptableObject database)
            : base(databaseKey, database)
        { }

        public AssetDatabaseGroupScriptableObject(IValuePair<string, AssetDatabaseScriptableObject>[] keyAndDatabases)
            : base(keyAndDatabases)
        { }
    }
}