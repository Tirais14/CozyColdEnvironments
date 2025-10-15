#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface IInteractable
    {
        object? Interact();
    }
    public interface IInteractable<out T> : IInteractable
    {
        new T Interact();

        object? IInteractable.Interact() => Interact();
    }
}
