#nullable enable
using UTIRLib.Returnables;

namespace UTIRLib.GameSystems.Interactables
{
    public interface IInteractable
    {
        MethodResult Interact();
    }
}
