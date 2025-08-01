using System.Collections.Generic;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.GameSystems.ItemStorageSystem;

#nullable enable
namespace UTIRLib.GameSystems
{
    public class StorageItem : IStorageItem, IEqualityComparer<IStorageItem>
    {
        public string Name { get; } = "NullItem";
        public int ID { get; } = int.MinValue;
        public Sprite Icon { get; } = TirLib.ErrorSprite;

        public StorageItem()
        {
        }

        public StorageItem(string name, int id, Sprite icon)
        {
            Name = name;
            ID = id;
            Icon = icon;
        }

        public bool Equals(IStorageItem x, IStorageItem y)
        {
            return x.IsNotNull() && y.IsNotNull() && x.ID == y.ID;
        }

        public int GetHashCode(IStorageItem obj)
        {
            return obj.ID.GetHashCode();
        }
    }
}
