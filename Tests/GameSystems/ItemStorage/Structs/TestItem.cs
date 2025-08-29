using UnityEngine;
using CozyColdEnvironments.GameSystems.ItemStorageSystem;

#nullable enable
namespace CozyColdEnvironments.Tests.GameSystems.Storage
{
    public struct TestItem : IStorageItem
    {
        public readonly string Name => "TestItem";
        public readonly int ID => 1;
        public readonly Sprite Icon => CC.DummySprite;
        public readonly int MaxStackCount => 0;
    }
}
