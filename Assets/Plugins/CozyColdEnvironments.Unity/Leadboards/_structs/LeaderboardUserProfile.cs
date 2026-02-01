using CCEnvs.Unity.Profiles;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Leaderboards
{
    public struct LeaderboardUserProfile
    {
        public IReadOnlyUserProfile Profile { get; }
        public long Score { get; }
    }
}
