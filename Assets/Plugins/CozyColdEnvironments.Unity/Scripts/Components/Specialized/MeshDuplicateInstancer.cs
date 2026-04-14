using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Pools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Components.Specialized
{
    [ExecuteAlways]
    public sealed class MeshDuplicateInstancer : CCBehaviour
    {
        private readonly Dictionary<MeshFilter, List<MeshFilter>> topologyDuplicatesMeshFilters = new();

        [SerializeField]
        private MeshFilter[] compareMeshes = new arr<MeshFilter>();

        private MeshFilter[] meshFilters = new arr<MeshFilter>();

        [SerializeField, HideInInspector]
        private List<MeshFilter> instantiatedMeshFilters = new();

        [SerializeField, HideInInspector]
        private List<MeshFilter> disabledMeshFilters = new();

        protected override void Start()
        {
            base.Start();

            if (Application.isPlaying)
            {
                DestroyDisabledMeshFilters();
                Destroy(this);
            }
        }

        public void InstantiateDuplicateMeshes()
        {
            ClearInsatntiatedMeshFilters();
            RestoreDisabledMeshFilters();
            RefreshMeshFilters();
            ProcessMeshFilters();
        }

#if UNITY_EDITOR
        public void Restore()
        {

        }
#endif
        private void RefreshMeshFilters()
        {
            meshFilters = this.Q()
                .FromChildrens()
                .IncludeInactive()
                .Components<MeshFilter>()
                .Except(instantiatedMeshFilters)
                .ToArray();

            FindTopologyMeshFilterDuplcates();
        }

        private void ProcessMeshFilters()
        {
            foreach (var topologyDuplicateMeshFilterItem in topologyDuplicatesMeshFilters)
            {
                var duplcateMeshFilters = topologyDuplicateMeshFilterItem.Value;

                MeshFilter originalMeshFilter = topologyDuplicateMeshFilterItem.Key;
                MeshFilter duplicateMeshFilter;
                MeshFilter instantiatedMeshFilter;

                Vector3 replacePos;
                Transform? replaceParent;

                for (int i = 0; i < duplcateMeshFilters.Count; i++)
                {
                    duplicateMeshFilter = duplcateMeshFilters[i];

                    replacePos = duplicateMeshFilter.transform.position;
                    replaceParent = duplicateMeshFilter.transform.parent;

                    duplicateMeshFilter.gameObject.SetActive(false);
                    disabledMeshFilters.Add(duplicateMeshFilter);

                    instantiatedMeshFilter = Instantiate(originalMeshFilter, replaceParent);
                    instantiatedMeshFilter.transform.position = replacePos;
                    instantiatedMeshFilters.Add(instantiatedMeshFilter);
                }
            }
        }

        private void ClearInsatntiatedMeshFilters()
        {
            for (int i = 0; i < instantiatedMeshFilters.Count; i++)
            {
                var meshFilter = instantiatedMeshFilters[i];

                if (meshFilter == null)
                    continue;

                if (Application.isEditor && !Application.isPlaying)
                    DestroyImmediate(meshFilter.gameObject);
                else
                    Destroy(meshFilter.gameObject);
            }

            instantiatedMeshFilters.Clear();
        }

        private void RestoreDisabledMeshFilters()
        {
            for (int i = 0; i < disabledMeshFilters.Count; i++)
            {
                var meshFilter = disabledMeshFilters[i];

                if (meshFilter == null)
                    continue;

                meshFilter.gameObject.SetActive(true);
            }

            disabledMeshFilters.Clear();
        }

        private void DestroyDisabledMeshFilters()
        {
            for (int i = 0; i < disabledMeshFilters.Count; i++)
                Destroy(disabledMeshFilters[i].gameObject);

            disabledMeshFilters.Clear();
        }

        private void FindTopologyMeshFilterDuplcates()
        {
            topologyDuplicatesMeshFilters.Clear();

            MeshFilter leftMeshFilter;
            MeshFilter rightMeshFilter;

            using var meshDistances = ListPool<float>.Shared.Get();

            for (int i = 0; i < compareMeshes.Length; i++)
            {
                leftMeshFilter = compareMeshes[i];

                var topologyDuplicates = topologyDuplicatesMeshFilters.GetOrCreateNew(leftMeshFilter);
                topologyDuplicates.Clear();

                for (int j = 0; j < meshFilters.Length; j++)
                {
                    rightMeshFilter = meshFilters[j];

                    if (leftMeshFilter.sharedMesh == rightMeshFilter.sharedMesh)
                        continue;

                    if (leftMeshFilter.sharedMesh.EqualsByVertexDistance(rightMeshFilter.sharedMesh))
                        topologyDuplicates.Add(rightMeshFilter);
                }
            }

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog(topologyDuplicatesMeshFilters.SelectMany(x => x.Value).Select(x => x.name).JoinStringsByLine());
        }
    }
}
