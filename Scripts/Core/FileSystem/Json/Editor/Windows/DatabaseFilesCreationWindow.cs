using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.UIElements;
using UTIRLib.Addressables;
using UTIRLib.Editor;

#nullable enable

namespace UTIRLib.FileSystem.Json.Editor
{
    public abstract class DatabaseFilesCreationWindow : TirLibEditorWindow
    {
        protected Toggle overwriteToggle = null!;
        protected Button createButton = null!;

        protected virtual string BuildName(string groupName, AssetType assetType)
        {
            return $"{groupName}_{assetType}_Database";
        }

        protected void CreateDatabaseFiles(FSPath saveDirectory, AddressableAssetGroup group)
        {
            AddressableAssetEntry[] allGroupAssets = AddressableEditorUtility.GatherAllAssetsFromGroup(group);

            Dictionary<AssetType, AddressableAssetEntry[]> sortedGroupAssets =
                AddressableEditorUtility.SortAddressableEntries(allGroupAssets);

            foreach (var sortedAssetsItem in sortedGroupAssets)
            {
                JsonFileAddressableDatabaseCreator.CreateDatabaseFile(
                    sortedAssetsItem.Value,
                    saveDirectory,
                    BuildName(group.name, sortedAssetsItem.Key),
                    overwriteToggle.value);
            }
        }

        protected virtual FSPath SelectSaveDirectory() => SelectDirectory();

        protected virtual void OnCreateClick()
        {
            FSPath savePath = SelectSaveDirectory();

            if (!savePath.HasValue)
            {
                Abort("Directory not selected.");
                return;
            }

            foreach (var group in GetAddressableSettings().groups)
            {
                CreateDatabaseFiles(savePath, group);
            }

            AssetDatabase.Refresh();
        }

        protected virtual AddressableAssetSettings GetAddressableSettings()
        {
            return AddressableAssetSettingsDefaultObject.Settings;
        }

        protected override void ConstructUIElements()
        {
            base.ConstructUIElements();

            overwriteToggle ??= new Toggle("Overwrite Files") {
                value = true
            };

            createButton ??= new Button(OnCreateClick) {
                text = "Create Database Files"
            };
        }
    }
}