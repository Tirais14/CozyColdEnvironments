using System;
using R3;
using UnityEngine.InputSystem;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    public class ButtonActionRx : InputActionRx<bool>
    {
        public override Observable<bool> TRaw => Raw.Select(ctx => ctx.ReadValueAsButton());
        public override Observable<bool> TStarted => Started.Select(ctx => ctx.ReadValueAsButton());
        public override Observable<bool> TPerformed => Performed.Select(ctx => ctx.ReadValueAsButton());
        public override Observable<bool> TCanceled => Canceled.Select(ctx => ctx.ReadValueAsButton());

        public ButtonActionRx(InputAction inputAction) : base(inputAction)
        {
        }
    }
}
