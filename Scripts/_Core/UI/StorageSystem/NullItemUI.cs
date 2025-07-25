#nullable enable
using System;
using UnityEngine;

namespace UTIRLib.UI.StorageSystem
{
    public sealed class NullItemUI : IItemUI
    {
        public string Name => string.Empty;
        public int ID => int.MinValue;
        public Sprite Icon => TirLib.ErrorSprite;

        public bool Equals(IItemUI other)
        {
            return other.ID == ID;
        }

        public override bool Equals(object obj)
        {
            return obj is IItemUI typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID);
        }
    }
}
