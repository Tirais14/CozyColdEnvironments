using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UTIRLib.Collections;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;
using UTIRLib.Linq;
using static UnityEngine.AddressableAssets.Addressables;
using Object = UnityEngine.Object;

#nullable enable

namespace UTIRLib.DB
{
    public class AssetDatabase<TDatabaseItem, TAsset> : Database<string, TDatabaseItem>,
        IAssetDatabase<TDatabaseItem, TAsset>
        where TDatabaseItem : IAssetDatabaseItem<TAsset>
        where TAsset : Object
    {
        public new TAsset? this[string assetName] => GetAsset(assetName);

        public AssetDatabase() : base()
        { }

        public async Task<TAsset> LoadAssetAsync(string assetName) => await GetValue(assetName).LoadAssetAsync();

        public async Task<IList<TAsset>> LoadAssetsAsync(IList<IResourceLocation> locations,
            Action<TAsset>? callback = null, bool releaseDependenciesOnFailure = false)
        {
            IList<TAsset> loadedAssets =
                await Addressables.LoadAssetsAsync(locations, callback, releaseDependenciesOnFailure).Task;

            int loadedAssetsCount = loadedAssets.Count;
            for (int i = 0; i < loadedAssetsCount; i++)
            {
                ForceMarkAssetAsLoaded(loadedAssets[i]);
            }

            return loadedAssets ?? Array.Empty<TAsset>();
        }

        public async Task<IList<TAsset>> LoadAssetsAsync(string[] keys, MergeMode mergeMode = MergeMode.Union,
            Action<TAsset>? callback = null, bool releaseDependenciesOnFailure = false)
        {
            if (keys.IsNullOrEmpty())
            {
                throw new CollectionArgumentException(nameof(keys), keys);
            }
            if (keys.HasNullElement())
            {
                throw new ArgumentException(nameof(keys) + "array cannot not contain null.");
            }

            IList<IResourceLocation> resourceLocations = await LoadResourceLocationsAsync(keys, mergeMode).Task;

            return await LoadAssetsAsync(resourceLocations, callback, releaseDependenciesOnFailure);
        }

        public async Task<IList<TAsset>> LoadAllAssetsAsync() => await LoadAssetsAsync(GetAllAssetAddresses());

        public bool IsAssetLoaded(string assetName) => GetValue(assetName).IsAssetLoaded;

        public string GetAssetGUID(string assetName) => GetValue(assetName).AssetGUID;

        public string GetAssetAddress(string assetName) => GetValue(assetName).Address;

        /// <summary>
        /// Unload asset from memory
        /// </summary>
        public void ReleaseAsset(string assetName)
        {
            TDatabaseItem databaseItem = GetValue(assetName);
            databaseItem.ReleaseAsset();
        }

        /// <summary>
        /// Unload asset from memory
        /// </summary>
        public bool TryReleaseAsset(string assetName)
        {
            TDatabaseItem databaseItem = GetValue(assetName);
            if (databaseItem.IsAssetLoaded)
            {
                databaseItem.ReleaseAsset();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Unload all assets from memory
        /// </summary>
        public void ReleaseAllAssets()
        {
            foreach (TDatabaseItem databaseItem in db.Values)
            {
                databaseItem.ReleaseAsset();
            }
        }

        /// <summary>
        /// Unload all assets from memory
        /// </summary>
        public bool TryReleaseAllAssets(string assetName)
        {
            bool result = false;
            foreach (TDatabaseItem databaseItem in db.Values)
            {
                if (TryReleaseAsset(assetName))
                {
                    result = true;
                }
            }

            return result;
        }

        protected string[] GetAllAssetGUIDs()
        {
            var dbValues = db.Values;
            int guidsAddedCount = 0;
            string[] guids = new string[dbValues.Count];
            foreach (var value in dbValues)
            {
                guids[guidsAddedCount] = value.AssetGUID;
                guidsAddedCount++;
            }

            return guids;
        }

        protected string[] GetAllAssetAddresses()
        {
            var dbValues = db.Values;
            int addressedAddedCount = 0;
            string[] addresses = new string[dbValues.Count];
            foreach (var value in dbValues)
            {
                addresses[addressedAddedCount] = value.Address;
                addressedAddedCount++;
            }

            return addresses;
        }

        protected void ForceMarkAssetAsLoaded(TAsset asset)
        {
            if (asset == null)
            {
                throw new ArgumentNullException(nameof(asset));
            }
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            Type databaseItemType = typeof(TDatabaseItem);
            FieldInfo assetField = databaseItemType.GetField("asset", bindingFlags);
            FieldInfo assetTypeField = databaseItemType.GetField("assetType", bindingFlags);

            TDatabaseItem databaseItem = GetValue(asset.name);
            Type assetType = asset.GetType();
            assetTypeField.SetValue(databaseItem, assetType);
            assetField.SetValue(databaseItem, asset);
        }

        public async Task<TAsset> GetOrLoadAssetAsync(string assetName)
        {
            TDatabaseItem databaseItem = GetValue(assetName);
            if (!databaseItem.IsAssetLoaded)
            {
                return await databaseItem.LoadAssetAsync();
            }
            else return databaseItem.Asset!;
        }

        public TAsset? GetAsset(string assetName) => GetValue(assetName).Asset;

        public TAsset? GetAsset(Type assetType, string assetName)
        {
            TAsset? asset = GetAsset(assetName);
            if (assetType.IsInstanceOfType(asset))
            {
                return asset;
            }
            else return default;
        }

        public T? GetAsset<T>(string assetName)
        {
            TAsset? asset = GetAsset(assetName);
            if (asset is T typedAsset)
            {
                return typedAsset;
            }
            else return default;
        }

        public bool TryGetAsset(string assetName, [NotNullWhen(true)] out TAsset? asset)
        {
            asset = GetAsset(assetName);

            return asset != null;
        }

        public bool TryGetAsset(Type assetType, string assetName, [NotNullWhen(true)] out TAsset? asset) =>
            TryFindAsset(assetName, assetType, out asset);

        public bool TryGetAssetT<T>(string assetName, [NotNullWhen(true)] out T? asset) =>
            TryFindAssetT(assetName, out asset);

        public TAsset? FindAsset(string assetNamePart)
        {
            TDatabaseItem? databaseItem = FindValueByKey((assetName) => NameFilter(assetName, assetNamePart));

            return databaseItem.IsNotDefault() ? databaseItem.Asset : default;
        }

        public TAsset? FindAsset(Type assetType)
        {
            TDatabaseItem? databaseItem = FindValue((databaseItem) => TypeFilter(databaseItem.Asset, assetType));

            return databaseItem.IsNotDefault() ? databaseItem.Asset : default;
        }

        public TAsset? FindAsset(string assetNamePart, Type assetType)
        {
            TDatabaseItem? databaseItem = FindValue((assetName) => NameFilter(assetName, assetNamePart),
                (databaseItem) => TypeFilter(databaseItem.Asset, assetType));

            return databaseItem.IsNotDefault() ? databaseItem.Asset : default;
        }

        public T? FindAssetT<T>() => FindAsset(typeof(T)).IsQ<T>();

        public T? FindAssetT<T>(string assetNamePart) => FindAsset(assetNamePart, typeof(T)).IsQ<T>();

        public bool TryFindAssetT<T>(string assetNamePart, [NotNullWhen(true)] out T? asset)
        {
            asset = FindAssetT<T>(assetNamePart);

            return asset.IsNotDefault();
        }

        public bool TryFindAssetT<T>([NotNullWhen(true)] out T? asset)
        {
            asset = FindAssetT<T>();

            return asset.IsNotDefault();
        }

        public bool TryFindAsset(string assetNamePart, [NotNullWhen(true)] out TAsset? asset)
        {
            asset = FindAsset(assetNamePart);

            return asset != null;
        }

        public bool TryFindAsset(Type assetType, [NotNullWhen(true)] out TAsset? asset)
        {
            asset = FindAsset(assetType);

            return asset != null;
        }

        public bool TryFindAsset(string assetNamePart, Type assetType, [NotNullWhen(true)] out TAsset? asset)
        {
            asset = FindAsset(assetNamePart, assetType);

            return asset != null;
        }

        public TAsset[] FindAssets(string assetNamePart) => FindAssetsInternal(assetNamePart, assetType: null);

        public TAsset[] FindAssets(Type assetType) => FindAssetsInternal(assetNamePart: null, assetType);

        public TAsset[] FindAssets(string assetNamePart, Type assetType) => FindAssetsInternal(assetNamePart, assetType);

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

        public T[] FindAssetsT<T>() => FindAssets(typeof(T)).Cast<T>().ToArray();

        public T[] FindAssetsT<T>(string assetNamePart) => FindAssets(assetNamePart).Cast<T>().ToArray();

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

        protected static bool NameFilter(string assetName, string assetNamePart) =>
            assetName.Contains(assetNamePart);

        private TAsset[] FindAssetsInternal(string? assetNamePart, Type? assetType)
        {
            Predicate<string>? nameFilter = null;
            Predicate<TDatabaseItem>? typeFilter = null;
            bool isNameFilterSetted = false;
            bool isTypeFilterSetted = false;
            if (assetNamePart != null)
            {
                nameFilter = (assetName) => NameFilter(assetName, assetNamePart);
                isNameFilterSetted = true;
            }
            if (assetType != null)
            {
                typeFilter = (databaseItem) => TypeFilter(databaseItem, assetType);
                isTypeFilterSetted = true;
            }

            TDatabaseItem[] databaseItems;
            if (isNameFilterSetted && isTypeFilterSetted)
            {
                databaseItems = FindValues(nameFilter!, typeFilter!);
            }
            else if (isNameFilterSetted)
            {
                databaseItems = FindValuesByKey(nameFilter!);
            }
            else
            {
                databaseItems = FindValues(typeFilter!);
            }

            List<TAsset> assets = new();
            for (int i = 0; i < databaseItems.Length; i++)
            {
                if (databaseItems[i].IsAssetLoaded)
                {
                    assets.Add(databaseItems[i].Asset!);
                }
            }

            return assets.ToArray();
        }

        Task IAssetDatabase.LoadAllAssetsAsync() => LoadAllAssetsAsync();

        object? IAssetDatabase.GetAsset(string assetName) => GetAsset(assetName);

        object? IAssetDatabase.GetAsset(Type assetType, string assetName) => GetAsset(assetType, assetName);
    }
}