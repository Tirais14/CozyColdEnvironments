#nullable enable
using CCEnvs.Reflection.ObjectModel;
using CCEnvs.Returnables;

namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface IInteractable
    {
        MethodResult Interact(ExplicitArguments args = default);
    }
    public interface IInteractable<T0> : IInteractable
    {
        new MethodResult<T0> Interact(ExplicitArguments args = default);

        MethodResult IInteractable.Interact(ExplicitArguments args) => Interact(args);
    }
    public interface IInteractable<T0, T1> : IInteractable<T0>
    {
        new MethodResult<T0, T1> Interact(ExplicitArguments args = default);

        MethodResult<T0> IInteractable<T0>.Interact(ExplicitArguments args)
        {
            return Interact(args);
        }
    }
    public interface IInteractable<T0, T1, T2> : IInteractable<T0, T1>
    {
        new MethodResult<T0, T1, T2> Interact(ExplicitArguments args = default);

        MethodResult<T0, T1> IInteractable<T0, T1>.Interact(ExplicitArguments args)
        {
            return Interact(args);
        }
    }
}
