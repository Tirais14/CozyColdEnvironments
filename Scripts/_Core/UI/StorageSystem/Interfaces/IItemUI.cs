using UnityEngine;

#nullable enable
namespace UTIRLib.UI.StorageSystem
{
    public interface IItemUI
    {
        string Name { get; }
        int ID { get; }
        Sprite Icon { get; }
        int MaxStackCount { get; }
    }
}
