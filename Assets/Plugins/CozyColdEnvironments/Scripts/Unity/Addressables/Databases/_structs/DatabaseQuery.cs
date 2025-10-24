using CCEnvs.Language;
using CCEnvs.Reflection;
using CCEnvs.Unity.Extensions;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using ZLinq;

#nullable enable
#pragma warning disable S1117
#pragma warning disable S3236
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public record DatabaseQuery
    {
        private Maybe<IAddressablesDatabaseRegistry> dbRegistry;
        private Maybe<IAddressablesDatabase> db;
        private AssetDatabaseKey dbKey;
        private AssetKey key;

        public DatabaseQuery()
        {
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnityEngine.Object GetAsset(Type assetType)
        {
            CC.Guard.NullArgument(assetType, nameof(assetType));
            Validate();

            dbKey = dbKey.With(assetType);

            return dbRegistry.Match(
                reg => reg[dbKey, key],
                () => db.Map(x => x[key]).AccessUnsafe()
                ).AccessUnsafe();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetAsset<T>(Type? assetType = null)
            where T : UnityEngine.Object
        {
            Validate();

            dbKey = dbKey.With(assetType ?? typeof(T));

            return dbRegistry.Match(
                reg => reg.GetAsset<T>(dbKey, key),
                () => db.Map(x => x.GetAsset<T>(key)).AccessUnsafe()
                ).AccessUnsafe();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TSecondary GetAsset<TPrimary, TSecondary>(Type? assetType = null)
            where TPrimary : UnityEngine.Object
        {
            Validate();
            assetType ??= typeof(TPrimary);

            if (assetType == typeof(GameObject))
                return GetAsset<TPrimary>(assetType).As<GameObject>()
                                                    .GetAssignedObject<TSecondary>()!;

            return GetAsset<TPrimary>(assetType).As<TSecondary>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<UnityEngine.Object> FindAsset(Type assetType)
        {
            CC.Guard.NullArgument(assetType, nameof(assetType));
            Validate();

            dbKey = dbKey.With(assetType);

            return dbRegistry.Match(
                reg => reg.FindAsset(dbKey, key),
                () => db.Map(x => x.FindAsset(key).Access())!
                ).Access();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> FindAsset<T>(Type? assetType = null)
            where T : UnityEngine.Object
        {
            Validate();

            dbKey = dbKey.With(assetType ?? typeof(T));

            return dbRegistry.Match(
                reg => reg.FindAsset<T>(dbKey, key),
                () => db.Map(x => x.FindAsset<T>(key).Access())!
                ).Access();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TSecondary FindAsset<TPrimary, TSecondary>(Type? assetType = null)
            where TPrimary : UnityEngine.Object
        {
            Validate();
            assetType ??= typeof(TPrimary);

            if (assetType == typeof(GameObject))
                return GetAsset<TPrimary>(assetType).As<GameObject>()
                                                    .GetAssignedObject<TSecondary>()!;

            return GetAsset<TPrimary>(assetType).As<TSecondary>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DatabaseQuery WithDatabaseID(UniID id)
        {
            dbKey = new AssetDatabaseKey(dbKey.AssetType.Access(), id);
            key = new AssetKey(key.AssetName.Access(), key.AssetID);

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DatabaseQuery WithDatabaseID(int num0,
                                            int num1,
                                            string str0)
        {
            return WithDatabaseID(num0, num1, str0, string.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DatabaseQuery WithDatabaseID(int num0, int num1)
        {
            return WithDatabaseID(num0, num1, string.Empty, string.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DatabaseQuery WithDatabaseID(int num0)
        {
            return WithDatabaseID(num0, default, string.Empty, string.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DatabaseQuery WithDatabaseID(int num0,
                                            string str0,
                                            string str1)
        {
            return WithDatabaseID(num0, default, str0, str1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DatabaseQuery WithDatabaseID(string str0,
                                            string str1)
        {
            return WithDatabaseID(default, default, str0, str1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DatabaseQuery WithDatabaseID(string str0)
        {
            return WithDatabaseID(default, default, str0, string.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DatabaseQuery WithDatabaseID(Enum value0)
        {
            return WithDatabaseID(UniID.FromEnum(value0));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DatabaseQuery WithDatabaseID(Enum value0, Enum value1)
        {
            return WithDatabaseID(UniID.FromEnum(value0, value1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DatabaseQuery WithDatabaseID<T0>(T0 value0)
            where T0 : struct, Enum
        {
            return WithDatabaseID(UniID.FromEnum(value0));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DatabaseQuery WithDatabaseID<T0, T1>(T0 value0, T1 value1)
             where T0 : struct, Enum
             where T1 : struct, Enum
        {
            return WithDatabaseID(UniID.FromEnum(value0, value1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DatabaseQuery WithAssetName(string? assetName)
        {
            key = new AssetKey(assetName, key.AssetID);

            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DatabaseQuery WithAssetID(int assetID)
        {
            key = new AssetKey(key.AssetName.Access(), assetID);

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DatabaseQuery In(IAddressablesDatabaseRegistry? dbRegistry)
        {
            this.dbRegistry = dbRegistry.Maybe()!;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DatabaseQuery In(IAddressablesDatabase? db)
        {
            this.db = db.Maybe()!;

            return this;
        }

        public DatabaseQuery Reset()
        {
            dbRegistry = default;
            db = default;
            dbKey = default;
            key = default;

            return this;
        }

        private void Validate()
        {
            if (db.IsNone && dbRegistry.IsNone)
                throw new InvalidOperationException($"{nameof(db)} and {nameof(dbRegistry)} is not setted.");
        }
    }
}
