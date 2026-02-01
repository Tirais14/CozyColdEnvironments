#nullable enable
using ObservableCollections;
using System;

namespace CCEnvs.Unity.Leaderboards
{
    public interface ILeaderboard : IDisposable
    {
        ObservableDictionary<Identifier, ILeaderboardEntry> Profiles { get; }

        long GetScore(Identifier userProfileID);
    }
}
