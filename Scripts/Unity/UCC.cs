using Cysharp.Threading.Tasks;
using UnityEngine;

#nullable enable
#pragma warning disable S101
namespace CCEnvs.Unity
{
    public delegate UniTask<TOutput> ConverterAsync<in TInput, TOutput>(TInput input);

    public class UCC : CCBehaviourStatic<UCC>
    {
        public static class Tags
        {
            public const string TRANSFORM_OVERRIDE = "TransformOverride";
            public const string GAME_OBJECT_OVERRIDE = "GameObjectOverride";
        }

        public static Sprite ColorSprite { get; private set; } = null!;
        public static Sprite DummySprite { get; private set; } = null!;
        public static Sprite ErrorSprite { get; private set; } = null!;

        protected override void OnAwake()
        {
            base.OnAwake();

            ColorSprite = Resources.Load<Sprite>("Textures/ColorSprite");
            DummySprite = Resources.Load<Sprite>("Textures/DummySprite");
            ErrorSprite = Resources.Load<Sprite>("Textures/ErrorSprite");
        }
    }
}
