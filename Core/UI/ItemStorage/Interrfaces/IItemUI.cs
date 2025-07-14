using UnityEngine;

#nullable enable
namespace UTIRLib.UI
{
    public interface IItemUI
    {
        string Name { get; }
        Sprite Icon { get; }
        int MaxStackCount { get; }
    }
}
