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
        public ICanvasRaycaster CanvasRaycaster { get; protected set; } = null!;

        protected override void OnAwake()
        {
            base.OnAwake();

            CanvasRaycaster = new CanvasRaycaster(EventSystem.current,
                                        GetComponent<GraphicRaycaster>());
        }
    }
}