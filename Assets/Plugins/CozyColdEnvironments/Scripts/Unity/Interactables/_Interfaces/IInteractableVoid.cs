#nullable enable
using CCEnvs.FuncLanguage;

namespace CCEnvs.Unity.Interactables
{
    public interface IInteractableVoid : IInteractable
    {
        new void Interact();

        object? IInteractable.Interact() => null;

        Either<string, object> IInteractableBase.Interact(object? tool)
        {
            return Either<string, object>.None;
        }
    }
}
