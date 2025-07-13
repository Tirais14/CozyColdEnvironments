using UnityEngine;

#nullable enable

namespace UTIRLib.DB
{
    /// <summary>
    /// Contains game object (prefab) asset reference and other asset loading tools and information
    /// </summary>
    public class AssetDatabaseGameObject : AssetDatabase<AssetDatabaseItemGameObject, GameObject>
    {
        public AssetDatabaseGameObject() : base()
        { }
    }
}