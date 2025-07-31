using UnityEngine;

#nullable enable
namespace UTIRLib.GameSystems.Storage
{
    public interface IItem
    {
        string Name { get; }
        int ID { get; }
        Sprite Icon { get; }
    }
}
