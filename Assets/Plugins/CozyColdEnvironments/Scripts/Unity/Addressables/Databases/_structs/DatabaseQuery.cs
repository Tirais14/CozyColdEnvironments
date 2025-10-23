using CCEnvs.Language;
using CCEnvs.Reflection;
using CCEnvs.Unity.Extensions;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZLinq;

#nullable enable
#pragma warning disable S1117
#pragma warning disable S3236
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public record DatabaseQuery
    {
        private IEnumerable<IAddressablesDatabase>? dbs;
        private AssetDatabaseKey dbKey;
        private AssetKey key;

        public DatabaseQuery()
        {
        }

        public UnityEngine.Object GetAsset(Type assetType)
        {
            CC.Guard.NullArgument(assetType, nameof(assetType));

            var dbs = this.dbs.ZL();

            return dbs.Select(x => (db: x, assets: x.Values))
                      .FirstOrDefault(x => x.assets.Any(y => y.GetType() == assetType))
                      .ToGhost()
                      .Match(
                       x => x.db,
                       () => dbs.FirstOrDefault(x => x.GetType().IsType(assetType)))
                      .Map(x => x!.GetAsset(key))
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
            assetType ??= typeof(TPrimary);

            if (assetType == typeof(GameObject))
                return GetAsset<TPrimary>(assetType).As<GameObject>()
                                                    .GetAssignedObject<TSecondary>()!;

            return GetAsset<TPrimary>(assetType).As<TSecondary>();
        }

        public Ghost<UnityEngine.Object?> FindAsset(Type assetType)
        {
            CC.Guard.NullArgument(assetType, nameof(assetType));

            var dbs = this.dbs.ZL();

            return dbs.Select(x => (db: x, assets: x.Values))
                      .FirstOrDefault(x => x.assets.Any(y => y.GetType() == assetType))
                      .ToGhost()
                      .Match(
                       x => x.db,
                       () => dbs.FirstOrDefault(x => x.GetType().IsType(assetType)))
                      .Map(x => x!.FindAsset(key))
                      .SelectMany(x => x)
                      .First();
        }
        public T FindAsset<T>(Type? assetType = null)
            where T : UnityEngine.Object
        {
            return GetAsset(assetType ?? typeof(T)).As<T>();
        }
        public TSecondary FindAsset<TPrimary, TSecondary>(Type? assetType = null)
            where TPrimary : UnityEngine.Object
        {
            assetType ??= typeof(TPrimary);

            if (assetType == typeof(GameObject))
                return GetAsset<TPrimary>(assetType).As<GameObject>()
                                                    .GetAssignedObject<TSecondary>()!;

            return GetAsset<TPrimary>(assetType).As<TSecondary>();
        }

        public DatabaseQuery DbID(UniID id)
        {
            dbKey = new AssetDatabaseKey(dbKey.AssetType.Value, id);
            key = new AssetKey(key.AssetName, key.AssetID);

            return this;
        }
        public DatabaseQuery DbID(int num0,
                                            int num1,
                                            string str0,
                                            string str1)
        {
            return DbID(new UniID
            {
                Num0 = num0,
                Num1 = num1,
                Str0 = str0,
                Str1 = str1
            });
        }
        public DatabaseQuery DbID(int num0,
                                            int num1,
                                            string str0)
        {
            return DbID(num0, num1, str0, string.Empty);
        }
        public DatabaseQuery DbID(int num0, int num1)
        {
            return DbID(num0, num1, string.Empty, string.Empty);
        }
        public DatabaseQuery DbID(int num0)
        {
            return DbID(num0, default, string.Empty, string.Empty);
        }
        public DatabaseQuery DbID(int num0,
                                            string str0,
                                            string str1)
        {
            return DbID(num0, default, str0, str1);
        }
        public DatabaseQuery DbID(string str0,
                                            string str1)
        {
            return DbID(default, default, str0, str1);
        }
        public DatabaseQuery DbID(string str0)
        {
            return DbID(default, default, str0, string.Empty);
        }

        public DatabaseQuery DbID(Enum value0)
        {
            return DbID(UniID.FromEnum(value0));
        }
        public DatabaseQuery DbID(Enum value0, Enum value1)
        {
            return DbID(UniID.FromEnum(value0, value1));
        }

        public DatabaseQuery DbID<T0>(T0 value0)
            where T0 : struct, Enum
        {
            return DbID(UniID.FromEnum(value0));
        }
        public DatabaseQuery DbID<T0, T1>(T0 value0, T1 value1)
             where T0 : struct, Enum
             where T1 : struct, Enum
        {
            return DbID(UniID.FromEnum(value0, value1));
        }

        public DatabaseQuery AssetName(string? assetName)
        {
            key = new AssetKey(assetName, key.AssetID);

            return this;
        }
        public DatabaseQuery AssetID(int assetID)
        {
            key = new AssetKey(key.AssetName, assetID);

            return this;
        }

        public DatabaseQuery DBs(IEnumerable<IAddressablesDatabase> dbs)
        {
            Guard.IsNotNull(dbs, nameof(dbs));

            this.dbs = dbs;

            return this;
        }

        public DatabaseQuery Reset()
        {
            dbs = default!;
            dbKey = default;
            key = default;

            return this;
        }
    }
}
