#nullable enable
using CCEnvs.FuncLanguage;

namespace CCEnvs.Unity.Interactables
{
    public interface IInteractableBase
    {
        Either<string, object> Interact(object? tool);
    }
}
