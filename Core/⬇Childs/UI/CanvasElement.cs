using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UTIRLib.Diagnostics;

#nullable enable

namespace UTIRLib.UI
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class CanvasElement : MonoX
    {
        private Canvas? canvasInternal;
        private RectTransform? rectTransformInternal;

        protected Canvas canvas {
            get {
                if (canvasInternal == null)
                {
                    canvasInternal = GetComponentInParent<Canvas>();
                }

                return canvasInternal;
            }
        }

        protected RectTransform rectTransform {
            get {
                if (rectTransformInternal == null)
                {
                    rectTransformInternal = (RectTransform)transform;
                }

                return rectTransformInternal;
            }
        }

        public bool InWorldSpace => canvas.renderMode == RenderMode.WorldSpace;
        public bool InScreenSpace => !InWorldSpace;

        public Vector2 ScreenPosition {
            get {
                Vector2 result;
                if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    result = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera,
                                                                     rectTransform.position);
                }
                else result = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);

                TirLibDebug.Assert(result.x > Screen.width || result.y > Screen.height, "Error while converting world position to screen point.", this);

                return result;
            }
        }
    }
}