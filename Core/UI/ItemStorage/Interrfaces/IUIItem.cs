using UnityEngine;

namespace UTIRLib.UI
{
    public interface IUIItem
    {
        string Name { get; }
        Sprite Icon { get; }
        int MaxStackCount { get; }
    }
}
