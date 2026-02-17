#if PLUGIN_YG_2 && PLATFORM_WEBGL
using CCEnvs.Attributes;
using CCEnvs.Dependencies;
using CCEnvs.Unity.Leaderboards;
using Humanizer;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs.Yandex
{
    public sealed class YandexLeaderboardAPI : ILeaderboardAPI
    {
        [field: OnInstallResetable]
        public static YandexLeaderboardAPI? Instance { get; private set; }

        private readonly List<IDisposable> disposables = new();

        private readonly ILeaderboard lboard;

        private readonly string lboardname;

        public YandexLeaderboardAPI(ILeaderboard lboard, string lboardname)
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexLeaderboardAPI));

            CC.Guard.IsNotNull(lboard, nameof(lboard));

            this.lboard = lboard;
            this.lboardname = lboardname;

            BindLeaderboardSpecialProfile();

            Instance = this;

            BuiltInDependecyContainer.Bind<ILeaderboardAPI>(this);
            BuiltInDependecyContainer.Bind(this);
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
                .AddTo(disposables);
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            if (lboard.SpecialEntry.IsNotNull())
                YG2.SetLeaderboard(lboardname, Mathf.RoundToInt(lboard.SpecialEntry.Score));

            disposed = true;
        }
    }
}
#endif
