using CCEnvs.Dependencies;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Dependencies;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace CCEnvs.Unity.UI
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

            DependencyContainer.Resolve<PointerInputActionRx>(UnityDependecyID.PointerInput);
        }
    }
}