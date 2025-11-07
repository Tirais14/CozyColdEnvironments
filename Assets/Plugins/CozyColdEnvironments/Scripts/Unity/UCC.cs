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
        public static class Tags
        {
            public const string TRANSFORM_OVERRIDE = "TransformOverride";
            public const string GAME_OBJECT_OVERRIDE = "GameObjectOverride";
        }

        public static Lazy<Sprite> ColorSprite { get; } = new(() => Resources.Load<Sprite>("Textures/ColorSprite"));
        public static Lazy<Sprite> TranparentSprite { get; } = new(() => Resources.Load<Sprite>("Textures/DummySprite"));
        public static Lazy<Sprite> ErrorSprite { get; } = new(() => Resources.Load<Sprite>("Textures/ErrorSprite"));
    }
}
