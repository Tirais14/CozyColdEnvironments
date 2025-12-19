using CCEnvs.FuncLanguage;
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

        public SpriteAddressablesDatabase(int capacity)
            :
            base(capacity)
        {
        }

        public SpriteAddressablesDatabase()
        {
        }

        public static Sprite[] ConvertSpriteAtlas(SpriteAtlas atlas)
        {
            var sprites = new Sprite[atlas.spriteCount];

            atlas.GetSprites(sprites);

            return sprites;
        }

        public static Sprite[] ConvertTexture(Texture2D texture)
        {
            return Range.From(Sprite.Create(texture,
                                            new Rect(x: 0f, y: 0f, texture.width, texture.height),
                                            new Vector2(texture.width / 2, texture.height / 2),
                                            texture.GetPixels().Length));
        }
    }
}
