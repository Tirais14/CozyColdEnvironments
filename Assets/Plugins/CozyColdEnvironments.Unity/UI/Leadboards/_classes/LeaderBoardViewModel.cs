using CCEnvs.Unity.Leaderboards;
using CCEnvs.Unity.UI;
using CCEnvs.Unity.UI.Leaderboards;
using ObservableCollections;

namespace CCEnvs.Unity
{
    public class LeaderBoardViewModel<TModel>
        :
        ViewModel<TModel>,
        ILeaderboardViewModel

        where TModel : ILeaderboard
    {
        private readonly ObservableDictionary<Identifier, ILeaderboardEntry> profiles;

        public IReadOnlyObservableDictionary<Identifier, ILeaderboardEntry> Entries => profiles;

        public LeaderBoardViewModel(TModel model) : base(model)
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

    public class LeaderBoardViewModel : LeaderBoardViewModel<Leaderboard>
    {
        public LeaderBoardViewModel(Leaderboard model) : base(model)
        {
        }
    }
}
