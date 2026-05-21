using CCEnvs.UnityX.InputSystem.Rx;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.UnityX.UI
{
    public interface ICanvasController
    {
        GraphicRaycaster graphicRaycaster { get; }
        ICanvasRaycaster CanvasRaycaster { get; }
        PointerInputActionRx PointerInput { get; }
        Canvas canvas { get; }
    }
}