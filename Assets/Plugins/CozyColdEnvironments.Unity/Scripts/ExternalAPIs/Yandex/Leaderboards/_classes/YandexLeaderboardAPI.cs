#if PLUGIN_YG_2 && PLATFORM_WEBGL
using CCEnvs.Unity.Leaderboards;
using Humanizer;
using R3;
using System;
using System.Threading;
using UnityEngine;
using YG;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    public sealed class YandexLeaderboardAPI : ILeaderboardAPI
    {
        private readonly ILeaderboard lboard;

        private readonly CancellationTokenSource disposeCancellationTokenSource;

        private readonly string lboardname;

        public YandexLeaderboardAPI(ILeaderboard lboard, string lboardname)
        {
            CC.Guard.IsNotNull(lboard, nameof(lboard));

            this.lboard = lboard;
            this.lboardname = lboardname;

            disposeCancellationTokenSource = new CancellationTokenSource();

            BindLeaderboardSpecialProfile();
        }

        private void BindLeaderboardSpecialProfile()
        {
            CC.Guard.IsNotNull(lboard.SpecialEntry, nameof(lboard.SpecialEntry));

            lboard.SpecialEntry.ObserveScore()
                .Delay(1.1.Seconds())
                .Subscribe(this,
                static (score, @this) =>
                {
                    YG2.SetLeaderboard(@this.lboardname, Mathf.RoundToInt(score));
                })
                .RegisterTo(disposeCancellationTokenSource.Token);
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            if (lboard.SpecialEntry.IsNull())
                this.PrintError($"{nameof(lboard.SpecialEntry)} of leaderboard is null");
            else
                YG2.SetLeaderboard(lboardname, Mathf.RoundToInt(lboard.SpecialEntry.Score));

            disposeCancellationTokenSource.Cancel();
            disposeCancellationTokenSource.Dispose();

            disposed = true;
        }
    }
}
#endif
