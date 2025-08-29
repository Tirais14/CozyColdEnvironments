using System;
using System.Collections.Generic;
using UnityEngine;
using CozyColdEnvironments.Diagnostics;
using CozyColdEnvironments.GameSystems.ItemStorageSystem;

#nullable enable
#pragma warning disable S4035
namespace CozyColdEnvironments.GameSystems
{
    public class StorageItem 
        :
        IStorageItem,
        IEquatable<IStorageItem>
    {
        public readonly static StorageItem Null = new();

        public string Name { get; } = "NullItem";
        public int ID { get; } = int.MinValue;
        public Sprite Icon { get; } = CC.ErrorSprite;
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

        public bool Equals(IStorageItem? other)
        {
            other ??= Null;

            return ID == other.ID;
        }

        public override bool Equals(object? obj)
        {
            obj ??= Null;

            return obj is IStorageItem typed && Equals(typed);
        }

        public override int GetHashCode() => ID.GetHashCode();

        public static bool operator ==(StorageItem left, StorageItem? right)
        {
            if (left.IsNull() && right.IsNull())
                return true;
            if (left.IsNull())
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(StorageItem left, StorageItem? right)
        {
            if (left.IsNull() && right.IsNull())
                return false;
            if (left.IsNull() || right.IsNull())
                return true;

            return !left.Equals(right);
        }
    }
}
