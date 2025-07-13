using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEditor.AddressableAssets.Settings;

#nullable enable

namespace UTIRLib.DB
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class AddressableAssetInfo
    {
        [JsonRequired] private readonly string name;
        [JsonRequired] private readonly string guid;
        [JsonRequired] private readonly string address;
        private readonly string labels;
        [JsonRequired] private readonly string assetTypeName;
        [JsonIgnore] private readonly AssetType assetType;
        [JsonIgnore] public string Name => name;
        [JsonIgnore] public string AssetGUID => guid;
        [JsonIgnore] public string Address => address;
        [JsonIgnore] public string[] Labels => GetLabels();
        [JsonIgnore] public AssetType AssetType => assetType;

        /// <exception cref="ArgumentNullException"></exception>
        public AddressableAssetInfo(AddressableAssetEntry addressableAssetEntry)
        {
            UnityEngine.Object tagetAsset = addressableAssetEntry.TargetAsset;

            name = tagetAsset.name;
            guid = addressableAssetEntry.guid;
            address = addressableAssetEntry.address;
            labels = LabelsToString(addressableAssetEntry.labels);
            assetType = TirLibHelper.SystemTypeToAssetType(tagetAsset.GetType());

            assetTypeName = assetType.ToString();
        }

        [JsonConstructor]
        public AddressableAssetInfo(string name, string guid, string address,
            string labels, string assetTypeName)
        {
            this.name = name;
            this.guid = guid;
            this.address = address;
            this.labels = labels;
            this.assetTypeName = assetTypeName;
            assetType = Enum.Parse<AssetType>(assetTypeName);
        }

        public string[] GetLabels()
        {
            if (string.IsNullOrEmpty(labels))
            {
                return Array.Empty<string>();
            }

            return labels.Split(", ");
        }

        public override string ToString() => $"{name} ({assetType}).\n(labels: {labels})";

        //private IAssetDatabaseItem CreateAssetDatabaseItem(AssetType assetType) =>
        //    assetType switch {
        //        AssetType.GameObject => ConstructAssetDatabaseItem(typeof(AssetDatabaseItemGameObject)),
        //        AssetType.ScriptableObject => ConstructAssetDatabaseItem(typeof(AssetDatabaseItemScriptableObject)),
        //        AssetType.Scene => ConstructAssetDatabaseItem(typeof(AssetDatabaseItemScene)),
        //        _ => ConstructAssetDatabaseItem(typeof(AssetDatabaseItemGeneric)),
        //    };

        //private IAssetDatabaseItem ConstructAssetDatabaseItem(Type type)
        //{
        //    BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
        //    Type[] constructorArgTypes = new Type[] { typeof(AddressableAssetInfo) };
        //    var constructorArgs = new object[] { this };

        //    return type.GetConstructor(bindingFlags, binder: null, constructorArgTypes, Array.Empty<ParameterModifier>()).
        //        Invoke(constructorArgs) as IAssetDatabaseItem ?? throw new NullReferenceException("Error while castong.");
        //}

        private static string LabelsToString(HashSet<string> labels)
        {
            if (labels == null || labels.Count == 0) return string.Empty;

            string[] labelsArray = labels.ToArray();
            StringBuilder labelsStringBuilder = new();
            int labelsCount = labelsArray.Length;
            for (int i = 0; i < labelsCount; i++)
            {
                if ((i + 1) < labelsCount) { labelsStringBuilder.Append($"{labelsArray[i]}, "); }
                else { labelsStringBuilder.Append($"{labelsArray[i]}"); }
            }

            return labelsStringBuilder.ToString();
        }
    }
}