using CCEnvs.Unity.InputSystem.Rx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.UI
{
    public interface ICanvasController
    {
        GraphicRaycaster graphicRaycaster { get; }
        ICanvasRaycaster CanvasRaycaster { get; }
        InputActionRx<Vector2> PointerInput { get; }
        Canvas canvas { get; }
    }
}