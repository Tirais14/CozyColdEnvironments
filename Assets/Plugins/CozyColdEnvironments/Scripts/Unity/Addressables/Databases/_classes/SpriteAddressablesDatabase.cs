using CCEnvs.Diagnostics;
using Cysharp.Threading.Tasks;
using SuperLinq;
using System;
using System.Collections.Generic;
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

        public override async UniTask LoadAssetsAsync(AssetLabels assetLabels)
        {
            CC.Guard.Argument(assetLabels,
                              nameof(assetLabels),
                              assetLabels.IsNotDefault());

            try
            {
                OnStartLoadingInternal();

                await LoadAssetsInternalAsync(assetLabels.ToArray());

                AsyncOperationHandle handle = await AddressableLoader.LoadAssetsByLabelsAsync<SpriteAtlas>(
                    assetLabels.ToArray(),
                    AddAtlasAsset);

                loadHandles.Add(handle);

                if (TexturesAsSprites)
                {
                    handle = await AddressableLoader.LoadAssetsByLabelsAsync<Texture2D>(
                        assetLabels.ToArray(),
                        AddTextureAsset);

                    loadHandles.Add(handle);
                }
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
            finally
            {
                OnLoadedInternal();
                TrimExcess();
            }
        }

        private void AddAtlasAsset(SpriteAtlas atlas)
        {
            var sprites = new Sprite[atlas.spriteCount];

            atlas.GetSprites(sprites);

            sprites.ForEach(AddAsset);
        }

        private void AddTextureAsset(Texture2D texture)
        {
            var sprite = Sprite.Create(
                texture,
                new Rect(x: 0f, y: 0f, texture.width, texture.height),
                new Vector2(texture.width / 2, texture.height / 2),
                texture.GetPixels().Length);

            AddAsset(sprite);
        }
    }
}
