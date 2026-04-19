using CCEnvs.Attributes;
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Patterns.Commands;
using CCEnvs.Threading;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.EditorSerialization;
using Cysharp.Threading.Tasks;
using R3;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

#if ZLINQ_PLUGIN
using ZLinq;
#endif

#nullable enable
namespace CCEnvs.Unity.Components.Specialized
{
    public sealed class MeshBatcher : CCBehaviour
    {
        private readonly Dictionary<MeshFilter, List<MeshFilter>> topologyDuplicatesMeshFilters = new();

        [SerializeField]
        private SerializedDictionary<MeshFilter, MeshFilter?> compareMeshes = new();

        [SerializeField]
        private bool clearMeshFilterInRuntime = true;

        [SerializeField]
        private bool destroyAfterStart = true;

        [SerializeField, HideInInspector]
        private List<MeshFilter> instantiatedMeshFilters = new();

        [SerializeField, HideInInspector]
        private List<MeshFilter> disabledMeshFilters = new();

        private bool isClearingMeshFilters;

        public Dictionary<MeshFilter, MeshFilter?> CompareMeshes => compareMeshes.Deserialized;

        protected override void Start()
        {
            base.Start();

            if (Application.isPlaying)
            {
                if (clearMeshFilterInRuntime)
                    ClearMeshFiltersAsync().ForgetByPrintException();

                if (destroyAfterStart)
                    DestroySelfAsync().Forget();
            }
        }

        public void BatchMeshFilters()
        {
            RestoreDisabledMeshFilters();
            DestroyInsatntiatedMeshFilters();
            FindTopologyMeshFilterDuplcates();
            ProcessMeshFilters();
        }

        public void RestoreMeshFilters()
        {
            DestroyInsatntiatedMeshFilters();
            RestoreDisabledMeshFilters();
        }

        public IReadOnlyList<MeshFilter> GetOriginalMeshFilters()
        {
            return this.Q()
                .FromChildrens()
                .IncludeInactive()
                .Components<MeshFilter>()
#if ZLINQ_PLUGIN
                .AsValueEnumerable()
#endif
                .Except(instantiatedMeshFilters)
                .ToList();
        }

        public void ClearMeshFilters()
        {
            DestroyDisabledMeshFilters();
        }

        public async UniTask ClearMeshFiltersAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await DestroyDisabledMeshFiltersAsync(cancellationToken);
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
                Quaternion replaceRot;
                Transform? replaceParent;

                for (int i = 0; i < duplcateMeshFilters.Count; i++)
                {
                    duplicateMeshFilter = duplcateMeshFilters[i];

                    duplicateMeshFilter.transform.GetPositionAndRotation(out replacePos, out replaceRot);
                    replaceParent = duplicateMeshFilter.transform.parent;

                    duplicateMeshFilter.gameObject.SetActive(false);
                    disabledMeshFilters.Add(duplicateMeshFilter);

                    instantiatedMeshFilter = Instantiate(originalMeshFilter, replaceParent);
                    instantiatedMeshFilter.transform.SetPositionAndRotation(replacePos, replaceRot);
                    instantiatedMeshFilters.Add(instantiatedMeshFilter);
                }
            }
        }

        private void DestroyInsatntiatedMeshFilters()
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

        private async UniTask DestroyDisabledMeshFiltersAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var disabledMeshFiltersCopy = disabledMeshFilters.EnumerableToArrayPooled(disabledMeshFilters.Count);

            int destroyedInFrame = 0;

            foreach (var meshFilter in disabledMeshFiltersCopy)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Destroy(meshFilter.gameObject);

                disabledMeshFilters.Remove(meshFilter);

                if (++destroyedInFrame >= 16)
                {
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                    destroyedInFrame = 0;
                }
            }
        }

        private void FindTopologyMeshFilterDuplcates()
        {
            topologyDuplicatesMeshFilters.Clear();

            MeshFilter rightMeshFilter;

            var meshFilters = GetOriginalMeshFilters();

            foreach (var leftMeshFilter in compareMeshes.Deserialized)
            {
                var topologyDuplicates = topologyDuplicatesMeshFilters.GetOrCreateNew(leftMeshFilter.Value.IfNull(leftMeshFilter.Key));

                topologyDuplicates.Clear();

                for (int j = 0; j < meshFilters.Count; j++)
                {
                    rightMeshFilter = meshFilters[j];

                    if (leftMeshFilter.Key.sharedMesh == rightMeshFilter.sharedMesh)
                        continue;

                    if (leftMeshFilter.Key.sharedMesh.EqualsByGeometry(rightMeshFilter.sharedMesh))
                        topologyDuplicates.Add(rightMeshFilter);
                }
            }

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog(topologyDuplicatesMeshFilters.SelectMany(x => x.Value).Select(x => x.name).JoinStringsByLine());
        }

        private async UniTaskVoid DestroySelfAsync()
        {
            await UniTask.WaitWhile(
                this,
                @this => @this.isClearingMeshFilters,
                cancellationToken: destroyCancellationToken
                );

            Destroy(this);
        }
    }
}
