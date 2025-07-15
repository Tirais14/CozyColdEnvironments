using UnityEngine.EventSystems;
using UnityEngine.UI;
using UTIRLib.InputSystem;

#nullable enable

namespace UTIRLib.UI
{
    public interface IUserInterface : IOpenable
    {
        EventSystem EventSystem { get; }
        GraphicRaycaster DefaultRaycaster { get; }
        IPointerHandler PointerHandler { get; }
        IRaycasterUI Raycaster { get; }
    }
}