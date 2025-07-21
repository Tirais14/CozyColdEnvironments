#nullable enable
using UnityEngine;

namespace UTIRLib.UI.StorageSystem
{
    public class NullItemUI : IItemUI
    {
        public string Name => string.Empty;
        public int ID => int.MinValue;
        public Sprite Icon => TirLib.ErrorSprite;
        public int MaxStackCount => int.MinValue;
    }
}
