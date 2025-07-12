#nullable enable

namespace UTIRLib.DB
{
    /// <summary>
    /// Contains information about generic asset
    /// </summary>
    public class AssetDatabaseItemGeneric : AssetDatabaseItem<UnityEngine.Object>
    {
        public AssetDatabaseItemGeneric(AddressableAssetInfo addressableAssetInfo) :
            base(addressableAssetInfo)
        { }
    }
}