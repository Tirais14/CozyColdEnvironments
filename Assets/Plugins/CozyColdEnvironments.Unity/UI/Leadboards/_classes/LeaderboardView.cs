using CCEnvs.Unity.Collections;
using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI.Leaderboards
{
    public abstract class LeaderboardView<TViewModel> : View<TViewModel>
        where TViewModel : ILeaderboardViewModel
    {
        private readonly List<IDisposable> viewModelDisposables = new();

        private readonly ReactiveProperty<int> toInstaniateCount = new();

        [SerializeField]
        private GameObject entryPrefab;

        [SerializeField]
        private ComponentList<ILeaderboardEntry> entries = null!;

        private bool isEntriesInstantiating;

        protected override void Start()
        {
            base.Start();
            BindToInstantiateCount();
            entries.SetDestroyOnRemove(true).SetDestroyByGameObject(true);
        }

        protected override void Init()
        {
            base.Init();

            viewModelDisposables.DisposeEach();
            viewModelDisposables.Clear();

            BindEntryGameObjects();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            toInstaniateCount.Dispose();
        }

        private void BindEntryGameObjects()
        {
            entries.Value.ObserveAdd()
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) =>
                {
                    if (!@this.viewModel.TryGetValue(out var vm))
                        return;

                    vm.Add(entry);
                })
                .AddTo(viewModelDisposables);

            entries.Value.ObserveRemove()
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) =>
                {
                    if (!@this.viewModel.TryGetValue(out var vm))
                        return;

                    vm.Remove(entry.Profile.ID);
                })
                .AddTo(viewModelDisposables);
        }

        private void BindProfilesAdd()
        {
            if (!viewModel.TryGetValue(out var vm))
                return;

            vm.Profiles.ObserveDictionaryAdd()
                .Subscribe(this,
                async (ev, @this) =>
                {
                    if (!viewModel.TryGetValue(out var vm))
                        return;

                    @this.toInstaniateCount.Value++;
                })
                .RegisterDisposableTo(this);

            vm.Profiles.ObserveDictionaryRemove()
                .Subscribe(this,
                async (ev, @this) =>
                {
                    if (!viewModel.TryGetValue(out var vm))
                        return;

                    if (@this.toInstaniateCount.Value < 1)
                    {
                        @this.toInstaniateCount.Value = 0;
                        return;
                    }

                    @this.toInstaniateCount.Value--;
                })
                .RegisterDisposableTo(this);
        }

        private void BindToInstantiateCount()
        {
            toInstaniateCount.Where(this,
                static (_, @this) => !@this.isEntriesInstantiating)
                .Subscribe(this,
                static (_, @this) => @this.InstantiateEntriesAsync().Forget())
                .RegisterDisposableTo(this);
        }

        private async UniTask InstantiateEntriesAsync()
        {
            isEntriesInstantiating = true;

            try
            {
                await UniTask.WaitForEndOfFrame(cancellationToken: destroyCancellationToken);

                var entries = (await InstantiateAsync(
                    entryPrefab,
                    toInstaniateCount.Value,
                    new InstantiateParameters(),
                    cancellationToken: destroyCancellationToken
                    ))
                    .Select(go =>
                    {
                        return go.Q().Component<ILeaderboardEntry>().Strict();
                    });

                this.entries.Value.AddRange(entries);
            }
            finally
            {
                isEntriesInstantiating = false;
            }
        }
    }
}
