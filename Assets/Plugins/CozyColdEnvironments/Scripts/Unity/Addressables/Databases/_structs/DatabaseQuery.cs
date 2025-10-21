using CCEnvs.Language;
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public record DatabaseQuery
    {
        public IEnumerable<IAddressablesDatabase> DBs { get; }
        public AssetDatabaseKey DbKey { get; }
        public AssetKey Key { get; }

        public DatabaseQuery(IEnumerable<IAddressablesDatabase> dbs)
        {
            DBs = dbs;
        }

        public DatabaseQuery(
            IEnumerable<IAddressablesDatabase> dbs,
            AssetDatabaseKey dbKey,
            AssetKey key)
            :
            this(dbs)
        {
            DbKey = dbKey;
            Key = key;
        }

        public UnityEngine.Object GetAsset(Type assetType)
        {
            CC.Guard.NullArgument(assetType, nameof(assetType));

            var dbs = DBs.ZL();

            return dbs.Select(x => (db: x, assets: x.Values))
                      .FirstOrDefault(x => x.assets.Any(y => y.GetType() == assetType))
                      .AsGhost()
                      .Match(
                       x => x.db,
                       () => dbs.FirstOrDefault(x => x.GetType().IsType(assetType)))
                      .Map(x => x!.GetAsset(Key))
                      .ValueUnsafe();
        }
        public T GetAsset<T>(Type? assetType = null)
            where T : UnityEngine.Object
        {
            return GetAsset(assetType ?? typeof(T)).As<T>();
        }
        public TSecondary GetAsset<TPrimary, TSecondary>(Type? assetType = null)
            where TPrimary : UnityEngine.Object
        {
            return GetAsset<TPrimary>(assetType).As<TSecondary>();
        }

        public UnityEngine.Object FindAsset(Type assetType)
        {
            CC.Guard.NullArgument(assetType, nameof(assetType));

            var dbs = DBs.ZL();

            return dbs.Select(x => (db: x, assets: x.Values))
                      .FirstOrDefault(x => x.assets.Any(y => y.GetType() == assetType))
                      .AsGhost()
                      .Match(
                       x => x.db,
                       () => dbs.FirstOrDefault(x => x.GetType().IsType(assetType)))
                      .Map(x => x!.FindAsset(Key))
                      .ValueUnsafe();
        }
        public T FindAsset<T>(Type? assetType = null)
            where T : UnityEngine.Object
        {
            return GetAsset(assetType ?? typeof(T)).As<T>();
        }
        public TSecondary FindAsset<TPrimary, TSecondary>(Type? assetType = null)
            where TPrimary : UnityEngine.Object
        {
            return GetAsset<TPrimary>(assetType).As<TSecondary>();
        }

        public DatabaseQuery WithDatabaseID(UniID id)
        {
            return new DatabaseQuery(DBs,
                new AssetDatabaseKey(DbKey.AssetType, id),
                new AssetKey(Key.AssetName, Key.AssetID));
        }
        public DatabaseQuery WithDatabaseID(int num0,
                                            int num1,
                                            string str0,
                                            string str1)
        {
            return WithDatabaseID(new UniID
            {
                Num0 = num0,
                Num1 = num1,
                Str0 = str0,
                Str1 = str1
            });
        }
        public DatabaseQuery WithDatabaseID(int num0,
                                            int num1,
                                            string str0)
        {
            return WithDatabaseID(num0, num1, str0, string.Empty);
        }
        public DatabaseQuery WithDatabaseID(int num0, int num1)
        {
            return WithDatabaseID(num0, num1, string.Empty, string.Empty);
        }
        public DatabaseQuery WithDatabaseID(int num0)
        {
            return WithDatabaseID(num0, default, string.Empty, string.Empty);
        }
        public DatabaseQuery WithDatabaseID(int num0,
                                            string str0,
                                            string str1)
        {
            return WithDatabaseID(num0, default, str0, str1);
        }
        public DatabaseQuery WithDatabaseID(string str0,
                                            string str1)
        {
            return WithDatabaseID(default, default, str0, str1);
        }
        public DatabaseQuery WithDatabaseID(string str0)
        {
            return WithDatabaseID(default, default, str0, string.Empty);
        }

        public DatabaseQuery WithDatabaseID(Enum value0)
        {
            return WithDatabaseID(UniID.FromEnum(value0));
        }
        public DatabaseQuery WithDatabaseID(Enum value0, Enum value1)
        {
            return WithDatabaseID(UniID.FromEnum(value0, value1));
        }

        public DatabaseQuery WithDatabaseID<T0>(T0 value0)
            where T0 : struct, Enum
        {
            return WithDatabaseID(UniID.FromEnum(value0));
        }
        public DatabaseQuery WithDatabaseID<T0, T1>(T0 value0, T1 value1)
             where T0 : struct, Enum
             where T1 : struct, Enum
        {
            return WithDatabaseID(UniID.FromEnum(value0, value1));
        }

        public DatabaseQuery WithAssetName(string? assetName)
        {
            return new DatabaseQuery(DBs,
                DbKey,
                new AssetKey(assetName, Key.AssetID));
        }
        public DatabaseQuery WithAssetName(int assetID)
        {
            return new DatabaseQuery(DBs,
                DbKey,
                new AssetKey(Key.AssetName, assetID));
        }
    }
}
