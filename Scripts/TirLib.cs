#nullable enable

using UnityEngine;
namespace UTIRLib
{
    public static class TirLib
    {
        public static class Tags 
        {
            public const string TRANSFORM_OVERRIDE = "TransformOverride";
            public const string GAME_OBJECT_OVERRIDE = "GameObjectOverride";
        }

        public readonly static LazyX<Sprite> ColorSprite = new(() =>
        {
            return Resources.Load<Sprite>("Textures/ColorSprite");
        });

        public readonly static LazyX<Sprite> DummySprite = new(() =>
        {
            return Resources.Load<Sprite>("Textures/DummySprite");
        });

        public readonly static LazyX<Sprite> ErrorSprite = new(() =>
        {
            return Resources.Load<Sprite>("Textures/ErrorSprite");
        });

        public static string WordSeparator { get; set; } = "_";
    }
}