#nullable enable
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Serialization;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CCEnvs.Unity.UI.Leaderboards
{
    public abstract class LeaderboardEntryView<TViewModel> : View<TViewModel>
        where TViewModel : ILeaderboardEntryViewModel
    {
        [SerializeField]
        protected SerializedDictionary<string, TMP_Text> scoreTextBindings = new();

        private readonly Dictionary<string, IDisposable> scoreBindings = new();

        protected override void Init()
        {
            base.Init();
            BindScoreValueAddRemoveOperations();
            BindTextComponents();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            scoreBindings.Values.DisposeEach();
            scoreBindings.Clear();
        }

        private void BindTextComponent(string key, Observable<string> scoreView)
        {
            if (!viewModel.TryGetValue(out var vm))
                return;

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
            if (!viewModel.TryGetValue(out var vm))
                return;

            foreach (var item in vm.Values.Unfiltered)
            {
                if (scoreBindings.ContainsKey(item.Value.Key))
                    continue;

                BindTextComponent(item.Value.Key, item.View);
            }
        }

        private void BindScoreValueAddRemoveOperations()
        {
            if (!viewModel.TryGetValue(out var vm))
                return;

            vm.Values.ObserveAdd(destroyCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) =>
                {
                    @this.BindTextComponent(item.Value.Key, item.View);
                })
                .RegisterDisposableTo(this);

            vm.Values.ObserveRemove(destroyCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) =>
                {
                    @this.UnbindTextComponent(item.Value.Key);
                })
                .RegisterDisposableTo(this);
        }
    }
}
