using UnityEngine;

#nullable enable

namespace UTIRLib.DB
{
    public class AssetDatabaseItemScriptableObject : AssetDatabaseItem<ScriptableObject>
    {
        public AssetDatabaseItemScriptableObject(AddressableAssetInfo addressableAssetInfo) :
            base(addressableAssetInfo)
        { }
    }
}