#if SPLINES_PLUGIN
using CCEnvs.Collections;
using CCEnvs.Pools;
using CCEnvs.Unity.Injections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

#nullable enable
namespace CCEnvs.Unity.Splines
{
    [ExecuteInEditMode, DisallowMultipleComponent]
    public class SplineRoad : SplineSampler
    {
        [SerializeField, Min(0.01f)]
        protected float width;

        [SerializeField, Min(0.01f)]
        protected float depth = 0.5f;

        [SerializeField, Min(0f)]
        protected float borderOffset = 0.15f;

        private readonly List<SplineRoadSegment> roadSegments = new();

        [SerializeField, HideInInspector]
        private MeshFilter? meshFilter;

        private int meshFilterSplineIdx = -1;

        public IReadOnlyList<SplineRoadSegment> RoadSegments => roadSegments;

        protected string MeshFilterName => $"{name}[MeshFilter][{splineIndex}]";

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (Time.frameCount % 30 != 0)
                return;

            Recalcuate();
#endif
        }

#if UNITY_2017_1_OR_NEWER
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.color = Color.white;

            SplineRoadSegment roadSegment;

            for (int i = 0; i < roadSegments.Count; i++)
            {
                roadSegment = roadSegments[i];

                for (int j = 0; j < roadSegment.Count; j++)
                    Gizmos.DrawSphere(cTransform.TransformPoint(roadSegment[j]), 0.15f);
            }
        }
#endif

        protected override void OnRecalculate()
        {
            base.OnRecalculate();

            roadSegments.Clear();
            SplineSamplerSegment samplerSegment;
            SplineRoadSegment roadSegment;

            for (int i = 0; i < Segments.Count; i++)
            {
                samplerSegment = Segments[i];
                roadSegment = GetRoadSegment(samplerSegment);

                roadSegments.Add(roadSegment);
            }

            var mesh = BuildMesh();
            GetMeshFilter().sharedMesh = mesh;
        }

        private MeshFilter CreateMeshFilter()
        {
            if (this.meshFilter != null)
                Destroy(this.meshFilter.gameObject);

            var go = new GameObject(
                $"{name}[MeshFilter][{splineIndex}]",
                typeof(MeshFilter),
                typeof(MeshRenderer)
                );

            go.transform.SetParent(cTransform);

            var meshFilter = go.GetComponent<MeshFilter>();
            meshFilterSplineIdx = splineIndex;

            return meshFilter;
        }

        private bool TryGetMeshFilter([NotNullWhen(true)] out MeshFilter? meshFilter)
        {
            return this.Q()
                .WithName(MeshFilterName, byFullName: true)
                .Component<MeshFilter>()
                .Lax()
                .TryGetValue(out meshFilter);
        }

        private MeshFilter GetMeshFilter()
        {
            if (meshFilter == null
                ||
                meshFilterSplineIdx != splineIndex)
            {
                if (TryGetMeshFilter(out var meshFilter))
                    this.meshFilter = meshFilter;
                else
                    this.meshFilter = CreateMeshFilter();
            }

            return meshFilter!;
        }

        private SplineRoadSegment GetRoadSegment(
            SplineSamplerSegment samplerSegment
            )
        {
            var right = math.cross(samplerSegment.Tangent, samplerSegment.UpVector);
            right = math.normalize(right);

            var left = -right;

            float3 leftTopPoint = width * left + samplerSegment.Position;
            float3 leftBottomPoint = (width + borderOffset) * left + samplerSegment.Position;

            leftBottomPoint.y -= depth;

            float3 rightTopPoint = width * right + samplerSegment.Position;
            float3 rightBottomPoint = (width + borderOffset) * right + samplerSegment.Position;

            rightBottomPoint.y -= depth;

            return new SplineRoadSegment(
                leftTopPoint,
                leftBottomPoint,
                rightTopPoint,
                rightBottomPoint
                );
        }

        private void CalucalateVertices()
        {

        }

        private Mesh BuildMesh()
        {
            var vertexCount = RoadSegments.Count * SplineRoadSegment.COUNT;

            using var vertices = ListPool<Vector3>.Shared.Get();
            vertices.Value.TryIncreaseCapacity(vertexCount);

            using var uvVertices = ListPool<Vector3>.Shared.Get();
            uvVertices.Value.TryIncreaseCapacity((int)(vertexCount * 1.5f));

            SplineRoadSegment roadSegment;

            for (int i = 0; i < RoadSegments.Count; i++)
            {
                roadSegment = RoadSegments[i];

                for (int j = 0; j < roadSegment.Count; j++)
                    vertices.Value.Add(roadSegment[j]);
            }

            var mesh = new Mesh();

            mesh.SetVertices(vertices.Value, 0, vertices.Value.Count);

            return mesh;
        }
    }
}
#endif
