using System;
using UnityEngine;

#nullable enable
namespace UTIRLib.UI.StorageSystem
{
    public interface IItemUI : IEquatable<IItemUI>
    {
        string Name { get; }
        int ID { get; }
        Sprite Icon { get; }
    }
}
