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
        protected float resolution = 1f;

        private readonly List<SplineSamplerSegment> segments = new();

        private ReactiveCommand<Unit>? onRecalcuate;

        [GetBySelf]
        private SplineContainer splineContainer = null!;

        public IReadOnlyList<SplineSamplerSegment> Segments => segments;

        protected override void Start()
        {
            base.Start();
            Spline.Changed += OnSplineChanged;
            Recalcuate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CCDisposable.Dispose(ref onRecalcuate);
            Spline.Changed -= OnSplineChanged;
        }

        private void OnSplineChanged(Spline spline, int idx, SplineModification modType)
        {
            var splines = splineContainer.Splines;

            if (idx >= splines.Count
                ||
                splines[idx] != spline)
            {
                return;
            }

            Recalcuate();
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;

            for (int i = 0; i < segments.Count; i++)
                Gizmos.DrawSphere(cTransform.TransformPoint(segments[i].Position), 0.15f);
        }

        protected virtual void OnValidate()
        {
            if (splineIndex >= splineContainer.Splines.Count)
                splineIndex = splineContainer.Splines.Count - 1;

            Recalcuate();
        }
#endif

        public void Recalcuate()
        {
            Guard.IsGreaterThan(splineIndex, -1, nameof(splineIndex));

            segments.Clear();

            if (splineContainer.Splines.Count == 0)
                return;

            int segmentCount = CalculateSegmentCount();
            float t;

            Spline spline = splineContainer[splineIndex];
            SplineSamplerSegment segment;

            for (int i = 0; i <= segmentCount; i++)
            {
                t = i / (float)segmentCount;

                spline.Evaluate(
                    t,
                    out var pos,
                    out var tangent,
                    out var upVector
                    );

                segment = new SplineSamplerSegment(pos, tangent, upVector);

                segments.Add(segment);
            }

            OnRecalculate();
            onRecalcuate?.Execute(Unit.Default);
        }

        public int CalculateSegmentCount()
        {
            var segmentCountRaw = resolution * splineContainer.CalculateLength();

            return (int)math.floor(segmentCountRaw);
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
