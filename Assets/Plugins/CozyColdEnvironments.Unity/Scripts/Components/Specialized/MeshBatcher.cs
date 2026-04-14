using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Threading;
using CCEnvs.Unity.Async;
using Cysharp.Threading.Tasks;
using R3;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Components.Specialized
{
    public sealed class MeshBatcher : CCBehaviour
    {
        private readonly static CommandScheduler commandScheduler = new(UnityFrameProvider.Update, nameof(MeshBatcher));

        private readonly Dictionary<MeshFilter, List<MeshFilter>> topologyDuplicatesMeshFilters = new();

        [SerializeField]
        private List<MeshFilter> compareMeshes = new();

        [SerializeField]
        private bool clearMeshFilterInRuntime = true;

        [SerializeField]
        private bool destroyAfterStart = true;

        [SerializeField, HideInInspector]
        private List<MeshFilter> instantiatedMeshFilters = new();

        [SerializeField, HideInInspector]
        private List<MeshFilter> disabledMeshFilters = new();

        private bool isRestoring;

        public IList<MeshFilter> CompareMeshes => compareMeshes;

        protected override void Start()
        {
            base.Start();

            if (Application.isPlaying)
            {
                if (clearMeshFilterInRuntime)
                    ClearMeshFiltersAsync().ForgetByPrintException();

                if (destroyAfterStart)
                    DestroySelf();
            }
        }

        internal void BatchMeshFiltersCore()
        {
            RestoreDisabledMeshFilters();
            FindTopologyMeshFilterDuplcates();
            ProcessMeshFilters();
        }

        public void BatchMeshFilters()
        {
            var cmdName = NameFactory.CreateFromCaller(this, nameof(BatchMeshFilters));

            Command.Builder.WithName(cmdName)
                .AsSingle()
                .WithState(this)
                .Synchronously()
                .WithExecuteAction(static @this => @this.BatchMeshFiltersCore())
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        internal void RestoreMeshFiltersCore()
        {
            DestroyInsatntiatedMeshFilters();
            RestoreDisabledMeshFilters();
        }

        public void RestoreMeshFilters()
        {
            var cmdName = NameFactory.CreateFromCaller(this, nameof(RestoreMeshFilters));

            Command.Builder.WithName(cmdName)
                .AsSingle()
                .WithState(this)
                .Synchronously()
                .WithExecuteAction(
                static (@this) => @this.RestoreMeshFiltersCore())
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        public IReadOnlyList<MeshFilter> GetOriginalMeshFilters()
        {
            return this.Q()
                .FromChildrens()
                .IncludeInactive()
                .Components<MeshFilter>()
                .Except(instantiatedMeshFilters)
                .ToArray();
        }

        internal void ClearMeshFiltersCore()
        {
            DestroyDisabledMeshFilters();
        }

        public void ClearMeshFilters()
        {
            var cmdName = NameFactory.CreateFromCaller(this, nameof(ClearMeshFilters));

            Command.Builder.WithName(cmdName)
                .AsSingle()
                .WithState(this)
                .Synchronously()
                .WithExecuteAction(static @this => @this.ClearMeshFiltersCore())
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        internal async UniTask ClearMeshFiltersAsyncCore(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await DestroyInsatntiatedMeshFiltersAsync(cancellationToken);
        }

        public async UniTask ClearMeshFiltersAsync(CancellationToken cancellationToken = default)
        {
            var cmdName = NameFactory.CreateFromCaller(this, nameof(ClearMeshFilters));

            using var _ = destroyCancellationToken.TryLinkTokens(
                cancellationToken,
                out cancellationToken
                );

            await Command.Builder.WithName(cmdName)
                .AsSingle()
                .WithState(this)
                .Asynchronously()
                .WithExecuteAction(
                static async (@this, cancellationToken) =>
                {
                    await @this.ClearMeshFiltersAsyncCore(cancellationToken);
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(commandScheduler)
                .WaitForDone();
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

                    replacePos = duplicateMeshFilter.transform.position;
                    replaceRot = duplicateMeshFilter.transform.rotation;
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

        private async UniTask DestroyInsatntiatedMeshFiltersAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int destroyOperationCountInFrame = 0;

            for (int i = instantiatedMeshFilters.Count - 1; i >= 0; i--)
            {
                var meshFilter = instantiatedMeshFilters[i];

                if (meshFilter == null)
                    continue;

                if (Application.isEditor && !Application.isPlaying)
                    DestroyImmediate(meshFilter.gameObject);
                else
                    Destroy(meshFilter.gameObject);

                destroyOperationCountInFrame++;

                if (destroyOperationCountInFrame >= 16)
                {
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                    destroyOperationCountInFrame = 0;
                }
            }
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

            var meshFilters = GetOriginalMeshFilters();

            for (int i = 0; i < compareMeshes.Count; i++)
            {
                leftMeshFilter = compareMeshes[i];

                var topologyDuplicates = topologyDuplicatesMeshFilters.GetOrCreateNew(leftMeshFilter);
                topologyDuplicates.Clear();

                for (int j = 0; j < meshFilters.Count; j++)
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

        private void DestroySelf()
        {
            var cmdName = NameFactory.CreateFromCaller(this, "Destroy");

            Command.Builder.WithName(cmdName)
                .AsSingle()
                .WithState(this)
                .Synchronously()
                .WithExecuteAction(static @this => Destroy(@this))
                .Build()
                .ScheduleBy(commandScheduler);
        }
    }
}
