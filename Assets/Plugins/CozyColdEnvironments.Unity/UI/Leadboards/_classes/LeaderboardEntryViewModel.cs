using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.UI.Leaderboards
{
    public class LeaderboardEntryViewModel<TModel>
        :
        ViewModel<TModel>,
        ILeaderboardEntryViewModel

        where TModel : ILeaderboardEntry
    {
        public ISynchronizedView<KeyValuePair<string, ReadOnlyReactiveProperty<float>>, Observable<string>> Values { get; }

        public ReadOnlyReactiveProperty<float> Score { get; }

        public LeaderboardEntryViewModel(TModel model) : base(model)
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
