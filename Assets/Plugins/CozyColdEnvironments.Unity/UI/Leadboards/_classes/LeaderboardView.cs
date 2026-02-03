using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Unity.Leaderboards;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI.Leaderboards
{
    public class LeaderboardView : View<LeaderboardViewModel>
    {
        private readonly Dictionary<ILeaderboardEntry, LeaderboardEntryView> entryViews = new();

        private readonly List<ILeaderboardEntry> newEntries = new();

        private readonly CommandScheduler commandScheduler = new(UnityFrameProvider.Update);

        [SerializeField]
        protected GameObject entryPrefab;

        [SerializeField]
        protected Transform entryViewsRoot;

        [SerializeField]
        protected bool sortingEnabled = true;

        [SerializeField]
        protected int maxVisibleCount = 24;

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

        protected Transform sortingBuffer { get; private set; } = null!;

        protected override void Awake()
        {
            base.Awake();
            CreateSortingBuffer();
        }

        protected override void Init()
        {
            base.Init();
            BindEntryAdd();
            BindEntryRemove();
            BindEntryClear();
            BindSortedEntries();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            commandScheduler.Dispose();
        }

        protected override Maybe<LeaderboardViewModel> CreateViewModel()
        {
            return new LeaderboardViewModel(new Leaderboard(), destroyCancellationToken);
        }

        protected virtual void SortEntries()
        {
            Command.Builder.SetName(nameof(SortEntries))
                .SetSingle()
                .WithState(this)
                .Asyncronously()
                .SetExecuteAction(
                static async (@this, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await UniTask.WaitForEndOfFrame(cancellationToken: cancellationToken);

                    if (!@this.sortingEnabled)
                    {
                        @this.TrimEntryViews(cancellationToken);
                        return;
                    }

                    @this.MoveEntryViewsToSortingBuffer(cancellationToken);
                    @this.ReorderEntryViews(cancellationToken);
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

            var instantiatedGOs = await InstantiateAsync(
                    entryPrefab,
                    newEntries.Count,
                    new InstantiateParameters
                    {
                        parent = entryViewsRoot
                    },
                    cancellationToken: cancellationToken
                    );

            if (newEntries.Count > instantiatedGOs.Length)
            {
                instantiatedGOs.ForEach(static go => Destroy(go));
                throw new InvalidOperationException("Instantiated count less than to instantaite count");
            }

            ILeaderboardEntry entry;

            int i = 0;
            int usedGOCount = 0;

            var entryViewInitTasks = ListPool<UniTask>.Shared.Get();

            try
            {
                while (newEntries.Count > 0)
                {
                    cancellationToken.ThrowIfCancellationRequestedByInterval(i);

                    entry = GetEntry(newEntries);

                    if (!viewModelUnsafe.Entries.ContainsKey(entry.Profile.ID))
                    {
                        this.PrintWarning($"The entry: {entry} doesn't added, because it ID is not contains in the {nameof(viewModel)}.{nameof(Leaderboard.Entries)}");
                        continue;
                    }

                    var entryView = GetEntryView(instantiatedGOs[i++]);

                    SetupEntryView(entryView, entry);

                    entryViews.Add(entry, entryView);

                    entryViewInitTasks.Value.Add(entryView.WaitForInitializedAsync(cancellationToken));

                    usedGOCount++;
                }

                await UniTask.WhenAll(entryViewInitTasks.Value);
            }
            finally
            {
                entryViewInitTasks.Dispose();

                int restGOs = instantiatedGOs.Length - usedGOCount;

                for (int j = 0; j < restGOs; j++)
                    Destroy(instantiatedGOs[j]);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static ILeaderboardEntry GetEntry(List<ILeaderboardEntry> entries)
            {
                var entry = entries[^1];

                entries.RemoveAt(entries.Count - 1);

                return entry;
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

        private void OnEntryAdd(ILeaderboardEntry entry)
        {
            this.PrintLog($"Adding {nameof(entry)}: {entry} to view");

            newEntries.Add(entry);

            Command.Builder.SetName(nameof(OnEntryAdd), this)
                .SetSingle()
                .WithState(this)
                .Asyncronously()
                .SetExecuteAction(
                static async (@this, cancellationToken) =>
                {
                    await @this.InstantiateNewEntriesAsync(cancellationToken);
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        private void OnEntryRemove(ILeaderboardEntry entry)
        {
            this.PrintLog($"Removing {nameof(entry)}: {entry} from view");

            Command.Builder.SetName(nameof(OnEntryRemove), this)
                .WithState((@this: this, entry))
                .Asyncronously()
                .SetExecuteAction(
                static async (args, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    args.@this.newEntries.Remove(args.entry);

                    if (args.@this.entryViews.Remove(args.entry, out var entryTransform))
                        Destroy(entryTransform.gameObject);
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        private void OnEntriesClear()
        {
            Command.Builder.SetName(nameof(OnEntriesClear), this)
                .WithState(this)
                .Syncronously()
                .SetExecuteAction(
                static @this =>
                {
                    @this.newEntries.Clear();
                    @this.entryViews.Clear();
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        private void BindEntryAdd()
        {
            viewModelUnsafe.Entries.ObserveDictionaryAdd(destroyCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) => @this.OnEntryAdd(entry))
                .AddTo(viewModelDisposables);
        }

        private void BindEntryRemove()
        {
            viewModelUnsafe.Entries.ObserveDictionaryRemove(destroyCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) => @this.OnEntryRemove(entry))
                .AddTo(viewModelDisposables);
        }

        private void BindEntryClear()
        {
            viewModelUnsafe.Entries.ObserveClear(destroyCancellationToken)
                .Subscribe(this,
                static (_, @this) => @this.OnEntriesClear())
                .AddTo(viewModelDisposables);
        }

        private void CreateSortingBuffer()
        {
            var sortingBufferGO = new GameObject(nameof(sortingBuffer));

            sortingBufferGO.transform.SetParent(cTransform);

            sortingBuffer = sortingBufferGO.transform;
        }

        private void BindSortedEntries()
        {
            viewModelUnsafe.SortedEntries.ObserveChanged(viewModelUnsafe.DisposeCancellationToken)
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.SortEntries();
                })
                .AddTo(viewModelDisposables);
        }

        private void MoveEntryViewsToSortingBuffer(
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (entryViews.IsEmpty())
                return;

            int i = 0;

            foreach (var entryView in entryViews.Values)
            {
                cancellationToken.ThrowIfCancellationRequestedByIntervalAndMoveNext(ref i);

                entryView.cTransform.SetParent(cTransform);
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
                viewModelUnsafe.SortedEntries.Count,
                maxVisibleCount
                );

            bool hasSpecialEntry = viewModelUnsafe.SpecialEntry.CurrentValue.IsSome;
            bool specialEntryProcessed = false;

            bool isEntryCountGreaterThanVisibleCount =
                    hasSpecialEntry
                    &&
                    reorderCount > viewModelUnsafe.SortedEntries.Count;

            for (int i = 0; i < reorderCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequestedByInterval(i);

                entry = viewModelUnsafe.SortedEntries[i];

                if (isEntryCountGreaterThanVisibleCount
                    &&
                    i == reorderCount - 1
                    &&
                    !specialEntryProcessed)
                {
                    entry = viewModelUnsafe.SpecialEntry.CurrentValue.GetValueUnsafe();
                }

                if (hasSpecialEntry
                    &&
                    viewModelUnsafe.SpecialEntry == entry)
                {
                    specialEntryProcessed = true;
                }

                if (entryViews.TryGetValue(entry, out entryView))
                {
                    entryView.cTransform.SetParent(entryViewsRoot);
                    entryView.Show();
                }
            }

            for (int i = reorderCount; i < viewModelUnsafe.SortedEntries.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequestedByInterval(i);

                entry = viewModelUnsafe.SortedEntries[i];

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

                if (!entryViewsEnumerator.MoveNextStruct(out entryView))
                    break;

                entryView.Show();
            }

            for (int i = keepCount; i < entryViewsCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequestedByInterval(i);

                if (!entryViewsEnumerator.MoveNextStruct(out entryView))
                    break;

                entryView.Hide();
            }
        }
    }
}
