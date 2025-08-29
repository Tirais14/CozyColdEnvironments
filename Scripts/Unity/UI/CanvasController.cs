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
        private InputActionReference m_PointerInput = null!;

        [field: GetBySelf]
        public GraphicRaycaster RaycasterGraphic { get; private set; } = null!;

        [field: GetBySelf]
        public ICanvasRaycaster RaycasterCanvas { get; private set; } = null!;

        public InputAction Pointer => m_PointerInput;
    }
}