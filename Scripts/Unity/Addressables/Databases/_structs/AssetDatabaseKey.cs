using CCEnvs.Unity.AddrsAssets.Databases;
using System;
using System.Diagnostics;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    [DebuggerDisplay("HashCode: {GetHashCode()}; String: {ToString()}")]
    public readonly struct AssetDatabaseKey : IEquatable<AssetDatabaseKey>
    {
        public Type DbAssetType { get; }
        public object? UniqueIndentifier { get; }

        public AssetDatabaseKey(Type dbAssetType)
            :
            this()
        {
            CC.Validate.ArgumentNull(dbAssetType, nameof(dbAssetType));

            DbAssetType = dbAssetType;
        }

        public AssetDatabaseKey(Type assetType,
                                object? uniqueIndentifier)
            :
            this(assetType)
        {
            UniqueIndentifier = uniqueIndentifier;
        }

        public AssetDatabaseKey(IAddressablesDatabase database, object? uniqueIndentifier)
            :
            this(database.AssetType, uniqueIndentifier)
        {
        }
        public AssetDatabaseKey(IAddressablesDatabase database)
            :
            this(database, uniqueIndentifier: null)
        {
        }

        public static bool operator ==(AssetDatabaseKey left, AssetDatabaseKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AssetDatabaseKey left, AssetDatabaseKey right)
        {
            return !(left == right);
        }

        public AssetDatabaseKey With(Type assetType)
        {
            return new AssetDatabaseKey(assetType, UniqueIndentifier);
        }

        public AssetDatabaseKey With(object? uniqueIdentifier)
        {
            return new AssetDatabaseKey(DbAssetType, uniqueIdentifier);
        }

        public bool Equals(AssetDatabaseKey other)
        {
            return DbAssetType == other.DbAssetType
                   && 
                   UniqueIndentifier == other.UniqueIndentifier;
        }
        public override bool Equals(object obj)
        {
            return obj is AssetDatabaseKey typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DbAssetType, UniqueIndentifier);
        }

        public override string ToString()
        {
            return $"{nameof(DbAssetType)}: {DbAssetType} | {nameof(UniqueIndentifier)}: {UniqueIndentifier}";
        }
    }
}
