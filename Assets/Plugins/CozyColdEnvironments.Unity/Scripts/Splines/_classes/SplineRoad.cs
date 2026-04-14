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

        [SerializeField]
        protected Material roadMeshMaterial = null!;

        private readonly List<SplineRoadSegment> roadSegments = new();

        [SerializeField, HideInInspector]
        private MeshFilter? roadMeshFilter;

        [SerializeField, HideInInspector]
        private MeshRenderer? roadMeshRenderer;

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

            var mesh = BuildRoadMesh();
            GetOrCreateRoadMeshFilter().sharedMesh = mesh;
        }

        private MeshFilter CreateRoadMeshFilter()
        {
            if (this.roadMeshFilter != null)
                Destroy(this.roadMeshFilter.gameObject);

            var go = new GameObject(
                $"{name}[MeshFilter][{splineIndex}]",
                typeof(MeshFilter),
                typeof(MeshRenderer)
                );

            go.transform.SetParent(cTransform);

            var roadMeshFilter = go.GetComponent<MeshFilter>();

            return roadMeshFilter;
        }

        private bool TryGetRoadMeshFilter([NotNullWhen(true)] out MeshFilter? roadMeshFilter)
        {
            return this.Q()
                .WithName(MeshFilterName, byFullName: true)
                .Component<MeshFilter>()
                .Lax()
                .TryGetValue(out roadMeshFilter);
        }

        private MeshFilter GetOrCreateRoadMeshFilter()
        {
            if (roadMeshFilter == null
                ||
                meshFilterSplineIdx != splineIndex)
            {
                if (TryGetRoadMeshFilter(out var roadMeshFilter))
                    this.roadMeshFilter = roadMeshFilter;
                else
                    this.roadMeshFilter = CreateRoadMeshFilter();

                meshFilterSplineIdx = splineIndex;

                roadMeshRenderer = this.roadMeshFilter.GetComponent<MeshRenderer>();
                roadMeshRenderer.sharedMaterial = roadMeshMaterial;

                this.roadMeshFilter.transform.localPosition = Vector3.zero;
            }

            return roadMeshFilter!;
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

        //private Mesh BuildRoadMesh()
        //{
        //    var vertexCount = RoadSegments.Count * SplineRoadSegment.COUNT;

        //    using var vertices = ListPool<Vector3>.Shared.Get();
        //    vertices.Value.TryIncreaseCapacity(vertexCount);

        //    using var uvVertices = ListPool<Vector3>.Shared.Get();
        //    uvVertices.Value.TryIncreaseCapacity((int)(vertexCount * 1.5f));

        //    SplineRoadSegment roadSegment;

        //    for (int i = 0; i < RoadSegments.Count; i++)
        //    {
        //        roadSegment = RoadSegments[i];

        //        for (int j = 0; j < roadSegment.Count; j++)
        //            vertices.Value.Add(roadSegment[j]);
        //    }

        //    var mesh = new Mesh();

        //    mesh.SetVertices(vertices.Value, 0, vertices.Value.Count);

        //    return mesh;
        //}

        private Mesh BuildRoadMesh()
        {
            int segmentCount = roadSegments.Count;
            if (segmentCount < 2) return new Mesh();

            int totalVerts = segmentCount * SplineRoadSegment.COUNT;
            int totalTris = (segmentCount - 1) * 6; // 3 грани * 2 треугольника

            using var vertices = ListPool<Vector3>.Shared.Get();
            using var uvs = ListPool<Vector2>.Shared.Get();
            using var triangles = ListPool<int>.Shared.Get();

            vertices.Value.TryIncreaseCapacity(totalVerts);
            uvs.Value.TryIncreaseCapacity(totalVerts);
            triangles.Value.TryIncreaseCapacity(totalTris);

            float cumulativeLength = 0f;
            float sideVOffset = depth;

            // 1. Первый сегмент
            var first = roadSegments[0];
            vertices.Value.Add(first[0]); uvs.Value.Add(new Vector2(cumulativeLength, -sideVOffset)); // LeftBottom
            vertices.Value.Add(first[1]); uvs.Value.Add(new Vector2(cumulativeLength, 0f));           // LeftTop
            vertices.Value.Add(first[2]); uvs.Value.Add(new Vector2(cumulativeLength, 1f));           // RightTop
            vertices.Value.Add(first[3]); uvs.Value.Add(new Vector2(cumulativeLength, 1f + sideVOffset)); // RightBottom

            // 2. Остальные сегменты
            for (int i = 1; i < segmentCount; i++)
            {
                var prev = roadSegments[i - 1];
                var curr = roadSegments[i];

                float segLen = math.distance(prev.LeftTopPoint, curr.LeftTopPoint);
                cumulativeLength += segLen;

                int pBase = (i - 1) * 4;
                int cBase = i * 4;

                vertices.Value.Add(curr[0]); uvs.Value.Add(new Vector2(cumulativeLength, -sideVOffset));
                vertices.Value.Add(curr[1]); uvs.Value.Add(new Vector2(cumulativeLength, 0f));
                vertices.Value.Add(curr[2]); uvs.Value.Add(new Vector2(cumulativeLength, 1f));
                vertices.Value.Add(curr[3]); uvs.Value.Add(new Vector2(cumulativeLength, 1f + sideVOffset));

                int p0 = pBase, p1 = pBase + 1, p2 = pBase + 2, p3 = pBase + 3;
                int c0 = cBase, c1 = cBase + 1, c2 = cBase + 2, c3 = cBase + 3;

                // 🔵 Верх (полотно) - инвертирован порядок для outward normals
                triangles.Value.Add(p1); triangles.Value.Add(c2); triangles.Value.Add(c1);
                triangles.Value.Add(p1); triangles.Value.Add(p2); triangles.Value.Add(c2);

                // 🔵 Левая стенка - инвертирован порядок
                triangles.Value.Add(p1); triangles.Value.Add(c0); triangles.Value.Add(p0);
                triangles.Value.Add(p1); triangles.Value.Add(c1); triangles.Value.Add(c0);

                // 🔵 Правая стенка - инвертирован порядок
                triangles.Value.Add(p2); triangles.Value.Add(c3); triangles.Value.Add(c2);
                triangles.Value.Add(p2); triangles.Value.Add(p3); triangles.Value.Add(c3);
            }

            var mesh = new Mesh
            {
                name = $"{name}_RoadMesh_{splineIndex}",
                indexFormat = totalVerts > 65535
                    ? UnityEngine.Rendering.IndexFormat.UInt32
                    : UnityEngine.Rendering.IndexFormat.UInt16
            };

            mesh.SetVertices(vertices.Value);
            mesh.SetUVs(0, uvs.Value);
            mesh.SetTriangles(triangles.Value, 0);

            // Пересчитывает нормали на основе нового winding order
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
#endif
