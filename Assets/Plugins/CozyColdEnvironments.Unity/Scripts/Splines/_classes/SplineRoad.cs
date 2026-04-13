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
        [SerializeField]
        protected float width;

        private readonly List<(float3 LeftPoint, float3 RightPoint)> sidePoints = new();

#if UNITY_2017_1_OR_NEWER
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.color = Color.white;

            for (int i = 0; i < sidePoints.Count; i++)
            {
                Gizmos.DrawSphere(sidePoints[i].LeftPoint, 0.05f);
                Gizmos.DrawSphere(sidePoints[i].RightPoint, 0.05f);
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

            leftPoint = width * right + segment;
            rightPoint = width * -right + segment;
        }

        private void CalucalateVertices()
        {

        }
    }
}
#endif
