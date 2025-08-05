using UnityEngine;
using UTIRLib.InputSystem;

#nullable enable

namespace UTIRLib.UI
{
    public interface ICanvasController
    {
        IPointerInput Pointer { get; }
        Vector2 PointerPosition { get; }
        ICanvasRaycaster CanvasRaycaster { get; }
    }
}