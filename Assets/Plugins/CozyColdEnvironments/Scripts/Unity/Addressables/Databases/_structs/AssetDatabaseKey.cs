using CCEnvs.Language;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable

namespace CCEnvs.Unity.AddrsAssets.Databases
{
    [DebuggerDisplay("HashCode: {GetHashCode()}; String: {ToString()}")]
    public readonly struct AssetDatabaseKey
        : IEquatable<AssetDatabaseKey>, 
        IIDMarked<UniID>
    {
        public Ghost<Type> AssetType { get; }
        public GhostStruct<UniID> DatabaseID { get; }

        UniID IIDMarked<UniID>.ID => DatabaseID.Value();

        public AssetDatabaseKey(Type? dbAssetType)
            :
            this()
        {
            AssetType = dbAssetType!;
        }

        public AssetDatabaseKey(Type? assetType,
                                UniID id)
            :
            this(assetType)
        {
            DatabaseID = id;
        }

        public AssetDatabaseKey(IAddressablesDatabase database, UniID id)
            :
            this(database.AssetType, id)
        {
        }
        public AssetDatabaseKey(IAddressablesDatabase database)
            :
            this(database, id: default)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetDatabaseKey With(Type assetType)
        {
            return new AssetDatabaseKey(assetType, DatabaseID.Value());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetDatabaseKey With(UniID id)
        {
            return new AssetDatabaseKey(AssetType.ValueUnsafe(), id);
        }

        public bool Equals(AssetDatabaseKey other)
        {
            return AssetType == other.AssetType
                   && 
                   DatabaseID == other.DatabaseID;
        }
        public override bool Equals(object obj)
        {
            return obj is AssetDatabaseKey typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AssetType, DatabaseID);
        }

        public override string ToString()
        {
            return $"{nameof(AssetType)}: {AssetType}; {nameof(DatabaseID)}: {DatabaseID};";
        }
    }
}
