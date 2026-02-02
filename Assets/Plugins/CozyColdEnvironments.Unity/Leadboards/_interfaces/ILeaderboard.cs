#nullable enable
using ObservableCollections;
using System;
using System.Collections.Generic;

namespace CCEnvs.Unity.Leaderboards
{
    public interface ILeaderboard : ICollection<ILeaderboardEntry>, IDisposable
    {
        IReadOnlyObservableDictionary<Identifier, ILeaderboardEntry> Entries { get; }

        bool TryGetScore(Identifier userProfileID, out float score);
    }
}
