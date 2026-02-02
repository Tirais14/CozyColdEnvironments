#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Leaderboards;
using CCEnvs.Unity.Profiles;
using CCEnvs.Unity.Serialization;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CCEnvs.Unity.UI.Leaderboards
{
    public class LeaderboardEntryView : View<LeaderboardEntryViewModel>
    {
        [SerializeField]
        protected SerializedDictionary<string, TMP_Text> scoreTextBindings = new();

        private readonly Dictionary<string, IDisposable> scoreBindings = new();

        protected override void Init()
        {
            base.Init();
            BindScoreValuesAdd();
            BindScoreValuesRemove();
            BindScoreValuesClear();
            BindTextComponents();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            scoreBindings.Values.DisposeEach();
            scoreBindings.Clear();
        }

        protected override Maybe<LeaderboardEntryViewModel> CreateViewModel()
        {
            return new LeaderboardEntryViewModel(LeaderboardEntry.Empty);
        }

        private void BindTextComponent(string key, Observable<string> scoreView)
        {
            if (scoreBindings.ContainsKey(key))
                throw new InvalidOperationException($"Text component with key: {key} already binded");

            var sub = scoreView.Subscribe((@this: this, key),
                static (scoreView, args) =>
                {
                    args.@this.scoreTextBindings.Deserialized[args.key].text = scoreView;
                });

            scoreBindings.Add(key, sub);
        }

        private void UnbindTextComponent(string key)
        {
            if (!scoreBindings.Remove(key, out var sub))
                return;

            sub.Dispose();
        }

        private void BindTextComponents()
        {
            foreach (var item in viewModelUnsafe.Values.Unfiltered)
            {
                if (scoreBindings.ContainsKey(item.Value.Key))
                    continue;

                BindTextComponent(item.Value.Key, item.View);
            }
        }

        private void BindScoreValuesAdd()
        {
            viewModelUnsafe.Values.ObserveAdd(destroyCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) =>
                {
                    @this.BindTextComponent(item.Value.Key, item.View);
                })
                .AddTo(viewModelDisposables);
        }

        private void BindScoreValuesRemove()
        {
            viewModelUnsafe.Values.ObserveRemove(destroyCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) =>
                {
                    @this.UnbindTextComponent(item.Value.Key);
                })
                .AddTo(viewModelDisposables);
        }

        private void BindScoreValuesClear()
        {
            viewModelUnsafe.Values.ObserveClear(destroyCancellationToken)
                .Subscribe(this,
                static (_, @this) =>
                {
                    foreach (var item in @this.scoreBindings)
                        item.Value.Dispose();

                    @this.scoreBindings.Clear();
                })
                .AddTo(viewModelDisposables);
        }
    }
}
