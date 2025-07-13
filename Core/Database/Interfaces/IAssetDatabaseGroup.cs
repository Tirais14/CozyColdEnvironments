#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;

namespace UTIRLib.DB
{
    public interface IAssetDatabaseGroup : IDatabaseGroup
    {
        new IAssetDatabase GetDatabase(object databaseKey);

        bool TryGetDatabase(object databaseKey, [NotNullWhen(true)] out IAssetDatabase? database);
    }

    public interface IAssetDatabaseGroup<TDatabase, TDatabaseItem, TAsset> : IAssetDatabaseGroup,
        IDatabaseGroup<string, TDatabase>
        where TDatabaseItem : IAssetDatabaseItem<TAsset>
        where TDatabase : class, IDatabase<string, TDatabaseItem>
    {
        TAsset? GetAsset(string databaseName, string assetName);

        TAsset? GetAsset(string databaseName, Type assetType, string assetName);

        T? GetAssetT<T>(string databaseName, string assetName);

        bool TryGetAsset(string databaseName, string assetName, [NotNullWhen(true)] out TAsset? asset);

        bool TryGetAsset(string databaseName, Type assetType, string assetName, [NotNullWhen(true)] out TAsset? asset);

        bool TryGetAssetT<T>(string databaseName, string assetName, [NotNullWhen(true)] out T? asset);

        TAsset? FindAsset(string assetNamePart, Type assetType);

        TAsset? FindAsset(string assetNamePart);

        TAsset? FindAsset(Type assetType);

        T? FindAssetT<T>(string assetNamePart);

        T? FindAssetT<T>();

        bool TryFindAsset(string assetNamePart, Type assetType, [NotNullWhen(true)] out TAsset? asset);

        bool TryFindAsset(string assetNamePart, [NotNullWhen(true)] out TAsset? asset);

        bool TryFindAsset(Type assetType, [NotNullWhen(true)] out TAsset? asset);

        bool TryFindAssetT<T>(string assetNamePart, [NotNullWhen(true)] out T? asset);

        bool TryFindAssetT<T>([NotNullWhen(true)] out T? asset);

        TAsset[] FindAssets(string assetNamePart, Type assetType);

        TAsset[] FindAssets(string assetNamePart);

        TAsset[] FindAssets(Type assetType);

        T[] FindAssetsT<T>(string assetNamePart);

        T[] FindAssetsT<T>();

        bool TryFindAssets(string assetNamePart, Type assetType, out TAsset[] assets);

        bool TryFindAssets(string assetNamePart, out TAsset[] assets);

        bool TryFindAssets(Type assetType, out TAsset[] assets);

        bool TryFindAssetsT<T>(string assetNamePart, out T[] assets);

        bool TryFindAssetsT<T>(out T[] assets);
    }
}