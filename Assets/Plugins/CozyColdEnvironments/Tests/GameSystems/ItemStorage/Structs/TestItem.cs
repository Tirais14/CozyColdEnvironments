using CCEnvs.Unity.GameSystems.Storages;
using UnityEngine;

#nullable enable
namespace CCEnvs.Tests.GameSystems.Storage
{
    public struct TestItem : IItem
    {
        public readonly string Name => "TestItem";
        public readonly int ID => 1;
        public readonly Sprite Icon => Unity.UCC.DummySprite;
        public readonly int MaxItemCount => 0;
    }
}
