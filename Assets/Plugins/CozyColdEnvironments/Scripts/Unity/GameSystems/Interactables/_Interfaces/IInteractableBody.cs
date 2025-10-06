using CCEnvs.Returnables;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface IInteractableBody
    {
        MethodResult GetInteractionResult();
    }
    public interface IInteractableBody<T0> : IInteractableBody
    {
        new MethodResult<T0> GetInteractionResult();

        MethodResult IInteractableBody.GetInteractionResult()
        {
            return GetInteractionResult();
        }
    }
    public interface IInteractionTransmitter<T0, T1> : IInteractableBody<T0>
    {
        new MethodResult<T0, T1> GetInteractionResult();

        MethodResult<T0> IInteractableBody<T0>.GetInteractionResult()
        {
            return GetInteractionResult();
        }
    }
    public interface IInteractionTransmitter<T0, T1, T2> : IInteractionTransmitter<T0, T1>
    {
        new MethodResult<T0, T1, T2> GetInteractionResult();

        MethodResult<T0, T1> IInteractionTransmitter<T0, T1>.GetInteractionResult()
        {
            return GetInteractionResult();
        }
    }
}
