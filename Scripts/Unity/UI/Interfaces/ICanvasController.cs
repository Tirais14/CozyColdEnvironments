using UnityEngine.InputSystem;
using UnityEngine.UI;

#nullable enable

namespace CozyColdEnvironments.UI
{
    public interface ICanvasController
    {
        GraphicRaycaster RaycasterGraphic { get; }
        InputAction Pointer { get; }
        ICanvasRaycaster RaycasterCanvas { get; }
    }
}