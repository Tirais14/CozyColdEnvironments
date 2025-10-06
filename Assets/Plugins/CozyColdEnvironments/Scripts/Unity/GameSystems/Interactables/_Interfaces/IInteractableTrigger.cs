#nullable enable
using CCEnvs.Reflection.Data;
using CCEnvs.Returnables;

namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface IInteractableTrigger : IToggleable
    {
        MethodResult InitiateInteraction(ExplicitArguments args = default);
    }
    public interface IInteractableTrigger<T0> : IInteractableTrigger
    {
        new MethodResult<T0> InitiateInteraction(ExplicitArguments args = default);

        MethodResult IInteractableTrigger.InitiateInteraction(ExplicitArguments args) => InitiateInteraction(args);
    }
    public interface IInteractableTrigger<T0, T1> : IInteractableTrigger<T0>
    {
        new MethodResult<T0, T1> InitiateInteraction(ExplicitArguments args = default);

        MethodResult<T0> IInteractableTrigger<T0>.InitiateInteraction(ExplicitArguments args)
        {
            return InitiateInteraction(args);
        }
    }
    public interface IInteractableTrigger<T0, T1, T2> : IInteractableTrigger<T0, T1>
    {
        new MethodResult<T0, T1, T2> InitiateInteraction(ExplicitArguments args = default);

        MethodResult<T0, T1> IInteractableTrigger<T0, T1>.InitiateInteraction(ExplicitArguments args)
        {
            return InitiateInteraction(args);
        }
    }
}
