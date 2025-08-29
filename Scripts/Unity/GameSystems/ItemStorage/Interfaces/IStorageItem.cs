using UnityEngine;

#nullable enable
namespace CozyColdEnvironments.GameSystems.ItemStorageSystem
{
    public interface IStorageItem
    {
        string Name { get; }
        int ID { get; }
        Sprite Icon { get; }
        int MaxStackCount { get; }
    }
}
