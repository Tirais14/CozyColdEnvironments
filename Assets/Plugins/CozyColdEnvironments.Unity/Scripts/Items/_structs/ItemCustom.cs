using System;
using UnityEngine;

#nullable enable
#pragma warning disable S4035
namespace CCEnvs.Unity.Items
{
    [Serializable]
    public class ItemCustom : IItem
    {
        public static ItemCustom Empty { get; } = new();

        public string Name { get; init; } = "None";
        public int ID { get; init; } = int.MinValue;
        public Sprite Icon { get; init; } = UCC.RedCrossSprite.Value;
        public int MaxItemCount { get; init; } = int.MaxValue;

        public static bool operator ==(ItemCustom? left, ItemCustom? right)
        {
            return ItemEqulaityComparer.Default.Equals(left, right);
        }

        public static bool operator !=(ItemCustom? left, ItemCustom? right)
        {
            return !(left == right);
        }

        public bool Equals(IItem? other)
        {
            return this.To<IItem>() == other;
        }

        public override bool Equals(object? obj)
        {
            return obj is IItem typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return ItemEqulaityComparer.Default.GetHashCode(this);
        }
    }
}
