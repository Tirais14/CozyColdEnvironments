#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface IInteractableVoid : IInteractable
    {
        new void Interact();

        object? IInteractable.Interact() => null;
    }
}
