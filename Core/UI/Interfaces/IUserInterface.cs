using UnityEngine.EventSystems;
using UnityEngine.UI;
using UTIRLib.InputSystem;

#nullable enable

namespace UTIRLib.UI
{
    public interface IUserInterface : IOpenable, IStateNotifier<IUserInterface>
    {
        EventSystem EventSystem { get; }
        GraphicRaycaster DefaultRaycaster { get; }
        IPointerHandler PointerHandler { get; }
        IUIRaycaster Raycaster { get; }
    }
}