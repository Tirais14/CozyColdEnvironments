using UTIRLib.InputSystem;

#nullable enable

namespace UTIRLib.UI
{
    public interface ICanvasController
    {
        IPointerInput Pointer { get; }
        IRaycasterUI Raycaster { get; }
    }
}