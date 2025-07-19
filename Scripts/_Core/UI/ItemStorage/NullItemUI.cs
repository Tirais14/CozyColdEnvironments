#nullable enable
using UnityEngine;

namespace UTIRLib.UI
{
    public class NullItemUI : IItemUI
    {
        public string Name => string.Empty;
        public Sprite Icon => TirLib.ErrorSprite;
        public int MaxStackCount => -1;
    }
}
