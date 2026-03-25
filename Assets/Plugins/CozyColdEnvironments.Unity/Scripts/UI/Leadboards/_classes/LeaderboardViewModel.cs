using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using CCEnvs.Attributes;
using CCEnvs.Collections;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Threading;
using CCEnvs.Unity.Leaderboards;
using Cysharp.Threading.Tasks;
using Humanizer;
using ObservableCollections;
using R3;
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

        [SerializeField, OptionalField]
        [Tooltip("Must contain LeaderboardEntryView")]
        protected GameObject specialEntryPrefab;

        [SerializeField, OptionalField]
        protected Showable? loadingScreen;

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

        public Showable? LoadingScreen {
            get => loadingScreen;
            set => loadingScreen = value;
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

        public IReadOnlyObservableDictionary<Identifier, ILeaderboardEntry> Entries => GuardedModel.Entries;

        public IReadOnlyObservableList<ILeaderboardEntry> SortedEntries => GuardedModel.SortedEntries;

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

        protected override void OnSetModel(ILeaderboard? model)
        {
        }

        protected override void InitModel(ILeaderboard model)
        {
            SetSpecialEntry(model);
        }

        public virtual void SortEntries()
        {
            Command.Builder.WithName(nameof(SortEntries), this)
               .AsSingle()
               .WithState(this)
               .Asynchronously()
               .WithExecuteAction(
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

        //FIXME: Separate a method
        private async UniTask InstantiateNewEntriesAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (newEntries.IsEmpty())
                return;

            CC.Guard.IsNotNull(entryViewsRoot, nameof(entryViewsRoot));

            long startTimestamp = TimeProvider.System.GetTimestamp();

            bool loadingScreenShown = false;

            if (loadingScreen.IsNotNull() && entryViewsRoot.childCount == 0)
            {
                loadingScreen.Show();
                loadingScreenShown = true;
            }

            await UniTask.WaitForEndOfFrame(cancellationToken: cancellationToken);

            ILeaderboardEntry entry;

            GameObject entryViewGO;

            var instantiateParameters = new InstantiateParameters
            {
                parent = entryViewsRoot
            };

            GameObject[] instantiatedGOs = await InstantiateAsync(
                    entryPrefab,
                    newEntries.Count,
                    instantiateParameters,
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

                    if (EqualityComparer<ILeaderboardEntry?>.Default.Equals(entry, SpecialEntry.CurrentValue)
                        &&
                        specialEntryPrefab != null)
                    {
                        Destroy(entryViewGO);

                        instantiatedGOs[i] = (await InstantiateAsync(
                            specialEntryPrefab,
                            instantiateParameters,
                            cancellationToken
                            ))[0];

                        entryViewGO = instantiatedGOs[i];
                    }

                    var entryView = getEntryView(entryViewGO);

                    setupEntryView(entryView, entry);

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

                processegGOs.Dispose();

                newEntries.Clear();

                SortEntries();

                if (loadingScreenShown)
                {
                    var timeSinceStart = TimeProvider.System.GetElapsedTime(startTimestamp);

                    if (timeSinceStart < 1.Seconds())
                    {
                        await UniTask.WaitForSeconds(
                            duration: (1.Seconds() - timeSinceStart).TotalSecondsF(),
                            ignoreTimeScale: true,
                            cancellationToken: cancellationToken
                            );
                    }

                    loadingScreen!.Hide();
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static LeaderboardEntryView getEntryView(GameObject go)
            {
                return go.Q()
                    .IncludeInactive()
                    .Component<LeaderboardEntryView>()
                    .Strict();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void setupEntryView(LeaderboardEntryView entryView, ILeaderboardEntry entry)
            {
                var entryViewModel = new LeaderboardEntryViewModel((LeaderboardEntry)entry);

                entryView.SetViewModel(entryViewModel);
            }
        }

        public void OnEntryAdd(ILeaderboardEntry entry)
        {
            this.PrintLog($"Adding {nameof(entry)}: {entry} to view");

            newEntries.Add(entry);

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(OnEntryAdd)
                );

            Command.Builder.WithName(cmdName)
                .AsSingle()
                .WithState((@this: this, entry))
                .Asynchronously()
                .WithExecuteAction(
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

        private void SetSpecialEntry(ILeaderboard model)
        {
            SpecialEntry = model.ObserveSpecialEntry()
                .ToReadOnlyReactiveProperty(model.SpecialEntry);

            SpecialEntry.AddTo(ModelDisposables);
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

            bool isEntryCountGreaterThanVisibleCount = SortedEntries.Count > reorderCount;

            for (int i = 0; i < reorderCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequestedByInterval(i);

                if (hasSpecialEntry
                    &&
                    !specialEntryProcessed
                    &&
                    isEntryCountGreaterThanVisibleCount
                    &&
                    i == reorderCount - 1)
                {
                    entry = SpecialEntry.CurrentValue!;
                }
                else
                    entry = SortedEntries[i];

                if (hasSpecialEntry
                    &&
                    EqualityComparer<ILeaderboardEntry?>.Default.Equals(entry, SpecialEntry.CurrentValue))
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

                if (!entryViews.TryGetValue(entry, out entryView)
                    ||
                    entryView.cTransform.parent == entryViewsRoot)
                {
                    continue;
                }

                entryView.cTransform.SetParent(entryViewsRoot);
                entryView.Hide();
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
