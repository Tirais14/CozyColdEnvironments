using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Language;
using Cysharp.Threading.Tasks;
using SuperLinq;
using System;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public class SpriteAddressablesDatabase : AddressablesDatabase<Sprite>
    {
        /// <summary>
        /// Loads <see cref="Texture2D"/> and converts to the <see cref="Sprite"/>
        /// </summary>
        public bool TexturesAsSprites { get; set; }

        public SpriteAddressablesDatabase()
        {
        }

        public SpriteAddressablesDatabase(int capacity)
            :
            base(capacity)
        {
        }

        public SpriteAddressablesDatabase(IEnumerable<KeyValuePair<AssetKey, Sprite>> values)
            :
            base(values)
        {
        }

        private static Sprite[] ConvertAtlasAsset(SpriteAtlas atlas)
        {
            var sprites = new Sprite[atlas.spriteCount];

            atlas.GetSprites(sprites);

            return sprites;
        }

        private static Sprite[] ConvertTextureAsset(Texture2D texture)
        {
            return Sprite.Create(
                texture,
                new Rect(x: 0f, y: 0f, texture.width, texture.height),
                new Vector2(texture.width / 2, texture.height / 2),
                texture.GetPixels().Length).ToSeq().ToArray();
        }

        public override async UniTask LoadAssetsAsync(AssetLabels assetLabels)
        {
            var tasks = new TempList<UniTask>()
            {
                base.LoadAssetsAsync(assetLabels),
                LoadAssetsAsync<SpriteAtlas>(assetLabels, ConvertAtlasAsset),
                TexturesAsSprites ? LoadAssetsAsync<Texture2D>(assetLabels, ConvertTextureAsset) : UniTask.CompletedTask
            };

            await UniTask.WhenAll((UniTask[])tasks);
        }
    }
}
