using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;
using UTIRLib.Initializer;

#nullable enable

namespace UTIRLib.TwoD
{
    public static class TileAssetsCreator
    {
        public static TileBase[] CreateFromAtlas(Type tileType, string textureAtlasPath,
            GameObject? bindedGameObject = null)
        {
            if (textureAtlasPath.IsNullOrEmpty())
            {
                throw new StringArgumentException(nameof(textureAtlasPath), textureAtlasPath);
            }

            Sprite[] sprites = LoadSpritesFromAtlas(textureAtlasPath);
            var tiles = new TileBase[sprites.Length];
            int spritesCount = sprites.Length;
            for (int i = 0; i < spritesCount; i++)
            {
                tiles[i] = (TileBase)ScriptableObject.CreateInstance(tileType);
                tiles[i].name = sprites[i].name;
                if (tiles[i] is Tile typedTile)
                {
                    typedTile.sprite = sprites[i];
                    if (bindedGameObject != null)
                    {
                        typedTile.gameObject = bindedGameObject;
                    }
                }
            }

            return tiles;
        }

        /// <exception cref="NullOrEmptyStringException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static TTile[] CreateFromAtlas<TTile>(string textureAtlasPath)
            where TTile : TileBase, IInitializable<Sprite> => CreateFromAtlas(typeof(TTile), textureAtlasPath) as TTile[] ??
            Array.Empty<TTile>();

        /// <exception cref="Exception"></exception>
        private static Sprite[] LoadSpritesFromAtlas(string textureAtlasPath)
        {
            textureAtlasPath = textureAtlasPath.Replace('\\', '/');
            Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(textureAtlasPath).
                OfType<Sprite>().
                ToArray();
            if (sprites.IsNullOrEmpty())
            {
                throw new Exception($"Specified texture atlas is not correct or not exist. Path: {textureAtlasPath}");
            }

            return sprites;
        }
    }
}