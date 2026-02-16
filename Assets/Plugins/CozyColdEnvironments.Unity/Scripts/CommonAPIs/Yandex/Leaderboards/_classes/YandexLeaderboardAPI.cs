#if PLUGIN_YG_2 && PLATFORM_WEBGL
using CCEnvs.Attributes;
using CCEnvs.Dependencies;
using CCEnvs.Unity.Leaderboards;
using Humanizer;
using R3;
using System.Threading;
using UnityEngine;
using YG;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs.Yandex
{
    public sealed class YandexLeaderboardAPI : ILeaderboardAPI
    {
        [field: OnInstallResetable]
        public static YandexLeaderboardAPI? Instance { get; private set; }

        private readonly ILeaderboard lboard;

        private readonly CancellationTokenSource disposeCancellationTokenSource;

        private readonly string lboardname;

        public YandexLeaderboardAPI(ILeaderboard lboard, string lboardname)
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexLeaderboardAPI));

            CC.Guard.IsNotNull(lboard, nameof(lboard));

            this.lboard = lboard;
            this.lboardname = lboardname;

            disposeCancellationTokenSource = new CancellationTokenSource();

            BindLeaderboardSpecialProfile();

            Instance = this;

            BuiltInDependecyContainer.BindTo<ILeaderboardAPI>(this);
            BuiltInDependecyContainer.BindTo(this);
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
