#nullable enable
using CozyColdEnvironments.Returnables;

namespace CozyColdEnvironments.GameSystems.Interactables
{
    public interface IInteractable
    {
        MethodResult Interact();
    }
    public interface IInteractable<T0> : IInteractable
    {
        new MethodResult<T0> Interact();

        MethodResult IInteractable.Interact() => Interact();
    }
    public interface IInteractable<T0, T1> : IInteractable<T0>
    {
        new MethodResult<T0, T1> Interact();

        MethodResult<T0> IInteractable<T0>.Interact() => Interact();
    }
    public interface IInteractable<T0, T1, T2> : IInteractable<T0, T1>
    {
        new MethodResult<T0, T1, T2> Interact();

        MethodResult<T0, T1> IInteractable<T0, T1>.Interact() => Interact();
    }
}
