using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Patterns.Commands;
using CCEnvs.Unity.Collections;
using CCEnvs.Unity.Leaderboards;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using SuperLinq;
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

        protected override Maybe<LeaderboardViewModel> CreateViewModel()
        {
            return new LeaderboardViewModel(new Leaderboard());
        }

        private async UniTask InstantiateNewEntriesAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (newEntries.IsEmpty())
                return;

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

            if (newEntries.Count > instantiatedGOs.Length)
            {
                instantiatedGOs.ForEach(static go => Destroy(go));
                throw new InvalidOperationException("Instantiated count less than to instantaite count");
            }

            ILeaderboardEntry entry;
            int i = 0;

            while (newEntries.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequestedByInterval(i);

                entry = newEntries[^1];

                newEntries.RemoveAt(newEntries.Count - 1);

                if (!viewModelUnsafe.Entries.ContainsKey(entry.Profile.ID))
                    continue;

                var entryView = instantiatedGOs[i++].Q()
                        .IncludeInactive()
                        .Component<LeaderboardEntryView>()
                        .Strict();

                var entryViewModel = new LeaderboardEntryViewModel((LeaderboardEntry)entry);

                entryView.SetViewModel(entryViewModel);

                entries.Add(entry, entryView.transform);
            }

            int restGOs = i + 1 - instantiatedGOs.Length;

            for (int j = 0; j < restGOs; j++)
                Destroy(instantiatedGOs[j]);
        }

        private void OnEntryAdd(ILeaderboardEntry entry)
        {
            newEntries.Add(entry);

            Command.Builder.SetName(nameof(OnEntryAdd))
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
            Command.Builder.SetName(nameof(OnEntryRemove))
                .WithState((@this: this, entry))
                .Asyncronously()
                .SetExecuteAction(
                static async (args, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    args.@this.newEntries.Remove(args.entry);

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
                .WithState(this)
                .Syncronously()
                .SetExecuteAction(
                static @this =>
                {
                    @this.newEntries.Clear();
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
            viewModelUnsafe.Entries.ObserveDictionaryAdd(destroyCancellationToken)
                .Select(ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) => @this.OnEntryAdd(entry))
                .AddTo(viewModelDisposables);
        }

        private void BindEntryRemove()
        {
            viewModelUnsafe.Entries.ObserveDictionaryRemove(destroyCancellationToken)
                .Select(ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) => @this.OnEntryRemove(entry))
                .AddTo(viewModelDisposables);
        }

        private void BindEntryClear()
        {
            viewModelUnsafe.Entries.ObserveClear(destroyCancellationToken)
                .Subscribe(this,
                static (_, @this) => @this.OnEntryClear())
                .AddTo(viewModelDisposables);
        }
    }
}
