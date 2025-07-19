using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UTIRLib.Attributes;
using UTIRLib.InputSystem;

#nullable enable

namespace UTIRLib.UI
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public sealed class CanvasController : MonoX, ICanvasController
    {
        [RequiredProperty]
        public IPointerInput Pointer { get; private set; } = null!;

        [RequiredProperty]
        public IRaycasterUI Raycaster { get; private set; } = null!;

        protected override void OnAwake()
        {
            base.OnAwake();

            Raycaster = new RaycasterUI(Pointer,
                                        GetComponent<GraphicRaycaster>(),
                                        EventSystem.current);
        }
    }
}