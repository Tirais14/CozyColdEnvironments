#if SPLINES_PLUGIN
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

#nullable enable
namespace CCEnvs.Unity.Splines
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SplineContainer))]
    public class SplineSampler : CCBehaviour
    {
        [SerializeField]
        protected int splineIndex;

        [SerializeField, Range(0f, 1f)]
        protected float time;

        [SerializeField, Min(0f)]
        protected float resolution;

        [GetBySelf]
        protected SplineContainer splineContainer = null!;

        protected float3 position;
        protected float3 forward;
        protected float3 upVector;

        protected virtual void Update()
        {
            splineContainer.Evaluate(
                splineIndex,
                time,
                out position,
                out forward,
                out upVector
                );

            splineContainer.CalculateLength();
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            Handles.matrix = transform.localToWorldMatrix;
            Handles.SphereHandleCap(
                0,
                position,
                Quaternion.identity,
                1f,
                EventType.Repaint
                );
        }
#endif
    }
}
#endif
