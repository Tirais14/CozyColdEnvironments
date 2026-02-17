#if PLUGIN_YG_2 && Leaderboards_yg && PLATFORM_WEBGL
using CCEnvs.Unity.CommonAPIs.Yandex.Leaderboards;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Leaderboards;
using CCEnvs.Unity.UI.Leaderboards;
using R3;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs.Yandex
{
    [RequireComponent(typeof(LeaderboardView))]
    public sealed class YandexLeaderboardAdapter : CCBehaviourStatic<YandexLeaderboardAdapter>
    {
        public string leaderboardName = "lboard";

        [GetBySelf]
        private LeaderboardView lboardView = null!;

        private ILeaderboard lboard = null!;

        private IDisposable? lboardPopulationDisposable;

        protected override void Start()
        {
            base.Start();

            lboard = lboardView.GetModel<ILeaderboard>();

            lboardView.ObserveShow()
                .SubscribeAwait(
                async (_, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    lboardPopulationDisposable?.Dispose();

                    lboardPopulationDisposable = await YandexLeaderboardHelper.PopulateLeaderboardAsync(
                        lboard,
                        leaderboardName,
                        cancellationToken: destroyCancellationToken
                        );
                })
                .AddDisposableTo(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            lboardPopulationDisposable?.Dispose();
        }
    }
}
#endif //PLUGIN_YG_2 && Leaderboards_yg && PLATFORM_WEBGL