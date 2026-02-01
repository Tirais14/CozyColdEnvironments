using CCEnvs.FuncLanguage;
using CCEnvs.Patterns.Commands;
using CCEnvs.Unity.Collections;
using CCEnvs.Unity.Leaderboards;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI.Leaderboards
{
    public class LeaderboardView : View<LeaderboardViewModel>
    {
        private readonly Dictionary<ILeaderboardEntry, Transform> entries = new();

        private readonly List<ILeaderboardEntry> newEntries = new();

        private readonly CommandScheduler commandScheduler = new(UnityFrameProvider.Update);

        [SerializeField]
        private GameObject entryPrefab;

        [SerializeField]
        private ComponentList entryViews = null!;

        protected override void Awake()
        {
            base.Awake();

            entryViews.SetTypeFilter(typeof(LeaderboardEntryView))
                .SetDestroyOnRemove(true)
                .SetDestroyByGameObject(true);
        }

        protected override void Start()
        {
            base.Start();
            isMutable = true;
        }

        protected override void Init()
        {
            base.Init();
            BindEntryAdd();
            BindEntryRemove();
            BindEntryClear();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            commandScheduler.Dispose();
        }

        protected override Maybe<LeaderboardViewModel> ViewModelFactory()
        {
            return new LeaderboardViewModel(new Leaderboard());
        }

        private async UniTask InstantiateNewEntriesAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await UniTask.WaitForEndOfFrame(cancellationToken: cancellationToken);

            var instantiatedGOs = await InstantiateAsync(
                entryPrefab,
                newEntries.Count,
                new InstantiateParameters
                {
                    parent = entryViews.cTransform.Value
                },
                cancellationToken: cancellationToken
                );

            int i = 0;

            foreach (var (go, entry) in instantiatedGOs.AsValueEnumerable()
                .Zip(newEntries))
            {
                cancellationToken.ThrowIfCancellationRequestedByIntervalAndMoveNext(ref i);

                var entryView = go.Q()
                        .IncludeInactive()
                        .Component<LeaderboardEntryView>()
                        .Strict();

                var entryViewModel = new LeaderboardEntryViewModel((LeaderboardEntry)entry);

                entryView.SetViewModel(entryViewModel);
            }
        }

        private void OnEntryAdd(ILeaderboardEntry entry)
        {
            newEntries.Add(entry);

            Command.Builder.SetName(nameof(OnEntryAdd))
                .SetSingle()
                .NextStep(this)
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
            Command.Builder.SetName(nameof(OnEntryRemove))
                .NextStep((@this: this, entry))
                .Asyncronously()
                .SetExecuteAction(
                static async (args, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (args.@this.entries.Remove(args.entry, out var entryTransform))
                        args.@this.entryViews.Value.Remove(entryTransform);
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        private void OnEntryClear()
        {
            Command.Builder.SetName(nameof(OnEntryClear))
                .NextStep(this)
                .Syncronously()
                .SetExecuteAction(
                static @this =>
                {
                    @this.entries.Clear();
                    @this.entryViews.Value.Clear();
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        private void BindEntryAdd()
        {
            viewModelUnsafe.Entries.ObserveDictionaryAdd()
                .Select(ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) => @this.OnEntryAdd(entry))
                .AddTo(viewModelDisposables);
        }

        private void BindEntryRemove()
        {
            viewModelUnsafe.Entries.ObserveDictionaryRemove()
                .Select(ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) => @this.OnEntryRemove(entry))
                .AddTo(viewModelDisposables);
        }

        private void BindEntryClear()
        {
            viewModelUnsafe.Entries.ObserveClear()
                .Subscribe(this,
                static (_, @this) => @this.OnEntryClear())
                .AddTo(viewModelDisposables);
        }
    }
}
