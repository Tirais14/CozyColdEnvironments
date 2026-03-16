using R3;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    public class ButtonActionRx : InputActionRx<bool>
    {
        [Preserve]
        public ButtonActionRx(InputAction inputAction) : base(inputAction)
        {
        }

        public override Observable<bool> ObserveRawValue()
        {
            return ObserveRaw().Select(static ctx => ctx.ReadValueAsButton());
        }

        public override Observable<bool> ObserveStartedValue()
        {
            return ObserveStarted().Select(static ctx => ctx.ReadValueAsButton());
        }

        public override Observable<bool> ObservePerformedValue()
        {
            return ObservePerformed().Select(static ctx => ctx.ReadValueAsButton());
        }

        public override Observable<bool> ObserveCanceledValue()
        {
            return ObserveCanceled().Select(static ctx => ctx.ReadValueAsButton());
        }
    }
}
