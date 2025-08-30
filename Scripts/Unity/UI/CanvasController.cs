using CCEnvs.Unity.ComponentSetter;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

#nullable enable

namespace CCEnvs.Unity.UI
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public class CanvasController : MonoCC, ICanvasController
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