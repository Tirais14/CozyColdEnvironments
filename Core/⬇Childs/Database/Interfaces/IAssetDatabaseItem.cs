using System;
using System.Threading.Tasks;

#nullable enable

namespace UTIRLib.DB
{
    public interface IAssetDatabaseItem
    {
        AddressableAssetInfo Info { get; }
        Type? AssetType { get; }
        bool IsAssetLoaded { get; }
        string AssetName { get; }
        string AssetGUID { get; }
        string Address { get; }

        void ReleaseAsset();
    }

    public interface IAssetDatabaseItem<T> : IAssetDatabaseItem
    {
        T? Asset { get; }

        Task<T> LoadAssetAsync();

        T LoadAsset();

        T? GetAsset();
    }
}