using CCEnvs.Dependencies;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Dependencies;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
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

            BuiltInDependecyContainer.Bind(GetComponent<Canvas>(),
                UnityDependecyID.HighPriorityCanvas);
        }
    }
}
