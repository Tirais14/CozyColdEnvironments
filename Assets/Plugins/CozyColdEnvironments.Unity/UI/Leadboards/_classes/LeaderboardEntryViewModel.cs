using CCEnvs.Unity.Leaderboards;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable enable
namespace CCEnvs.Unity.UI.Leaderboards
{
    public class LeaderboardEntryViewModel
        :
        ViewModel<ILeaderboardEntry>
    {
        public ISynchronizedView<KeyValuePair<string, ReactiveProperty<float>>, ReadOnlyReactiveProperty<string>> Values { get; }

        public ReadOnlyReactiveProperty<float> Score { get; }

        public LeaderboardEntryViewModel(
            ILeaderboardEntry model,
            CancellationToken cancellationToken
            ) 
            : 
            base(model, cancellationToken)
        {
            Values = model.ScoreValues.CreateView(
                item =>
                {
                    return item.Value.Select(static score => score.ToString())!
                        .ToReadOnlyReactiveProperty(item.Value.Value.ToString())
                        .AddTo(disposables);
                })!;

            Score = model.ObserveScore().ToReadOnlyReactiveProperty();
        }
    }
}
