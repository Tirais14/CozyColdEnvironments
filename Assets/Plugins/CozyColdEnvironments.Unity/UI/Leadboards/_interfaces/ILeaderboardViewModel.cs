using CCEnvs.Unity.Leaderboards;
using ObservableCollections;

#nullable enable
namespace CCEnvs.Unity.UI.Leaderboards
{
    public interface ILeaderboardViewModel
        : 
        IViewModel
    {
        IReadOnlyObservableDictionary<Identifier, ILeaderboardEntry> Profiles { get; }

        void Add(ILeaderboardEntry userProfile);

        bool Remove(Identifier userProfileID);
    }
}
