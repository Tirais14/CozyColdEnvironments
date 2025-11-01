#nullable enable
using CCEnvs.FuncLanguage;

namespace CCEnvs.Unity.Interactables
{
    public interface IInteractableBase
    {
        int InteractionPriority => 0;
        bool IsInteractable => true;
        int InteractionLayer => -1;

        Either<string, object> Interact(object? tool);
    }
}
