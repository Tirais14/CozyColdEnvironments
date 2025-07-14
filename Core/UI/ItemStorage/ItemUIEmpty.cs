#nullable enable
using UnityEngine;

namespace UTIRLib.UI
{
    public class ItemUIEmpty : IItemUI
    {
        private readonly static Sprite emptyIcon = Sprite.Create(Texture2D.grayTexture, new Rect(), new Vector2());

        public string Name => string.Empty;
        public Sprite Icon => emptyIcon;
        public int MaxStackCount => -1;
    }
}
