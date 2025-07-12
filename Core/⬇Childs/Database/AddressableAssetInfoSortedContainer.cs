using System;
using System.Collections.Generic;

#nullable enable

namespace UTIRLib.DB
{
    using UnityEngine;

    public class AddressableAssetInfoSortedContainer
    {
        private readonly AddressableAssetInfo[] rawInfo;

        private AddressableAssetInfo[] genericInfo = null!;
        private AddressableAssetInfo[] gameObjectInfo = null!;
        private AddressableAssetInfo[] scriptableObjectInfo = null!;
        private AddressableAssetInfo[] scenesInfo = null!;
        public AddressableAssetInfo[] GenericAddressables => genericInfo;
        public AddressableAssetInfo[] GameObjectAddressables => gameObjectInfo;
        public AddressableAssetInfo[] ScriptableObjectAddressables => scriptableObjectInfo;
        public AddressableAssetInfo[] SceneAddressables => scenesInfo;
        public bool IsSorted { get; private set; }

        /// <exception cref="ArgumentNullException"></exception>
        public AddressableAssetInfoSortedContainer(AddressableAssetInfo[] rawInfo)
        {
            this.rawInfo = rawInfo ??
                throw new ArgumentNullException(nameof(rawInfo));

            Sort();
        }

        public void Sort()
        {
            if (IsSorted)
            {
                Debug.LogError("Already sorted.");
                return;
            }

            List<AddressableAssetInfo> genericInfoList = new();
            List<AddressableAssetInfo> gameObjectInfoList = new();
            List<AddressableAssetInfo> scriptableObjectInfoList = new();
            List<AddressableAssetInfo> scenesInfoList = new();
            int rawInfoCount = rawInfo.Length;
            for (int i = 0; i < rawInfoCount; i++)
            {
                if (rawInfo[i].AssetType == AssetType.Generic) { genericInfoList.Add(rawInfo[i]); }
                else if (rawInfo[i].AssetType == AssetType.GameObject) { gameObjectInfoList.Add(rawInfo[i]); }
                else if (rawInfo[i].AssetType == AssetType.ScriptableObject) { scriptableObjectInfoList.Add(rawInfo[i]); }
                else if (rawInfo[i].AssetType == AssetType.Scene) { scenesInfoList.Add(rawInfo[i]); }
            }

            genericInfo = genericInfoList.Count > 0 ?
                genericInfoList.ToArray() : Array.Empty<AddressableAssetInfo>();

            gameObjectInfo = gameObjectInfoList.Count > 0 ?
                gameObjectInfoList.ToArray() : Array.Empty<AddressableAssetInfo>();

            scriptableObjectInfo = scriptableObjectInfoList.Count > 0 ?
                scriptableObjectInfoList.ToArray() : Array.Empty<AddressableAssetInfo>();

            scenesInfo = scenesInfoList.Count > 0 ?
                scenesInfoList.ToArray() : Array.Empty<AddressableAssetInfo>();

            IsSorted = true;
        }
    }
}