using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable

namespace UTIRLib
{
    public static partial class TirLibHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static AssetType SystemTypeToAssetType(Type assetObjectType)
        {
            if (assetObjectType == null)
            {
                throw new ArgumentNullException(nameof(assetObjectType));
            }

            if (typeof(GameObject).IsAssignableFrom(assetObjectType))
            {
                return AssetType.GameObject;
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(assetObjectType))
            {
                return AssetType.ScriptableObject;
            }
            else if (typeof(SceneAsset).IsAssignableFrom(assetObjectType))
            {
                return AssetType.Scene;
            }
            else if (typeof(Object).IsAssignableFrom(assetObjectType))
            {
                return AssetType.Generic;
            }
            else throw new InvalidOperationException();
        }
    }
}