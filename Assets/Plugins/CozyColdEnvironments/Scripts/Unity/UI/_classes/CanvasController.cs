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
        [GetBySelf]
        public GraphicRaycaster graphicRaycaster { get; private set; } = null!;

        [GetBySelf]
        public ICanvasRaycaster CanvasRaycaster { get; private set; } = null!;

        public InputActionRx<Vector2> PointerInput { get; private set; } = null!;

        protected override void Start()
        {
            base.Start();

            DependencyContainer.Resolve<InputActionRx<Vector2>>(DependencyID.PointerInput);
        }
    }
}