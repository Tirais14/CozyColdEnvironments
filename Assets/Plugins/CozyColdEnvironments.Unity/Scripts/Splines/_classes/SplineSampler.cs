#if SPLINES_PLUGIN
using CCEnvs.Disposables;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CommunityToolkit.Diagnostics;
using R3;
using System.Collections.Generic;
using Unity.Mathematics;
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
        protected int splineIndex = 0;

        [SerializeField, Min(0.01f)]
        protected float resolution = 3f;

        private readonly List<float3> segments = new();
        private readonly List<float3> tangents = new();
        private readonly List<float3> upVectors = new();

        private ReactiveCommand<Unit>? onRecalcuate;

        [GetBySelf]
        private SplineContainer splineContainer = null!;

        public IReadOnlyList<float3> Segments => segments;
        public IReadOnlyList<float3> Tangents => tangents;
        public IReadOnlyList<float3> UpVectors => upVectors;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CCDisposable.Dispose(ref onRecalcuate);
        }

        private void Update()
        {
            Recalcuate();
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;

            for (int i = 0; i < segments.Count; i++)
                Gizmos.DrawSphere(segments[i], 0.05f);
        }
#endif

        public void Recalcuate()
        {
            Guard.IsGreaterThan(splineIndex, -1, nameof(splineIndex));

            segments.Clear();
            tangents.Clear();
            upVectors.Clear();

            if (splineContainer.Splines.Count == 0)
                return;

            float segmentCount = GetSegmentCount();
            float t;

            Spline spline = splineContainer[splineIndex];

            for (int i = 0; i <= segmentCount; i++)
            {
                t = i / (float)segmentCount;

                spline.Evaluate(
                    t,
                    out var pos,
                    out var tangent,
                    out var upVector
                    );

                segments.Add(pos);
                tangents.Add(tangent);
                upVectors.Add(upVector);
            }

            OnRecalculate();
            onRecalcuate?.Execute(Unit.Default);
        }

        public float GetSegmentCount()
        {
            return resolution * splineContainer.CalculateLength();
        }

        public Observable<Unit> ObserveRecalculate()
        {
            onRecalcuate ??= new ReactiveCommand<Unit>();
            return onRecalcuate;
        }

        protected virtual void OnRecalculate() { }
    }
}
#endif
