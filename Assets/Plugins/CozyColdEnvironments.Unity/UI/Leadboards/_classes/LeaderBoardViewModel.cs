using CCEnvs.Unity.Leaderboards;
using CCEnvs.Unity.UI;
using CCEnvs.Unity.UI.Leaderboards;
using ObservableCollections;
using R3;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace CCEnvs.Unity
{
    public class LeaderBoardViewModel<TModel>
        :
        ViewModel<TModel>,
        ILeaderboardViewModel<TModel>

        where TModel : ILeaderboard
    {
        private readonly ObservableDictionary<Identifier, ILeaderboardEntry> profiles;

        public IReadOnlyObservableDictionary<Identifier, ILeaderboardEntry> Profiles => profiles;

        public LeaderBoardViewModel(TModel model) : base(model)
        {
            profiles = model.Profiles;
        }

        public void Add(ILeaderboardEntry userProfile)
        {
            profiles.Add(userProfile.Profile.ID, userProfile);
        }

        public bool Remove(Identifier userProfileID)
        {
            return profiles.Remove(userProfileID);
        }
    }
}
