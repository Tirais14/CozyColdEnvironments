#nullable enable
using UnityEngine;
using UTIRLib.UI.StorageSystem;

namespace UTIRLib.UI.ItemSystem
{
    public class NullItemUI : IItemUI
    {
        public string Name => string.Empty;
        public int ID => int.MinValue;
        public Sprite Icon => TirLib.ErrorSprite;
        public int MaxStackCount => int.MinValue;
    }
}
