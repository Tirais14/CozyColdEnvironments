#nullable enable

using UnityEngine;
namespace CozyColdEnvironments
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

        public static string WordSeparator { get; set; } = "_";

        public static class Create 
        {
            public static T[] Array<T>(params T[] values) => values;
        }
    }
}