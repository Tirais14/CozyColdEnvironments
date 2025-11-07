using CCEnvs.Attributes.Metadata;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

#nullable enable

namespace CCEnvs.Unity
{
    public enum AssetType
    {
        None,
        Object,
        Prefab,

        [MetaString("Scriptable Object")]
        ScriptableObject,

        Texture2D,
        Sprite,
        SpriteAtlas,
        Scene,
    }

    public static class AssetTypeExtensions
    {
        public static Type? ToSystemType(this AssetType assetType) =>
            assetType switch
            {
                AssetType.Object => typeof(Object),
                AssetType.Prefab => typeof(GameObject),
                AssetType.ScriptableObject => typeof(ScriptableObject),
                AssetType.Scene => typeof(SceneAsset),
                AssetType.Texture2D => typeof(Texture2D),
                AssetType.Sprite => typeof(Sprite),
                AssetType.SpriteAtlas => typeof(SpriteAtlas),
                _ => throw new InvalidOperationException(assetType.ToString()),
            };
    }
}