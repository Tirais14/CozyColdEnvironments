using CCEnvs.FuncLanguage;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable

namespace CCEnvs.Unity.Databases
{
    [DebuggerDisplay("HashCode: {GetHashCode()}; String: {ToString()}")]
    public readonly struct AssetDatabaseKey
        : IEquatable<AssetDatabaseKey>,
        IIDMarked<Identifier>
    {
        public Maybe<Type> AssetType { get; }
        public Maybe<Identifier> DatabaseID { get; }

        Identifier IIDMarked<Identifier>.ID => DatabaseID.GetValue();

        public AssetDatabaseKey(Type? dbAssetType)
            :
            this()
        {
            AssetType = dbAssetType!;
        }

        public AssetDatabaseKey(Type? assetType,
                                Identifier id)
            :
            this(assetType)
        {
            DatabaseID = id;
        }

        public AssetDatabaseKey(IAssetDatabase database, Identifier id)
            :
            this(database.AssetType, id)
        {
        }
        public AssetDatabaseKey(IAssetDatabase database)
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
            return new AssetDatabaseKey(assetType, DatabaseID.GetValue());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetDatabaseKey With(Identifier id)
        {
            return new AssetDatabaseKey(AssetType.GetValueUnsafe(), id);
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
