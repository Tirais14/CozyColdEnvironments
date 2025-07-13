using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;
using Object = UnityEngine.Object;

#nullable enable

namespace UTIRLib.Addressables
{
    public static class AddressableEditorUtility
    {
        /// <returns><see cref="string"/>.empty when failed</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string EntryLabelsToString(AddressableAssetEntry addressableAssetEntry)
        {
            if (addressableAssetEntry == null)
            {
                throw new ArgumentNullException(nameof(addressableAssetEntry));
            }

            StringBuilder labelStringBuilder = new();
            labelStringBuilder.AppendJoin(", ", addressableAssetEntry.labels);
            return labelStringBuilder.ToString();
        }

        /// <exception cref="WrongStringException"></exception>
        public static AddressableAssetGroup GetAddressableAssetGroup(string groupName,
            StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new StringArgumentException(nameof(groupName), groupName);
            }

            bool filter(AddressableAssetGroup group) =>
                group.Name.Contains(groupName, stringComparison);

            return AddressableAssetSettingsDefaultObject.Settings.groups.FirstOrDefault(filter);
        }

        public static Dictionary<string, AddressableAssetEntry[]> GatherAllAssetsFromAllGroups(
                                                                  AddressableAssetSettings settings,
                                                                  bool excludeFolder = true)
        {
            AddressableAssetGroup[] addressableAssetGroups = settings.groups.ToArray();

            AddressableAssetEntry[] gatheredEntries;
            Dictionary<string, List<AddressableAssetEntry>> addressablesByGroup = new();
            foreach (AddressableAssetGroup group in addressableAssetGroups)
            {
                if (group.entries.Count == 0)
                {
                    continue;
                }

                if (!addressablesByGroup.ContainsKey(group.Name))
                {
                    addressablesByGroup.Add(group.Name, new List<AddressableAssetEntry>());
                }

                gatheredEntries = GatherAllAssetsFromGroup(group, excludeFolder);
                if (gatheredEntries != null && gatheredEntries.Length > 0)
                {
                    addressablesByGroup[group.Name].AddRange(gatheredEntries);
                }
            }

            Dictionary<string, AddressableAssetEntry[]> result = new();
            foreach (var addressablesBuGroupItem in addressablesByGroup)
            {
                result.Add(addressablesBuGroupItem.Key, addressablesBuGroupItem.Value.ToArray());
            }

            return result;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static AddressableAssetEntry[] GatherAllAssetsFromGroup(
            AddressableAssetGroup addressableAssetGroup, bool excludeFolder = true)
        {
            if (addressableAssetGroup == null)
            {
                throw new ArgumentNullException(nameof(addressableAssetGroup));
            }
            if (addressableAssetGroup.entries.IsEmpty()) return Array.Empty<AddressableAssetEntry>();

            List<AddressableAssetEntry> foundEntries = new();

            addressableAssetGroup.GatherAllAssets(foundEntries,
                                                  includeSelf: true,
                                                  recurseAll: true,
                                                  includeSubObjects: true);

            if (excludeFolder)
            {
                return foundEntries.Count > 0 ?
                foundEntries.Where(entry => !entry.IsFolder).
                ToArray() : Array.Empty<AddressableAssetEntry>();
            }
            else
            {
                return foundEntries.Count > 0 ?
                foundEntries.ToArray() : Array.Empty<AddressableAssetEntry>();
            }
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static Dictionary<(string, AssetType), AddressableAssetEntry[]>
            SortAddressablesByGroupAndType(Dictionary<string, AddressableAssetEntry[]> addressablesByGroup)
        {
            if (addressablesByGroup.IsNull())
            {
                throw new ArgumentNullException(nameof(addressablesByGroup));
            }
            if (addressablesByGroup.IsEmpty()) return new Dictionary<(string, AssetType), AddressableAssetEntry[]>();

            Dictionary<(string groupName, AssetType assetType), AddressableAssetEntry[]> results = new();

            Dictionary<AssetType, AddressableAssetEntry[]> sortedAddressables;
            foreach (var addressablesByGroupPair in addressablesByGroup)
            {
                if (addressablesByGroupPair.Value.Length == 0) continue;

                sortedAddressables = SortAddressableEntries(addressablesByGroupPair.Value);

                foreach (var sortedAddressablesPair in sortedAddressables)
                {
                    results.Add((addressablesByGroupPair.Key, sortedAddressablesPair.Key), sortedAddressablesPair.Value);
                }
            }

            return results;
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="EmptyCollectionException"></exception>
        public static Dictionary<AssetType, AddressableAssetEntry[]> SortAddressableEntries
            (AddressableAssetEntry[] addressableAssetEntries)
        {
            if (addressableAssetEntries == null)
            {
                throw new ArgumentNullException(nameof(addressableAssetEntries));
            }
            if (addressableAssetEntries.IsNull())
            {
                throw new ArgumentNullException(nameof(addressableAssetEntries));
            }
            if (addressableAssetEntries.IsEmpty())
            {
                return new Dictionary<AssetType, AddressableAssetEntry[]>();
            }

            Dictionary<AssetType, List<AddressableAssetEntry>> addressablesDictionary = new(){
                { AssetType.GameObject, new List<AddressableAssetEntry>() },
                { AssetType.ScriptableObject, new List<AddressableAssetEntry>() },
                { AssetType.Scene, new List<AddressableAssetEntry>() },
                { AssetType.Generic, new List<AddressableAssetEntry>() },
            };

            Object targetObject;
            foreach (AddressableAssetEntry addressableAssetEntry in addressableAssetEntries)
            {
                targetObject = addressableAssetEntry.TargetAsset;
                if (targetObject is GameObject)
                {
                    addressablesDictionary[AssetType.GameObject].Add(addressableAssetEntry);
                }
                else if (targetObject is ScriptableObject)
                {
                    addressablesDictionary[AssetType.ScriptableObject].Add(addressableAssetEntry);
                }
                else if (targetObject is SceneAsset)
                {
                    addressablesDictionary[AssetType.Scene].Add(addressableAssetEntry);
                }
                else
                {
                    addressablesDictionary[AssetType.Generic].Add(addressableAssetEntry);
                }
            }

            Dictionary<AssetType, AddressableAssetEntry[]> sortedAddressables = new();
            foreach (var item in addressablesDictionary)
            {
                if (item.Value.Count == 0) continue;

                sortedAddressables.Add(item.Key,
                    item.Value.ToArray());
            }

            return sortedAddressables;
        }
    }
}