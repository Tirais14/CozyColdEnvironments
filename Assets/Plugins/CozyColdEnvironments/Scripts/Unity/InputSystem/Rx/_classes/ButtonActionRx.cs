using System;
using UniRx;
using UnityEngine.InputSystem;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    public class ButtonActionRx : InputActionRx<bool>
    {
        public override IObservable<bool> TRaw => Raw.Select(ctx => ctx.ReadValueAsButton());
        public override IObservable<bool> TStarted => Started.Select(ctx => ctx.ReadValueAsButton());
        public override IObservable<bool> TPerformed => Performed.Select(ctx => ctx.ReadValueAsButton());
        public override IObservable<bool> TCanceled => Canceled.Select(ctx => ctx.ReadValueAsButton());

        public ButtonActionRx(InputAction inputAction) : base(inputAction)
        {
        }
    }
}
