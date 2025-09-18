using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Storages
{
    public interface IStorageItem
    {
        string Name { get; }
        int ID { get; }
        Sprite Icon { get; }
        int MaxStackCount { get; }
    }
}
