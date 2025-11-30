using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using ZLinq;

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
        public StringMatchSettings TextMatchSettings { get; set; }

        public AddressablesDatabaseSearch() => Reset();

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AddressablesDatabaseSearch ByPartialName(bool state = true)
        {
            if (state)
                TextMatchSettings |= StringMatchSettings.Partial;
            else
                TextMatchSettings &= ~StringMatchSettings.Partial;

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
        public AddressablesDatabaseSearch ByNumberID(Maybe<int> number = default)
        {
            numberIdFilter = number;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AddressablesDatabaseSearch ByEnumID<T>(T input)
            where T : unmanaged, Enum
        {
            var id = Identifier.Create(input);

            ByNumberID(id.Number);
            ByTextID(id.Text.Raw);

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AddressablesDatabaseSearch InDatabase(Type? type = null, Type? assetType = null)
        {
            database = Database(type, assetType).Strict().Maybe();
            ResetFilters();

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<IAddressablesDatabase> Databases(Type? type = null, Type? assetType = null)
        {
            CC.Guard.IsNotNull(registry.Raw, nameof(registry));

            var reg = registry.GetValueUnsafe();

            return from item in reg
                   where FilterType(item.Value.GetType(), type)
                   where FilterType(item.Value.AssetType, assetType)
                   where FilterText(item.Key.Text)
                   where FilterNumber(item.Key.Number)
                   select item.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<IAddressablesDatabase> Database(Type? type = null, Type? assetType = null)
        {
            CC.Guard.IsNotNull(registry.Raw, nameof(registry));

            var reg = registry.GetValueUnsafe();

            var id = new Identifier()
            {
                Number = numberIdFilter,
                Text = textIdFilter
            };

            if (reg[id].Lax().TryGetValue(out IAddressablesDatabase? db)
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

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<T> Database<T>(Type? assetType = null)
            where T : IAddressablesDatabase
        {
            return Database(typeof(T), assetType).Cast<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<object> Assets(Type? type = null)
        {
            CC.Guard.IsNotNull(database.Raw, nameof(database));

            IAddressablesDatabase db = database.GetValueUnsafe();

            return (from item in db.Keys.ZLinq().Zip(db.Values, (key, value) => (key, value))
                   where FilterType(item.value.GetType(), type)
                   where FilterText(item.key.Text)
                   where FilterNumber(item.key.Number)
                   select item.value)
                   .AsEnumerable();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Assets<T>()
        {
            return Assets(typeof(T)).Cast<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<object> Asset(Type? type = null)
        {
            CC.Guard.IsNotNull(database.Raw, nameof(database));

            var db = database.GetValueUnsafe();

            var id = new Identifier()
            {
                Number = numberIdFilter,
                Text = textIdFilter
            };

            if (db[id].Lax().TryGetValue(out object? asset))
                return (asset, null!);

            return (Assets(type).FirstOrDefault(), new AssetNotFoundException(db, id, type));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<T> Asset<T>()
        {
            return Asset(typeof(T)).Cast<T>();
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
            TextMatchSettings = StringMatchSettings.Default;

            return this;
        }

        protected bool FilterText(Maybe<string> other)
        {
            if (textIdFilter.IsNone)
                return true;

            return other.Match(textIdFilter);
        }

        protected bool FilterNumber(Maybe<int> other)
        {
            return numberIdFilter.Map(filter =>
                        other.Map(number =>
                            number == filter).GetValue(true)
                            ).GetValue(true);
        }
    }
}
