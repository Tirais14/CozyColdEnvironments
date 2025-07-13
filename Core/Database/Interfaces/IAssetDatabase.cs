using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceLocations;
using static UnityEngine.AddressableAssets.Addressables;

#nullable enable

namespace UTIRLib.DB
{
    public interface IAssetDatabase : IDatabase
    {
        object? GetAsset(string assetName);

        object? GetAsset(Type assetType, string assetName);

        T? GetAsset<T>(string assetName);

        bool IsAssetLoaded(string assetName);

        string GetAssetGUID(string assetName);

        string GetAssetAddress(string assetName);

        void ReleaseAsset(string assetName);

        void ReleaseAllAssets();

        bool TryReleaseAsset(string assetName);

        bool TryReleaseAllAssets(string assetName);

        Task LoadAllAssetsAsync();
    }

    public interface IAssetDatabase<TDatabaseItem, TAsset> : IAssetDatabase, IDatabase<string, TDatabaseItem>
        where TDatabaseItem : IAssetDatabaseItem<TAsset>
    {
        new TAsset? this[string assetName] { get; }

        Task<TAsset> LoadAssetAsync(string assetName);

        Task<IList<TAsset>> LoadAssetsAsync(IList<IResourceLocation> locations, Action<TAsset>? callback = null,
            bool releaseDependenciesOnFailure = false);

        Task<IList<TAsset>> LoadAssetsAsync(string[] keys, MergeMode mergeMode = MergeMode.Union,
            Action<TAsset>? callback = null, bool releaseDependenciesOnFailure = false);

        new Task<IList<TAsset>> LoadAllAssetsAsync();

        new TAsset? GetAsset(string assetName);

        new TAsset? GetAsset(Type assetType, string assetName);

        bool TryGetAsset(string assetName, [NotNullWhen(true)] out TAsset? asset);

        bool TryGetAsset(Type assetType, string assetName, [NotNullWhen(true)] out TAsset? asset);

        bool TryGetAssetT<T>(string assetName, [NotNullWhen(true)] out T? asset);

        Task<TAsset> GetOrLoadAssetAsync(string assetName);

        TAsset? FindAsset(string assetNamePart);

        TAsset? FindAsset(Type assetType);

        TAsset? FindAsset(string assetNamePart, Type assetType);

        T? FindAssetT<T>();

        T? FindAssetT<T>(string assetNamePart);

        bool TryFindAsset(string assetNamePart, [NotNullWhen(true)] out TAsset? asset);

        bool TryFindAsset(Type assetType, [NotNullWhen(true)] out TAsset? asset);

        bool TryFindAsset(string assetNamePart, Type assetType, [NotNullWhen(true)] out TAsset? asset);

        bool TryFindAssetT<T>(string assetNamePart, [NotNullWhen(true)] out T? asset);

        bool TryFindAssetT<T>([NotNullWhen(true)] out T? asset);

        TAsset[] FindAssets(string assetNamePart);

        TAsset[] FindAssets(Type assetType);

        TAsset[] FindAssets(string assetNamePart, Type assetType);

        bool TryFindAssets(string assetNamePart, out TAsset[] assets);

        bool TryFindAssets(Type assetType, out TAsset[] assets);

        bool TryFindAssets(string assetNamePart, Type assetType, out TAsset[] assets);

        T[] FindAssetsT<T>();

        T[] FindAssetsT<T>(string assetNamePart);

        bool TryFindAssetsT<T>(string assetNamePart, out T[] assets);

        bool TryFindAssetsT<T>(out T[] assets);
    }
}