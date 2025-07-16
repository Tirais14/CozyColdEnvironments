using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UTIRLib.Extensions;
using Object = UnityEngine.Object;

#nullable enable

namespace UTIRLib.DB
{
    public class AssetDatabaseGroup<TDatabase, TDatabaseItem, TAsset> : DatabaseGroup<string, TDatabase>,
        IAssetDatabaseGroup<TDatabase, TDatabaseItem, TAsset>
        where TDatabase : class, IAssetDatabase<TDatabaseItem, TAsset>
        where TDatabaseItem : IAssetDatabaseItem<TAsset>
        where TAsset : Object
    {
        public AssetDatabaseGroup()
        { }

        public AssetDatabaseGroup(string databaseKey, TDatabase database) => AddDatabase(databaseKey, database);

        public AssetDatabaseGroup(IValuePair<string, TDatabase>[] keyAndDatabases) => AddDatabaseRange(keyAndDatabases);

        public bool TryGetDatabase(object databaseKey, [NotNullWhen(true)] out IAssetDatabase? database)
        {
            if (TryGetDatabase((string)databaseKey, out TDatabase databaseTyped))
            {
                database = databaseTyped;
                return true;
            }

            database = null;
            return false;
        }

        public TAsset? GetAsset(string databaseName, string assetName) => GetDatabase(databaseName).GetAsset(assetName);

        public TAsset? GetAsset(string databaseName, Type assetType, string assetName) =>
            GetDatabase(databaseName).GetAsset(assetType, assetName);

        public T? GetAssetT<T>(string databaseName, string assetName)
            => GetDatabase(databaseName).GetAsset<T>(assetName);

        public bool TryGetAsset(string databaseName, string assetName, [NotNullWhen(true)] out TAsset? asset)
        {
            if (TryGetDatabase(databaseName, out TDatabase database) && database.TryGetAsset(assetName, out asset))
            {
                return true;
            }

            asset = null;
            return false;
        }

        public bool TryGetAsset(string databaseName, Type assetType, string assetName,
            [NotNullWhen(true)] out TAsset? asset)
        {
            if (TryGetDatabase(databaseName, out TDatabase database) &&
                database.TryGetAsset(assetType, assetName, out asset))
            {
                return true;
            }

            asset = null;
            return false;
        }

        public bool TryGetAssetT<T>(string databaseName, string assetName, [NotNullWhen(true)] out T? asset)
        {
            if (TryGetDatabase(databaseName, out TDatabase database) &&
                database.TryGetAssetT(assetName, out asset))
            {
                return true;
            }

            asset = default;
            return false;
        }

        public TAsset? FindAsset(string assetNamePart, Type assetType)
        {
            foreach (TDatabase db in dbs.Values)
            {
                if (db.TryFindAsset(assetNamePart, assetType, out TAsset? asset))
                {
                    return asset;
                }
            }

            return null;
        }

        public TAsset? FindAsset(string assetNamePart)
        {
            foreach (TDatabase db in dbs.Values)
            {
                if (db.TryFindAsset(assetNamePart, out TAsset? asset))
                {
                    return asset;
                }
            }

            return null;
        }

        public TAsset? FindAsset(Type assetType)
        {
            foreach (TDatabase db in dbs.Values)
            {
                if (db.TryFindAsset(assetType, out TAsset? asset))
                {
                    return asset;
                }
            }

            return null;
        }

        public T? FindAssetT<T>(string assetNamePart)
        {
            foreach (TDatabase db in dbs.Values)
            {
                if (db.TryFindAssetT(assetNamePart, out T? asset))
                {
                    return asset;
                }
            }

            return default;
        }

        public T? FindAssetT<T>()
        {
            foreach (TDatabase db in dbs.Values)
            {
                if (db.TryFindAssetT(out T? asset))
                {
                    return asset;
                }
            }

            return default;
        }

        public bool TryFindAsset(string assetNamePart, Type assetType, [NotNullWhen(true)] out TAsset? asset)
        {
            foreach (TDatabase db in dbs.Values)
            {
                if (db.TryFindAsset(assetNamePart, assetType, out asset))
                {
                    return true;
                }
            }

            asset = null;
            return false;
        }

        public bool TryFindAsset(string assetNamePart, [NotNullWhen(true)] out TAsset? asset)
        {
            foreach (TDatabase db in dbs.Values)
            {
                if (db.TryFindAsset(assetNamePart, out asset))
                {
                    return true;
                }
            }

            asset = null;
            return false;
        }

        public bool TryFindAsset(Type assetType, [NotNullWhen(true)] out TAsset? asset)
        {
            foreach (TDatabase db in dbs.Values)
            {
                if (db.TryFindAsset(assetType, out asset))
                {
                    return true;
                }
            }

            asset = null;
            return false;
        }

        public bool TryFindAssetT<T>(string assetNamePart, [NotNullWhen(true)] out T? asset)
        {
            foreach (TDatabase db in dbs.Values)
            {
                if (db.TryFindAssetT(assetNamePart, out asset))
                {
                    return true;
                }
            }

            asset = default;
            return false;
        }

        public bool TryFindAssetT<T>([NotNullWhen(true)] out T? asset)
        {
            foreach (TDatabase db in dbs.Values)
            {
                if (db.TryFindAssetT(out asset))
                {
                    return true;
                }
            }

            asset = default;
            return false;
        }

        public TAsset[] FindAssets(string assetNamePart)
        {
            List<TAsset> assets = new();
            foreach (TDatabase db in dbs.Values)
            {
                assets.AddRange(db.FindAssets(assetNamePart));
            }

            return assets.ToArray();
        }

        public TAsset[] FindAssets(Type assetType)
        {
            List<TAsset> assets = new();
            foreach (TDatabase db in dbs.Values)
            {
                assets.AddRange(db.FindAssets(assetType));
            }

            return assets.ToArray();
        }

        public TAsset[] FindAssets(string assetNamePart, Type assetType)
        {
            List<TAsset> assets = new();
            foreach (TDatabase db in dbs.Values)
            {
                assets.AddRange(db.FindAssets(assetNamePart, assetType));
            }

            return assets.ToArray();
        }

        public T[] FindAssetsT<T>(string assetNamePart)
        {
            List<T> assets = new();
            foreach (TDatabase db in dbs.Values)
            {
                assets.AddRange(db.FindAssetsT<T>(assetNamePart));
            }

            return assets.ToArray();
        }

        public T[] FindAssetsT<T>()
        {
            List<T> assets = new();
            foreach (TDatabase db in dbs.Values)
            {
                assets.AddRange(db.FindAssetsT<T>());
            }

            return assets.ToArray();
        }

        public bool TryFindAssets(string assetNamePart, out TAsset[] assets)
        {
            assets = FindAssets(assetNamePart);

            return assets.IsNotEmpty();
        }

        public bool TryFindAssets(Type assetType, out TAsset[] assets)
        {
            assets = FindAssets(assetType);

            return assets.IsNotEmpty();
        }

        public bool TryFindAssets(string assetNamePart, Type assetType, out TAsset[] assets)
        {
            assets = FindAssets(assetNamePart, assetType);

            return assets.IsNotEmpty();
        }

        public bool TryFindAssetsT<T>(string assetNamePart, out T[] assets)
        {
            assets = FindAssetsT<T>(assetNamePart);

            return assets.IsNotEmpty();
        }

        public bool TryFindAssetsT<T>(out T[] assets)
        {
            assets = FindAssetsT<T>();

            return assets.IsNotEmpty();
        }

        IAssetDatabase IAssetDatabaseGroup.GetDatabase(object databaseKey) => GetDatabase((string)databaseKey);
    }
}