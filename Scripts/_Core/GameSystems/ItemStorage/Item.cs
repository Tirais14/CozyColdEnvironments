using System.Collections.Generic;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.GameSystems.Storage;

#nullable enable
namespace UTIRLib.GameSystems
{
    public class Item : IItem, IEqualityComparer<IItem>
    {
        public string Name { get; } = "NullItem";
        public int ID { get; } = int.MinValue;
        public Sprite Icon { get; } = TirLib.ErrorSprite;

        protected Item()
        {
        }

        protected Item(string name, int id, Sprite icon)
        {
            Name = name;
            ID = id;
            Icon = icon;
        }

        public bool Equals(IItem x, IItem y)
        {
            return x.IsNotNull() && y.IsNotNull() && x.ID == y.ID;
        }

        public int GetHashCode(IItem obj)
        {
            return obj.ID.GetHashCode();
        }
    }
}
