#nullable enable
namespace CCEnvs.Unity.Interactables
{
    public interface IInteractableVoid : IInteractable
    {
        new void Interact();

        object? IInteractable.Interact() => null;
    }
}
