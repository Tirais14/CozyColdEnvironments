#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace UTIRLib
{
    public static class TirLib
    {
        public readonly static Lazy<Sprite> ColorSprite = new(() =>
        {
            return Resources.Load<Sprite>("Textures/ColorSprite");
        });

        public readonly static Lazy<Sprite> DummySprite = new(() =>
        {
            return Resources.Load<Sprite>("Textures/DummySprite");
        });

        public readonly static Lazy<Sprite> ErrorSprite = new(() =>
        {
            return Resources.Load<Sprite>("Textures/ErrorSprite");
        });

        public static string WordSeparator { get; set; } = "_";
    }
}