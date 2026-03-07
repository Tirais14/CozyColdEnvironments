using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using CCEnvs.Collections;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using ZLinq;

#nullable enable
#pragma warning disable S1117
#pragma warning disable S3236
namespace CCEnvs.Unity.Databases
{
    public struct AssetDatabaseQuery
    {
        public object? Target { get; set; }
        public Identifier? ID { get; set; }
        public StringMatchSettings TextMatchSettings { get; set; }

        public static AssetDatabaseQuery Create()
        {
            return new AssetDatabaseQuery().Reset();
        }

        private static bool FilterType(Type left, Type? right)
        {
            return right is null || left.IsType(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetDatabaseQuery From(IAssetDatabase? db)
        {
            CC.Guard.IsNotNull(db, nameof(db));

            Target = db;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetDatabaseQuery From(IAssetDatabaseRegistry? reg)
        {
            CC.Guard.IsNotNull(reg, nameof(reg));

            Target = reg;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetDatabaseQuery ByPartialName(bool state = true)
        {
            if (state)
                TextMatchSettings |= StringMatchSettings.Partial;
            else
                TextMatchSettings &= ~StringMatchSettings.Partial;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetDatabaseQuery ByTextID(string? name = null)
        {
            ID = ID.GetValueOrDefault().WithText(name);

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetDatabaseQuery ByNumberID(int? number = default)
        {
            ID = ID.GetValueOrDefault().WithNumber(number);

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetDatabaseQuery ByID(Identifier id = default)
        {
            ID = id;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetDatabaseQuery ByEnumID<T>(T input)
            where T : unmanaged, Enum
        {
            var id = Identifier.Create(input);

            ByNumberID(id.Number);
            ByTextID(id.Text);

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetDatabaseQuery InDatabase(Type? type = null, Type? assetType = null)
        {
            Target = Database(type: type, assetType: assetType).Strict();

            ResetID();

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IEnumerable<IAssetDatabase> Databases(Type? type = null, Type? assetType = null)
        {
            return new RegistryEnumerator(
                this,
                typeFilter: type,
                assetTypeFilter: assetType
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Result<IAssetDatabase> Database(Type? assetType = null, Type? type = null)
        {
            var dbs = Databases(type, assetType);

            return (dbs.SingleOrDefault(), new DatabaseNotFoundException(ID, (IAssetDatabaseRegistry)Target!, assetType));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Result<IAssetDatabase<TAssetType>> Database<TAssetType>(Type? dbType = null)
        {
            return Database(assetType: typeof(TAssetType), type: dbType).Cast<IAssetDatabase<TAssetType>>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IEnumerable<object> Assets(Type? type = null)
        {
            return new DatabaseEnumerator(this, type);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IEnumerable<T> Assets<T>()
        {
            return Assets(typeof(T)).Cast<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Result<object> Asset(Type? type = null)
        {
            return (Assets(type).FirstOrDefault(), new AssetNotFoundException((IAssetDatabase)Target!, ID, type));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Result<T> Asset<T>()
        {
            return Asset(typeof(T)).Cast<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetDatabaseQuery ResetID()
        {
            ID = default;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetDatabaseQuery Reset()
        {
            Target = null;
            ResetID();
            TextMatchSettings = StringMatchSettings.Default;

            return this;
        }

        public struct RegistryEnumerator : IEnumerator<IAssetDatabase>, IEnumerable<IAssetDatabase>
        {
            private readonly AssetDatabaseQuery query;

            private readonly Type? typeFilter;

            private readonly Type? assetTypeFilter;

            private readonly IAssetDatabaseRegistry registry;

            private readonly IEnumerator<KeyValuePair<Identifier, IAssetDatabase>> regEnumerator;

            public IAssetDatabase Current { get; private set; }

            readonly object IEnumerator.Current => Current;

            public RegistryEnumerator(AssetDatabaseQuery query, Type? typeFilter, Type? assetTypeFilter)
            {
                CC.Guard.IsNotNullTarget(query.Target);

                if (query.Target.IsNot<IAssetDatabaseRegistry>(out var registry))
                    throw new ArgumentException($"The {nameof(query.Target)} type must be a {nameof(IAssetDatabaseRegistry)}");

                this.registry = registry;
                regEnumerator = registry.GetEnumerator();

                Current = null!;

                this.query = query;
                this.typeFilter = typeFilter;
                this.assetTypeFilter = assetTypeFilter;
            }

            public bool MoveNext()
            {
                var loopFuse = LoopFuse.Create();

                while (regEnumerator.TryMoveNext(out var item)
                       &&
                       loopFuse.DebugMoveNext())
                {
                    Current = item.Value;

                    if (query.ID != null)
                    {
                        if (item.Key != query.ID)
                            continue;
                    }

                    if (!FilterType())
                        continue;

                    if (!FilterAssetType())
                        continue;

                    return true;
                }

                return false;
            }

            public readonly void Reset()
            {
                throw new NotSupportedException(nameof(Reset));
            }

            public readonly void Dispose()
            {
            }

            public readonly IEnumerator<IAssetDatabase> GetEnumerator()
            {
                return this;
            }

            readonly IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private readonly bool FilterType()
            {
                if (typeFilter is null)
                    return true;

                return Current.GetType().IsType(typeFilter);
            }

            private readonly bool FilterAssetType()
            {
                if (assetTypeFilter is null)
                    return true;

                return Current.AssetType.IsType(assetTypeFilter);
            }
        }

        public struct DatabaseEnumerator : IEnumerator<object>, IEnumerable<object>
        {
            private readonly AssetDatabaseQuery query;

            private readonly Type? typeFilter;

            private readonly IAssetDatabase db;

            private readonly IEnumerator<Identifier> dbKeysEnumerator;

            private readonly IEnumerator<object> dbAssetsEnumerator;

            public object Current { get; private set; }

            public DatabaseEnumerator(AssetDatabaseQuery query, Type? typeFilter)
            {
                CC.Guard.IsNotNullTarget(query.Target);

                db = (IAssetDatabase)query.Target;
                dbKeysEnumerator = db.Keys.GetEnumerator();
                dbAssetsEnumerator = db.Values.GetEnumerator();

                Current = null!;

                this.query = query;
                this.typeFilter = typeFilter;
            }

            public bool MoveNext()
            {
                var loopFuse = LoopFuse.Create();

                while (dbKeysEnumerator.TryMoveNext(out var assetID)
                       &&
                       dbAssetsEnumerator.TryMoveNext(out var asset)
                       &&
                       loopFuse.DebugMoveNext())
                {
                    if (query.ID != null)
                    {
                        if (assetID != query.ID)
                            continue;
                    }

                    Current = asset;

                    if (!FilterType())
                        continue;

                    return true;
                }

                return false;
            }

            public readonly void Reset()
            {
                throw new NotSupportedException(nameof(Reset));
            }

            public readonly void Dispose()
            {
            }

            public readonly IEnumerator<object> GetEnumerator() => this;

            readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private readonly bool FilterType()
            {
                if (typeFilter is null)
                    return true;

                return Current.GetType().IsType(typeFilter);
            }
        }
    }
}
