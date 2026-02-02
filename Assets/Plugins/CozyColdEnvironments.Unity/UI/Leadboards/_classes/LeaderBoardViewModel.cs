using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Leaderboards;
using ObservableCollections;
using R3;
using System.Threading;

#nullable enable
namespace CCEnvs.Unity.UI.Leaderboards
{
    public class LeaderboardViewModel
        :
        ViewModel<ILeaderboard>
    {
        public IReadOnlyObservableDictionary<Identifier, ILeaderboardEntry> Entries => model.Entries;

        public IReadOnlyObservableList<ILeaderboardEntry> SortedEntries => model.SortedEntries;

        public ReadOnlyReactiveProperty<Maybe<ILeaderboardEntry>> SpecialEntry { get; }

        public LeaderboardViewModel(ILeaderboard model, CancellationToken cancellationToken)
            :
            base(model, cancellationToken)
        {
            SpecialEntry = model.ObserveSpecialProfile()
                .Select(this,
                static (maybeProfile, @this) =>
                {
                    if (!maybeProfile.TryGetValue(out var profile))
                        return Maybe<ILeaderboardEntry>.None;

                    return @this.Entries[profile.ID].Maybe();
                })
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);
        }

        public void Add(ILeaderboardEntry entry)
        {
            model.Add(entry);
        }

        public bool Remove(ILeaderboardEntry entry)
        {
            return model.Remove(entry);
        }

        public void Clear()
        {
            model.Clear();
        }
    }
}
