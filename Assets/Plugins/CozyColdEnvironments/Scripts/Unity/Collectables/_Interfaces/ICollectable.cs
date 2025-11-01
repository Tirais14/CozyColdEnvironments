using CCEnvs.Unity.Interactables;

#nullable enable
namespace CCEnvs.Unity
{
    public interface ICollectable : IInteractable
    {
        object? Collect();

        object? IInteractable.Interact() => Collect();
    }
    public interface ICollectable<out T> : ICollectable
    {
        new T Collect();

        object? ICollectable.Collect() => Collect();
    }
}
