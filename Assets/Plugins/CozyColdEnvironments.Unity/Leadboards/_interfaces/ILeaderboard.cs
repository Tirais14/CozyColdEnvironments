#nullable enable
using CCEnvs.FuncLanguage;
using ObservableCollections;
using System;

namespace CCEnvs.Unity.Leaderboards
{
    public interface ILeaderboard : IDisposable
    {
        ObservableDictionary<Identifier, ILeaderboardEntry> Entries { get; }

        bool TryGetScore(Identifier userProfileID, out float score);
    }
}
