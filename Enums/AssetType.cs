using System;
using UnityEditor;
using UnityEngine;
using UTIRLib.Attributes.Metadata;
using Object = UnityEngine.Object;

#nullable enable

namespace UTIRLib
{
    [Flags]
    public enum AssetType
    {
        [MetaString("")]
        None,

        [MetaString("UnityObject")]
        Generic,

        [MetaString("Prefab")]
        GameObject = 2,

        ScriptableObject = 4,
        Scene = 8
    }

    public static class AssetTypeNameExtensions
    {
        public static Type? ToSystemType(this AssetType assetType) =>
            assetType switch {
                AssetType.Generic => typeof(Object),
                AssetType.GameObject => typeof(GameObject),
                AssetType.ScriptableObject => typeof(ScriptableObject),
                AssetType.Scene => typeof(SceneAsset),
                _ => null,
            };
    }
}