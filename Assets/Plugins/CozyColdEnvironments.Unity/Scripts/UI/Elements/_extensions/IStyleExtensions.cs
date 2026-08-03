using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public static class IStyleExtensions
    {
        public static Vector2 GetPosition(this IStyle source)
        {
            CC.Guard.IsNotNullSource(source);
            return new Vector2(source.left.value.value, source.top.value.value);
        }

        public static void SetPosition(this IStyle source, Vector2 position)
        {
            CC.Guard.IsNotNullSource(source);
            source.left = position.x;
            source.top = position.y;
        }
    }
}
