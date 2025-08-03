using UnityEngine;
using UTIRLib.GameSystems.ItemStorageSystem;

#nullable enable
namespace UTIRLib.Tests.GameSystems.Storage
{
    public struct TestItem : IStorageItem
    {
        public readonly string Name => "TestItem";
        public readonly int ID => 1;
        public readonly Sprite Icon => TirLib.DummySprite;
        public readonly int MaxStackCount => 0;
    }
}
