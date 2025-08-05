using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UTIRLib.Attributes;
using UTIRLib.InputSystem;

#nullable enable

namespace UTIRLib.UI
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public abstract class ACanvasController : MonoX, ICanvasController
    {
        [GetBySelf]
        public GraphicRaycaster RaycasterGraphic { get; private set; } = null!;

        [field: RequiredField]
        public IPointerInput Pointer { get; protected set; } = null!;

        [field: RequiredField]
        public ICanvasRaycaster RaycasterCanvas { get; protected set; } = null!;

        public Vector2 PointerPosition => Pointer.Value;
    }
}