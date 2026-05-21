using CCEnvs.Dependencies;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.Dependencies;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX
{
    /// <summary>
    /// Marks <see cref="Canvas"/> as <see cref="Canvas"/> with higher sort order for correct overlapping UI elements on example while is dragging any object
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public sealed class HighPriorityCanvasMarker : CCBehaviour
    {
        protected override void Awake()
        {
            base.Awake();

            CCServices.Bind(
                GetComponent<Canvas>(),
                UnityDependecyID.HighPriorityCanvas
                );

            CCServices.Bind(this);
        }
    }
}
