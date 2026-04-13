#if SPLINES_PLUGIN
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Splines
{
    [ExecuteInEditMode]
    public class SplineRoad : SplineSampler
    {
        [SerializeField, Min(0.01f)]
        protected float width;

        private readonly List<(float3 LeftPoint, float3 RightPoint)> sidePoints = new();

#if UNITY_2017_1_OR_NEWER
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.color = Color.white;

            for (int i = 0; i < sidePoints.Count; i++)
            {
                Gizmos.DrawSphere(cTransform.TransformPoint(sidePoints[i].LeftPoint), 0.15f);
                Gizmos.DrawSphere(cTransform.TransformPoint(sidePoints[i].RightPoint), 0.15f);
            }
        }
#endif

        protected override void OnRecalculate()
        {
            base.OnRecalculate();

            sidePoints.Clear();

            int segmentCount = math.min(Tangents.Count, UpVectors.Count);
            segmentCount = math.min(Segments.Count, segmentCount);

            for (int i = 0; i < segmentCount; i++)
            {
                GetSidePoints(
                    Segments[i],
                    Tangents[i],
                    UpVectors[i],
                    out var leftPoint,
                    out var rightPoint
                    );

                sidePoints.Add((leftPoint, rightPoint));
            }
        }

        private void GetSidePoints(
            float3 segment,
            float3 tangent,
            float3 upVector,
            out float3 leftPoint,
            out float3 rightPoint
            )
        {
            var right = math.cross(tangent, upVector);
            right = math.normalize(right);

            rightPoint = width * right + segment;
            leftPoint = width * -right + segment;
        }

        private void CalucalateVertices()
        {

        }
    }
}
#endif
