using CCEnvs.Attributes;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Leaderboards;
using System.Linq;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    public sealed class YandexLeaderboardAPI : CCBehaviourStatic<YandexLeaderboardAPI>
    {
        public ILeaderboard Leaderboard { get; }

        [OnInstallResetable]
        public static YandexLeaderboardAPI? Instance { get; private set; }

        public YandexLeaderboardAPI(ILeaderboard leaderboard)
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexLeaderboardAPI));

            CC.Guard.IsNotNull(leaderboard, nameof(leaderboard));

            this.Leaderboard = leaderboard;

            Instance = this;
        }

        private void BindLeaderboard()
        {
            var userProfile = 

            foreach (var entry in Leaderboard.Entries.Select(entry => entry.Value))
            {
                entry.ObserveScore()
            }
        }
    }
}
