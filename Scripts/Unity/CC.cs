using UnityEngine;

#nullable enable
#pragma warning disable S101
namespace CCEnvs.Unity
{
    public static class CC
    {
        public static class Tags
        {
            public const string TRANSFORM_OVERRIDE = "TransformOverride";
            public const string GAME_OBJECT_OVERRIDE = "GameObjectOverride";
        }

        public readonly static LazyCC<Sprite> ColorSprite = new(() =>
        {
            return Resources.Load<Sprite>("Textures/ColorSprite");
        });

        public readonly static LazyCC<Sprite> DummySprite = new(() =>
        {
            return Resources.Load<Sprite>("Textures/DummySprite");
        });

        public readonly static LazyCC<Sprite> ErrorSprite = new(() =>
        {
            return Resources.Load<Sprite>("Textures/ErrorSprite");
        });
    }
}
