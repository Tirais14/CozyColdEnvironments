using CCEnvs.Collections;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Threading;
using CCEnvs.Unity.Leaderboards;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Leaderboards
{
    public class LeaderboardViewModel
        :
        ViewModelBehaviour<ILeaderboard>
    {
        protected readonly Dictionary<ILeaderboardEntry, LeaderboardEntryView> entryViews = new();

        protected readonly List<ILeaderboardEntry> newEntries = new();

        protected readonly CommandScheduler commandScheduler = new(UnityFrameProvider.Update);

        [SerializeField]
        [Tooltip("Must contain LeaderboardEntryView")]
        protected GameObject entryPrefab;

        [SerializeField]
        [Tooltip("Must contain LeaderboardEntryView")]
        protected GameObject specialEntryPrefab;

        [SerializeField]
        protected Transform entryViewsRoot;

        [SerializeField]
        protected bool sortingEnabled = true;

        [SerializeField]
        protected int maxVisibleCount = 24;

        protected Transform sortingBuffer { get; private set; } = null!;

        public GameObject EntryPrefab {
            get => entryPrefab;
            set => entryPrefab = value;
        }

        public Transform EntryViewsRoot {
            get => entryViewsRoot;
            set => entryViewsRoot = value;
        }

        public bool SortingEnabled {
            get => sortingEnabled;
            set => sortingEnabled = value;
        }

        public int MaxVisibleCount {
            get => maxVisibleCount;
            set => maxVisibleCount = value;
        }

        public IReadOnlyObservableDictionary<Identifier, ILeaderboardEntry> Entries => model.Entries;

        public IReadOnlyObservableList<ILeaderboardEntry> SortedEntries => model.SortedEntries;

        public ReadOnlyReactiveProperty<ILeaderboardEntry?> SpecialEntry { get; set; } = null!;

        protected override void Awake()
        {
            base.Awake();
            CreateSortingBuffer();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            commandScheduler.Dispose();
        }

        protected override void Init()
        {
            base.Init();
            SetSpecialEntry();
        }

        public virtual void SortEntries()
        {
            Command.Builder.SetName(nameof(SortEntries), this)
               .SetSingle()
               .WithState(this)
               .Asyncronously()
               .SetExecuteAction(
               static async (@this, cancellationToken) =>
               {
                   await UniTask.WaitForEndOfFrame(cancellationToken: cancellationToken);

                   if (!@this.sortingEnabled)
                   {
                       @this.TrimEntryViews(cancellationToken);
                       return;
                   }

                   @this.MoveEntryViewsToSortingBuffer(cancellationToken);

                   @this.ReorderEntryViews(cancellationToken);

                   @this.sortingBuffer.gameObject.SetActive(false);
               })
               .BuildPooled()
               .Value
               .AttachExternalCancellationToken(destroyCancellationToken)
               .ScheduleBy(commandScheduler);
        }

        private async UniTask InstantiateNewEntriesAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (newEntries.IsEmpty())
                return;

            CC.Guard.IsNotNull(entryViewsRoot, nameof(entryViewsRoot));

            await UniTask.WaitForEndOfFrame(cancellationToken: cancellationToken);

            ILeaderboardEntry entry;

            GameObject entryViewGO;

            GameObject[] instantiatedGOs = await InstantiateAsync(
                    entryPrefab,
                    newEntries.Count,
                    new InstantiateParameters
                    {
                        parent = entryViewsRoot
                    },
                    cancellationToken: cancellationToken
                    );

            var processegGOs = HashSetPool<GameObject>.Shared.Get();

            int newEntriesCount = Math.Min(newEntries.Count, instantiatedGOs.Length);

            try
            {
                for (int i = 0; i < newEntriesCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequestedByInterval(i);

                    entry = newEntries[i];

                    if (!Entries.ContainsKey(entry.Profile.ID))
                    {
                        this.PrintWarning($"The entry: {entry} doesn't added, because it ID is not contains in the {nameof(Leaderboard.Entries)}");
                        continue;
                    }

                    entryViewGO = instantiatedGOs[i];

                    var entryView = GetEntryView(entryViewGO);

                    SetupEntryView(entryView, entry);

                    entryViews.Add(entry, entryView);

                    processegGOs.Value.Add(entryViewGO);
                }
            }
            finally
            {
                int instantiatedGOsCount = instantiatedGOs.Length;

                if (processegGOs.Value.Count != instantiatedGOsCount)
                {
                    for (int i = 0; i < instantiatedGOsCount; i++)
                    {
                        entryViewGO = instantiatedGOs[i];

                        if (!processegGOs.Value.Contains(entryViewGO))
                            Destroy(entryViewGO);
                    }
                }

                newEntries.Clear();

                SortEntries();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static LeaderboardEntryView GetEntryView(GameObject go)
            {
                return go.Q()
                    .IncludeInactive()
                    .Component<LeaderboardEntryView>()
                    .Strict();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void SetupEntryView(LeaderboardEntryView entryView, ILeaderboardEntry entry)
            {
                var entryViewModel = new LeaderboardEntryViewModel((LeaderboardEntry)entry, entryView.destroyCancellationToken);

                entryView.SetViewModel(entryViewModel);
            }
        }

        public void OnEntryAdd(ILeaderboardEntry entry)
        {
            this.PrintLog($"Adding {nameof(entry)}: {entry} to view");

            newEntries.Add(entry);

            Command.Builder.SetName(nameof(OnEntryAdd), this)
                .SetSingle()
                .WithState((@this: this, entry))
                .Asyncronously()
                .SetExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.InstantiateNewEntriesAsync(cancellationToken);
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        public void OnEntryRemove(ILeaderboardEntry entry)
        {
            this.PrintLog($"Removing {nameof(entry)}: {entry} from view");

            newEntries.Remove(entry);

            if (entryViews.Remove(entry, out var entryTransform))
                Destroy(entryTransform.gameObject);
        }

        public void OnEntriesClear()
        {
            foreach (var child in entryViews.Values)
                Destroy(child.gameObject);

            newEntries.Clear();
            entryViews.Clear();
        }

        private void CreateSortingBuffer()
        {
            var sortingBufferGO = new GameObject(nameof(sortingBuffer));

            sortingBufferGO.transform.SetParent(cTransform);

            sortingBuffer = sortingBufferGO.transform;
        }

        private void SetSpecialEntry()
        {
            SpecialEntry = model.ObserveSpecialEntry()
                .ToReadOnlyReactiveProperty(model.SpecialEntry);

            SpecialEntry.AddTo(modelDisposables);
        }

        private void MoveEntryViewsToSortingBuffer(
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (entryViews.IsEmpty())
                return;

            sortingBuffer.gameObject.SetActive(true);

            int i = 0;

            foreach (var entryView in entryViews.Values)
            {
                cancellationToken.ThrowIfCancellationRequestedByIntervalAndMoveNext(ref i);

                entryView.cTransform.SetParent(sortingBuffer);
            }
        }

        private void ReorderEntryViews(
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (entryViews.IsEmpty())
                return;

            LeaderboardEntryView entryView;

            ILeaderboardEntry entry;

            int reorderCount = Math.Min(
                SortedEntries.Count,
                maxVisibleCount
                );

            bool hasSpecialEntry = SpecialEntry.CurrentValue.IsNotNull();
            bool specialEntryProcessed = false;

            bool isEntryCountGreaterThanVisibleCount =
                    hasSpecialEntry
                    &&
                    reorderCount > SortedEntries.Count;

            for (int i = 0; i < reorderCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequestedByInterval(i);

                entry = SortedEntries[i];

                if (hasSpecialEntry
                    &&
                    isEntryCountGreaterThanVisibleCount
                    &&
                    i == reorderCount - 1
                    &&
                    !specialEntryProcessed)
                {
                    entry = SpecialEntry.CurrentValue!;
                }

                if (hasSpecialEntry
                    &&
                    SpecialEntry == entry)
                {
                    specialEntryProcessed = true;
                }

                if (entryViews.TryGetValue(entry, out entryView))
                {
                    entryView.cTransform.SetParent(entryViewsRoot);
                    entryView.Show();
                }
            }

            for (int i = reorderCount; i < SortedEntries.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequestedByInterval(i);

                entry = SortedEntries[i];

                if (entryViews.TryGetValue(entry, out entryView))
                {
                    entryView.cTransform.SetParent(entryViewsRoot);
                    entryView.Hide();
                }
            }
        }

        private void TrimEntryViews(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (entryViews.IsEmpty())
                return;

            int entryViewsCount = entryViews.Count;
            int keepCount = Math.Min(entryViewsCount, maxVisibleCount);

            var entryViewsEnumerator = entryViews.Values.GetEnumerator();

            LeaderboardEntryView entryView;

            for (int i = 0; i < keepCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequestedByInterval(i);

                if (!entryViewsEnumerator.TryMoveNextStruct(out entryView))
                    break;

                entryView.Show();
            }

            for (int i = keepCount; i < entryViewsCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequestedByInterval(i);

                if (!entryViewsEnumerator.TryMoveNextStruct(out entryView))
                    break;

                entryView.Hide();
            }
        }
    }
}
