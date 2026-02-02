using CCEnvs.Unity.Leaderboards;
using ObservableCollections;

namespace CCEnvs.Unity.UI.Leaderboards
{
    public class LeaderboardViewModel
        :
        ViewModel<ILeaderboard>
    {
        public IReadOnlyObservableDictionary<Identifier, ILeaderboardEntry> Entries { get; }

        public LeaderboardViewModel(ILeaderboard model) : base(model)
        {
            Entries = model.Entries;
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
