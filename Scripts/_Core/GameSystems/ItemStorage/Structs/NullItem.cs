#nullable enable
using System;
using UnityEngine;

namespace UTIRLib.GameSystems.Storage
{
    public readonly struct NullItem : IItem, IEquatable<NullItem>
    {
        public readonly string Name => string.Empty;
        public readonly int ID => int.MinValue;
        public readonly Sprite Icon => TirLib.ErrorSprite;

        public bool Equals(NullItem other) => true;

        public override bool Equals(object obj)
        {
            return obj is NullItem typed && Equals(typed);
        }

        public override int GetHashCode() => 0;

        public static bool operator ==(NullItem a, NullItem b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(NullItem a, NullItem b)
        {
            return !a.Equals(b);
        }
    }
}
