using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Leaderboards;
using ObservableCollections;
using R3;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Leaderboards
{
    public class LeaderboardEntryViewModel
        :
        ViewModel<ILeaderboardEntry>
    {
        public ISynchronizedView<KeyValuePair<string, float>, string> ScoreRecords { get; }

        public ReadOnlyReactiveProperty<string> Score { get; }

        public ReadOnlyReactiveProperty<Sprite?> ProfileIcon { get; }

        public ReadOnlyReactiveProperty<string> Position { get; }

        public string ProfileName => model.Profile.Name;

        public LeaderboardEntryViewModel(
            ILeaderboardEntry model,
            CancellationToken cancellationToken
            )
            :
            base(model, cancellationToken)
        {
            ScoreRecords = model.ScoreRecords.CreateView(
                static item => item.Value.ToString())
                .AddTo(disposables);

            Score = model.ObserveScore()
                .Select(score => score.ToString())
                .ToReadOnlyReactiveProperty(model.Score.ToString())
                .AddTo(disposables);

            ProfileIcon = model.Profile.ObserveIcon()
                .Select(
                static sprite =>
                {
                    return sprite.Maybe().GetValue(static () => UCC.AnonymousProfileImage);
                })
                .ToReadOnlyReactiveProperty(model.Profile.Icon.Maybe().GetValue(static () => UCC.AnonymousProfileImage))
                .AddTo(disposables)!;

            Position = model.ObservePosition()
                .Select(
                static pos =>
                {
                    return pos.GetValueOrDefault().ToString();
                })
                .ToReadOnlyReactiveProperty(model.Position.GetValueOrDefault().ToString())
                .AddTo(disposables);
        }
    }
}
