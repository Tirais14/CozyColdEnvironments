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
        [field: RequiredField]
        public IPointerInput Pointer { get; protected set; } = null!;

        [field: RequiredField]
        public IRaycasterUI Raycaster { get; protected set; } = null!;

        protected override void OnAwake()
        {
            base.OnAwake();

            Raycaster = new RaycasterUI(Pointer,
                                        GetComponent<GraphicRaycaster>(),
                                        EventSystem.current);
        }
    }
}