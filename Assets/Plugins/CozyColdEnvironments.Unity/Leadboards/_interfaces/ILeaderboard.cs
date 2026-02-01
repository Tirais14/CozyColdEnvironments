#nullable enable
using ObservableCollections;
using System;

namespace CCEnvs.Unity.Leaderboards
{
    public interface ILeaderboard : IDisposable
    {
        ObservableDictionary<Identifier, ILeaderboardEntry> Entries { get; }

        float GetScore(Identifier userProfileID);
    }
}
