using CCEnvs;
using CCEnvs.Collections.Performance;
using CCEnvs.FuncLanguage;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
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

        public SpriteAddressablesDatabase(Identifier id, int capacity) : base(id, capacity)
        {
        }

        public SpriteAddressablesDatabase(int capacity)
            :
            base(capacity)
        {
        }

        public SpriteAddressablesDatabase(Identifier id) : base(id)
        {
        }

        public SpriteAddressablesDatabase()
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
                texture.GetPixels().Length).AsBox().ToArray();
        }

        public override async UniTask LoadAssetsAsync(AssetLabels assetLabels)
        {
            var tasks = new List<UniTask>()
            {
                base.LoadAssetsAsync(assetLabels),
                LoadAssetsByLabelsAsync<SpriteAtlas>(assetLabels, ConvertAtlasAsset),
                TexturesAsSprites ? LoadAssetsByLabelsAsync<Texture2D>(assetLabels, ConvertTextureAsset) : UniTask.CompletedTask
            };

            await UniTask.WhenAll(tasks);
        }
    }
}
