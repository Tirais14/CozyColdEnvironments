using UnityEngine;
using UnityEngine.UI;
using UTIRLib.InputSystem;

#nullable enable

namespace UTIRLib.UI
{
    public interface ICanvasController
    {
        GraphicRaycaster RaycasterGraphic { get; }
        IPointerInput Pointer { get; }
        Vector2 PointerPosition { get; }
        ICanvasRaycaster RaycasterCanvas { get; }
    }
}