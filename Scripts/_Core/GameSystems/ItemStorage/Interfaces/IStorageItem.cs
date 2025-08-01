using UnityEngine;

#nullable enable
namespace UTIRLib.GameSystems.ItemStorageSystem
{
    public interface IStorageItem
    {
        string Name { get; }
        int ID { get; }
        Sprite Icon { get; }
    }
}
