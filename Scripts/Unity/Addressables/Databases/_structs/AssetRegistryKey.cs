using CCEnvs.Unity.AddrsAssets.Databases;
using System;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public readonly struct AssetRegistryKey : IEquatable<AssetRegistryKey>
    {
        private readonly Type assetType;
        private readonly object? uniqueIndentifier;

        public AssetKey AssetKey { get; }

        public AssetRegistryKey(AssetKey assetKey, Type assetType)
            :
            this()
        {
            CC.Validate.ArgumentNull(assetType, nameof(assetType));

            this.AssetKey = assetKey;
            this.assetType = assetType;
        }

        public AssetRegistryKey(AssetKey assetKey,
                                Type assetType,
                                object? uniqueIndentifier) 
            : 
            this(assetKey, assetType)
        {
            this.uniqueIndentifier = uniqueIndentifier;
        }

        public static AssetRegistryKey From(IAddressablesDatabase database)
        {
            CC.Validate.ArgumentNull(database, nameof(database));

            return new AssetRegistryKey(assetKey: default, database.AssetType);
        }

        public static bool operator ==(AssetRegistryKey left, AssetRegistryKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AssetRegistryKey left, AssetRegistryKey right)
        {
            return !(left == right);
        }

        public AssetRegistryKey With(Type assetType)
        {
            return new AssetRegistryKey(AssetKey, assetType, uniqueIndentifier);
        }

        public bool Equals(AssetRegistryKey other)
        {
            return ToString() == other.ToString();
        }
        public override bool Equals(object obj)
        {
            return obj is AssetRegistryKey typed && Equals(typed);
        }

        public override string ToString()
        {
            return $"{nameof(AddrsAssets.AssetKey)}: {AssetKey}. Database asset type = {assetType.FullName}; database unique indentifier = {uniqueIndentifier}";
        }

        public override int GetHashCode() => ToString().GetHashCode();
    }
}
