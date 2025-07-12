using UnityEngine;

#nullable enable

namespace UTIRLib
{
    public static class ColorExtensions
    {
        public static Color With(this Color color, float r, float g, float b) => new(r, g, b, color.a);

        public static Color WithRed(this Color color, float red) => new(red, color.g, color.b, color.a);

        public static Color WithGreen(this Color color, float green)
        {
            return new(color.r, green, color.b, color.a);
        }

        public static Color WithBlue(this Color color, float blue)
        {
            return new(color.r, color.g, blue, color.a);
        }

        public static Color WithAlpha(this Color color, float alpha)
        {
            return new(color.r, color.g, color.b, alpha);
        }
    }
}