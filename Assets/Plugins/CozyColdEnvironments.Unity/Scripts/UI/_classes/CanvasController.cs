using CCEnvs.Services;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.Dependencies;
using CCEnvs.UnityX.Injections;
using CCEnvs.UnityX.InputSystem.Rx;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace CCEnvs.UnityX.UI
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public class CanvasController : CCBehaviour, ICanvasController
    {
        [field: GetBySelf]
        public GraphicRaycaster graphicRaycaster { get; private set; } = null!;

        [field: GetBySelf]
        public ICanvasRaycaster CanvasRaycaster { get; private set; } = null!;

        public PointerInputActionRx PointerInput { get; private set; } = null!;

        [field: GetBySelf]
        public Canvas canvas { get; private set; } = null!;

        protected override void Start()
        {
            base.Start();

            CCServices.Resolve<PointerInputActionRx>(UnityDependecyID.PointerInput);
        }
    }
}