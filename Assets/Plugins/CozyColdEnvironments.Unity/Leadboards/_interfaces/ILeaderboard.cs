#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Profiles;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;

namespace CCEnvs.Unity.Leaderboards
{
    public interface ILeaderboard : ICollection<ILeaderboardEntry>, IDisposable
    {
        IReadOnlyObservableDictionary<Identifier, ILeaderboardEntry> Entries { get; }

        IReadOnlyObservableList<ILeaderboardEntry> SortedEntries { get; }

        /// <summary>
        /// In general, it is a property that reflects the player's profile.
        /// </summary>
        Maybe<IUserProfile> SpecialProfile { get; set; }

        bool TryGetScore(Identifier userProfileID, out float score);

        new IDisposable Add(ILeaderboardEntry entry);

        Observable<Maybe<IUserProfile>> ObserveSpecialProfile();

        void ICollection<ILeaderboardEntry>.Add(ILeaderboardEntry item)
        {
            _ = Add(item);
        }
    }
}
