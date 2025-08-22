using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UTIRLib.Attributes;

#nullable enable

namespace UTIRLib.UI
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public class CanvasController : MonoX, ICanvasController
    {
        [SerializeField]
        private InputActionReference pointerRef = null!;

        [GetBySelf]
        public GraphicRaycaster RaycasterGraphic { get; private set; } = null!;

        [GetBySelf]
        public ICanvasRaycaster RaycasterCanvas { get; private set; } = null!;

        public InputAction Pointer => pointerRef;
    }
}