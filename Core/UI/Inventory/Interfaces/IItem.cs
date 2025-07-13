using UnityEngine;

#nullable enable

namespace UTIRLib.UI
{
    public interface IItem
    {
        string ItemName { get; }

        Sprite Sprite { get; }

        int MaxQuantity { get; }
    }
}