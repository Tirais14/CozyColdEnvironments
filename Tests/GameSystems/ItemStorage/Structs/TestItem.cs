using UnityEngine;
using UTIRLib.GameSystems.Storage;

#nullable enable
namespace UTIRLib.Tests.GameSystems.Storage
{
    public struct TestItem : IItem
    {
        public readonly string Name => "TestItem";
        public readonly int ID => 1;
        public readonly Sprite Icon => TirLib.DummySprite;
    }
}
