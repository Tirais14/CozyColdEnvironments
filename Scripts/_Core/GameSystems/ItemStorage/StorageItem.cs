using System;
using System.Collections.Generic;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.GameSystems.ItemStorageSystem;

#nullable enable
namespace UTIRLib.GameSystems
{
    public class StorageItem : IStorageItem, IEquatable<IStorageItem>, IEqualityComparer<IStorageItem>
    {
        public readonly static StorageItem Null = new();

        public string Name { get; } = "NullItem";
        public int ID { get; } = int.MinValue;
        public Sprite Icon { get; } = TirLib.ErrorSprite;
        public int MaxStackCount { get; } = 0;

        public StorageItem()
        {
        }

        public StorageItem(string name, int id, Sprite icon, int maxStackCount = 0)
        {
            Name = name;
            ID = id;
            Icon = icon;
            MaxStackCount = maxStackCount;
        }

        public bool Equals(IStorageItem other)
        {
            if (other.IsNull())
                return false;

            return ID == other.ID;
        }

        public bool Equals(IStorageItem x, IStorageItem y)
        {
            if (x.IsNull() && y.IsNull())
                return true;
            if (x.IsNull())
                return false;

            return x.ID == y.ID;
        }

        public override bool Equals(object obj)
        {
            return obj is IStorageItem typed && Equals(typed);
        }

        public override int GetHashCode() => ID.GetHashCode();

        public int GetHashCode(IStorageItem obj)
        {
            return obj.ID.GetHashCode();
        }

        public static bool operator ==(StorageItem left, StorageItem right)
        {
            if (left.IsNull() && right.IsNull())
                return true;
            if (left.IsNull())
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(StorageItem left, StorageItem right)
        {
            if (left.IsNull() && right.IsNull())
                return false;
            if (left.IsNull() || right.IsNull())
                return true;

            return !left.Equals(right);
        }
    }
}
