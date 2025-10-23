using CCEnvs.Diagnostics;
using CCEnvs.Unity.AddrsAssets.Databases;
using System;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public class DatabaseNotFoundException : CCException
    {
        public DatabaseNotFoundException()
        {
        }

        public DatabaseNotFoundException(Type dbAssetType, object? dbID = null, Exception? innerException = null)
            :
            base($"{nameof(dbAssetType)}: {dbAssetType}; {nameof(dbID)}: {dbID};", innerException)
        {
        }

        public DatabaseNotFoundException(AssetDatabaseKey key, Exception? innerException = null)
            :
            this(key.AssetType.Value()!, key.DatabaseID, innerException)
        {
        }
    }
}
