using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Collections;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Leaderboards;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI.Leaderboards
{
    public abstract class LeaderboardView<TViewModel> : View<TViewModel>
        where TViewModel : ILeaderboardViewModel
    {
        private readonly List<IDisposable> viewModelDisposables = new();

        [SerializeField]
        private GameObject entryPrefab;

        [SerializeField]
        private ComponentList<ILeaderboardEntry> entries = null!;

        private int instantiationCount;

        protected override void Start()
        {
            base.Start();
            entries.SetDestroyOnRemove(true).SetDestroyByGameObject(true);
        }

        protected override void Init()
        {
            base.Init();

            viewModelDisposables.DisposeEach();
            viewModelDisposables.Clear();

            BindEntries();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            toInstaniateCount.Dispose();
        }

        //private void BindEntries()
        //{
        //    if (viewModel.IsNone)
        //        return;

        //    entries.Value.ObserveAdd(destroyCancellationToken)
        //        .Select(static ev => ev.Value)
        //        .Subscribe(this,
        //        static (entry, @this) =>
        //        {
        //            @this.viewModelUnsafe.Add(entry);
        //        })
        //        .AddTo(viewModelDisposables);

        //    entries.Value.ObserveRemove(destroyCancellationToken)
        //        .Select(static ev => ev.Value)
        //        .Subscribe(this,
        //        static (entry, @this) =>
        //        {
        //            @this.viewModelUnsafe.Remove(entry.Profile.ID);
        //        })
        //        .AddTo(viewModelDisposables);

        //    entries.Value.ObserveClear(destroyCancellationToken)
        //        .Subscribe(this,
        //        static (_, @this) =>
        //        {
        //            @this.viewModelUnsafe.Clear();
        //        })
        //        .AddTo(viewModelDisposables);
        //}

        protected abstract ILeaderboardViewModel CreateEntryViewModel(ILeaderboardEntry entry);

        private async UniTask WaitWhileInstantiationAsync()
        {
            await UniTask.WaitWhile(this,
                static @this => @this.instantiationCount > 0,
                cancellationToken: destroyCancellationToken
                );
        }

        private void BindEntries()
        {
            if (!viewModel.TryGetValue(out var vm))
                return;

            vm.Entries.ObserveDictionaryAdd()
                .Select(ev => ev.Value)
                .Subscribe(this,
                async (entry, @this) =>
                {
                    UniTask.Create((entry, @this),
                        static async (args) =>
                        {
                            args.@this.instantiationCount++;

                            try
                            {
                                (await InstantiateAsync(args.@this.entryPrefab))[0].Q()
                                    .Component<IView>()
                                    .Strict()
                                    .SetViewModelUnsafe(args.@this.CreateEntryViewModel(args.entry)
                                    );
                            }
                            finally
                            {
                                args.@this.instantiationCount--;
                            }
                        })
                        .Forget();
                })
                .AddTo(viewModelDisposables);

            vm.Entries.ObserveDictionaryRemove()
                .Select(ev => ev.Value)
                .Subscribe(this,
                async (entry, @this) =>
                {
                    UniTask.Create((entry, @this),
                        static async (args) =>
                        {
                            await args.@this.WaitWhileInstantiationAsync();

                            args.@this.entries.Value.Remove(args.entry);
                        })
                        .Forget();
                })
                .AddTo(viewModelDisposables);

            vm.Entries.ObserveClear()
                .Subscribe(this,
                static (_, @this) =>
                {
                    UniTask.Create(@this,
                        static async @this =>
                        {
                            await @this.WaitWhileInstantiationAsync();

                            @this.entries.Value.Clear();
                        })
                        .Forget();
                })
                .AddTo(viewModelDisposables);
        }
    }
}
