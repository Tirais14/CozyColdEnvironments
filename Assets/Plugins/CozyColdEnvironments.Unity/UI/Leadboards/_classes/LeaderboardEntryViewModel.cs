using CCEnvs.Unity.Leaderboards;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.UI.Leaderboards
{
    public class LeaderboardEntryViewModel
        :
        ViewModel<LeaderboardEntry>,
        ILeaderboardEntryViewModel
    {
        public ISynchronizedView<KeyValuePair<string, ReactiveProperty<float>>, Observable<string>> Values { get; }

        public ReadOnlyReactiveProperty<float> Score { get; }

        public LeaderboardEntryViewModel(LeaderboardEntry model) : base(model)
        {
            Values = model.ScoreValues.CreateView(
                item =>
                {
                    return item.Value.Select(static score => score.ToString());
                });

            Score = model.ObserveScore().ToReadOnlyReactiveProperty();
        }
    }
}
