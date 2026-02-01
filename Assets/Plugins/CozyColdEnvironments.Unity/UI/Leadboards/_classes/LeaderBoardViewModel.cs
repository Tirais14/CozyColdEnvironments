using CCEnvs.Unity.Leaderboards;
using ObservableCollections;

namespace CCEnvs.Unity.UI.Leaderboards
{
    public class LeaderboardViewModel
        :
        ViewModel<Leaderboard>,
        ILeaderboardViewModel
    {
        private readonly ObservableDictionary<Identifier, ILeaderboardEntry> profiles;

        public IReadOnlyObservableDictionary<Identifier, ILeaderboardEntry> Entries => profiles;

        public LeaderboardViewModel(Leaderboard model) : base(model)
        {
            profiles = model.Entries;
        }

        public void Add(ILeaderboardEntry userProfile)
        {
            profiles.Add(userProfile.Profile.ID, userProfile);
        }

        public bool Remove(Identifier userProfileID)
        {
            return profiles.Remove(userProfileID);
        }

        public void Clear()
        {
            profiles.Clear();
        }
    }
}
