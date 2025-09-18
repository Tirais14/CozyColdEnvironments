using UnityEngine;

#nullable enable
#pragma warning disable S101
namespace CCEnvs.Unity
{
    public class UCC : MonoCCStatic<UCC>
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
