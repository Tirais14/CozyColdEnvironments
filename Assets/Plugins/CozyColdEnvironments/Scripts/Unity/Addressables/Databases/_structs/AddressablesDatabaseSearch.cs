using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using ZLinq;
using Object = UnityEngine.Object;

#nullable enable
#pragma warning disable S1117
#pragma warning disable S3236
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public record AddressablesDatabaseSearch
    {
        public Maybe<IAddressablesDatabase> database { get; set; }
        public Maybe<IAddressablesDatabaseRegistry> registry { get; set; }
        public Maybe<string> textIdFilter { get; set; }
        public Maybe<int> numberIdFilter { get; set; }

        protected static bool FilterType(Type left, Type? right)
        {
            return right is null || left.IsType(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AddressablesDatabaseSearch From(IAddressablesDatabase db)
        {
            CC.Guard.IsNotNull(db, nameof(db));

            database = db.Maybe();

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AddressablesDatabaseSearch From(IAddressablesDatabaseRegistry reg)
        {
            CC.Guard.IsNotNull(reg, nameof(reg));

            registry = reg.Maybe();

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AddressablesDatabaseSearch ByTextID(string? name = null)
        {
            textIdFilter = name;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AddressablesDatabaseSearch ByNumberID(int id = int.MinValue)
        {
            if (id == int.MinValue)
            {
                numberIdFilter = Maybe<int>.None;
                return this;
            }

            numberIdFilter = id;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AddressablesDatabaseSearch ByDatabase(Type type, Type? assetType = null)
        {
            database = Database(type, assetType);
            ResetFilters();

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<IAddressablesDatabase> Databases(Type type, Type? assetType = null)
        {
            Guard.IsNotNull(type);

            var reg = registry.GetValueUnsafe();

            return from db in reg.Values
                   where FilterType(db.GetType(), type)
                   where FilterText(db.ID.Text)
                   where FilterNumber(db.ID.Number)
                   select db;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<IAddressablesDatabase> Database(Type type, Type? assetType = null)
        {
            var reg = registry.GetValueUnsafe();

            var id = new Identifier()
            {
                Number = numberIdFilter,
                Text = textIdFilter
            };

            if (reg.TryGetValue(id, out IAddressablesDatabase db)
                &&
                FilterType(db.GetType(), type)
                &&
                FilterType(db.AssetType, assetType))
            {
                return (db, null);
            }

            var dbs = Databases(type, assetType).ToArray();

            return (dbs.SingleOrDefault(), new DatabaseNotFoundException(id, reg, assetType));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<T> Database<T>(Type? assetType = null)
            where T : IAddressablesDatabase
        {
            return Database(typeof(T), assetType).Cast<T>();
        }

        public IEnumerable<Object> Assets(Type? type = null)
        {
            IAddressablesDatabase db = database.GetValueUnsafe();

            return from item in db.Cast<KeyValuePair<Identifier, Object>>()
                   where FilterType(item.Value.GetType(), type)
                   where FilterText(item.Key.Text)
                   where FilterNumber(item.Key.Number)
                   select item.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<Object> Asset(Type? type = null)
        {
            var db = database.GetValueUnsafe();

            var id = new Identifier()
            {
                Number = numberIdFilter,
                Text = textIdFilter
            };

            return db[id].Lax().Match(
                some: asset => (asset, null!),
                none: () => (Assets(type).SingleOrDefault(), new AssetNotFoundException(db, id, type)))
                .GetValueUnsafe();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AddressablesDatabaseSearch ResetFilters()
        {
            textIdFilter = null;
            numberIdFilter = Maybe<int>.None;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AddressablesDatabaseSearch Reset()
        {
            database = null;
            registry = null;
            ResetFilters();

            return this;
        }

        protected bool FilterText(Maybe<string> text)
        {
            return textIdFilter.Map(filter =>
                        text.Map(text =>
                            text.ContainsOrdinal(filter)).GetValue(true)
                            ).Raw;
        }

        protected bool FilterNumber(Maybe<int> number)
        {
            return numberIdFilter.Map(filter =>
                        number.Map(number =>
                            number == filter).GetValue(true)
                            ).Raw;
        }
    }
}
