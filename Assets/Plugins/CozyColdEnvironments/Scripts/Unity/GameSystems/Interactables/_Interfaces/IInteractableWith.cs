#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface IInteractableWith : IInteractableBase
    {
        object? Interact(object arg);
    }
    public interface IInteractableWith<in TIn, out TOut> : IInteractableWith
    {
        TOut Interact(TIn arg);

        object? IInteractableWith.Interact(object arg) => Interact(arg);
    }
}
