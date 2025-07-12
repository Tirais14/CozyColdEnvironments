using System;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UTIRLib.Diagnostics;
using UTIRLib.Editor;
using UTIRLib.Extensions;
using UTIRLib.FileSystem;
using UTIRLib.Utils;

#nullable enable

namespace UTIRLib.TwoD
{
    public abstract class TileAssetCreationWindow : TirLibEditorWindow
    {
        private ObjectField textureObjectField = null!;
        private ObjectField bindedGameObjectField = null!;
        private Button createButton = null!;

        protected abstract Enum DefaultTileTypeName { get; }
        protected abstract string DefaultOpenFolderPanelDirectory { get; }

        protected override void ConstructUIElements()
        {
            base.ConstructUIElements();

            textureObjectField = new ObjectField("Texture") {
                objectType = typeof(Texture2D),
                allowSceneObjects = false
            };

            bindedGameObjectField = new ObjectField("Game Object") {
                objectType = typeof(GameObject),
                allowSceneObjects = false
            };

            createButton = new Button(CreateTiles) {
                text = "Create"
            };
        }

        protected abstract Type GetTileType();

        private void CreateTiles()
        {
            string targetDirectory = EditorUtility.OpenFolderPanel(
                "Select target directory",
                DefaultOpenFolderPanelDirectory,
                string.Empty
            );

            if (targetDirectory.IsNullOrEmpty() || !Directory.Exists(targetDirectory))
            {
                Debug.Log("Cancelled.");
                return;
            }
            if (textureObjectField.value == null)
            {
                Debug.LogWarning("Texture not specified.");
                return;
            }

            TileBase[] tiles;
            try
            {
                tiles = TileAssetsCreator.CreateFromAtlas(
                    GetTileType(),
                    AssetDatabase.GetAssetPath(textureObjectField.value),
                    (GameObject?)bindedGameObjectField.value
                );
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return;
            }

            int tilesCount = tiles.Length;
            for (int i = 0; i < tilesCount; i++)
            {
                if (tiles[i].name.IsNullOrEmpty())
                {
                    throw new WrongStringException(tiles[i].name);
                }

                FSPath assetPath = new(targetDirectory, tiles[i].name + ".asset");

                AssetDatabase.CreateAsset(tiles[i], assetPath);
            }
        }
    }
}