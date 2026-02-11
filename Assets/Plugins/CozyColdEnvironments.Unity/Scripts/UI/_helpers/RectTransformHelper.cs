using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public static class RectTransformHelper
    {
        /// <summary>
        /// If sensivity is not null direction will be normalized and multiplied by sensivity
        /// </summary>
        /// <returns>In the rect position</returns>
        public static RectTransform MoveByDirectionInRect(
            this RectTransform source,
            Vector2 direction,
            Rect rect,
            float? sensivity = null)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            if (direction == Vector2.zero)
                return source;

            CC.Guard.IsNotDefault(rect, nameof(rect));

            Vector2 targetPos = source.anchoredPosition;

            if (sensivity is null)
                targetPos += direction;
            else
                targetPos += direction.normalized * sensivity.Value;

            targetPos = targetPos.ClampInRect(rect);

            source.anchoredPosition = targetPos;

            return source;
        }

        public static RectTransform SetRelativePosition(
            this RectTransform source,
            Vector2 position,
            Rect rect)
        {
            CC.Guard.IsNotNull(source, nameof(source));
            CC.Guard.IsNotDefault(rect, nameof(rect));

            position = position.ClampInRect(rect);

            source.anchoredPosition = position;

            return source;
        }
    }
}
