using CCEnvs.Patterns.Commands;
using CCEnvs.Unity.Items;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

#nullable enable
#pragma warning disable S101
namespace CCEnvs.Unity
{
    public delegate UniTask<TOutput> ConverterAsync<in TInput, TOutput>(TInput input);

    public static class UCC
    {
        public static Lazy<Sprite> ColorSprite { get; } = new(() => Resources.Load<Sprite>("Textures/ColorSprite"));
        public static Lazy<Sprite> Transparent { get; } = new(() => Resources.Load<Sprite>("Textures/DummySprite"));
        public static Lazy<Sprite> RedCrossSprite { get; } = new(() => Resources.Load<Sprite>("Textures/RedCross"));
        public static Lazy<IInventory> WorldInventory { get; } = new(() => new Inventory());
        public static Lazy<Material> MockMaterial { get; } = new(() => Resources.Load<Material>("CC/Mock_Material"));
    }
}
