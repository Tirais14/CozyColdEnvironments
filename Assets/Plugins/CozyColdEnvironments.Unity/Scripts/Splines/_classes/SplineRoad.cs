#if SPLINES_PLUGIN
using Unity.Mathematics;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Splines
{
    public class SplineRoad : SplineSampler
    {
        [SerializeField]
        protected float width;

        private void GetSidePositions(
            out float3 leftPos,
            out float3 rightPos
            )
        {
            var right = math.cross(forward, upVector);

            leftPos = width * right + position;
            rightPos = width * -right + position;
        }

        private void CalucalateVertices()
        {

        }
    }
}
#endif
