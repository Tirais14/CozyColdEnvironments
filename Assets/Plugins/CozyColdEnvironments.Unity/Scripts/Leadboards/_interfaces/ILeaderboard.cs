#nullable enable
using System;
using System.Collections.Generic;
using CCEnvs.Unity.Profiles;
using ObservableCollections;
using R3;

namespace CCEnvs.Unity.Leaderboards
{
    public interface ILeaderboard : ICollection<ILeaderboardEntry>, IDisposable
    {
        IReadOnlyObservableDictionary<Identifier, ILeaderboardEntry> Entries { get; }

        IReadOnlyObservableList<ILeaderboardEntry> SortedEntries { get; }

        IReadOnlyObservableDictionary<Identifier, int> EntryPositions { get; }

        /// <summary>
        /// In general, it is a property that reflects the player's profile.
        /// </summary>
        IUserProfile? SpecialProfile { get; set; }

        ILeaderboardEntry? SpecialEntry { get; }

        IComparer<ILeaderboardEntry> Comparer { get; set; }

        bool TryGetScore(Identifier userProfileID, out float score);

        new IDisposable Add(ILeaderboardEntry entry);

        Observable<IUserProfile?> ObserveSpecialProfile();

        Observable<ILeaderboardEntry?> ObserveSpecialEntry();

        void ICollection<ILeaderboardEntry>.Add(ILeaderboardEntry item)
        {
            _ = Add(item);
        }
    }
}
