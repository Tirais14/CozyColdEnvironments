#if PLUGIN_YG_2 && PLATFORM_WEBGL
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Leaderboards;
using CCEnvs.Unity.Profiles;
using CCEnvs.Unity.UI.Leaderboards;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using YG;
using YG.Utils.LB;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    [RequireComponent(typeof(LeaderboardYG))]
    public sealed class YandexLeaderboardAdapter : CCBehaviourStatic<YandexLeaderboardAdapter>
    {
#if Leaderboards_yg
        private readonly List<IDisposable> disposables = new();

        [GetBySelf]
        private LeaderboardYG worstLeaderboard = null!;

        private ILeaderboard leaderboard = null!;

        private LeaderboardView leaderboardView = null!;

        protected override void Start()
        {
            base.Start();

            YG2.onGetLeaderboard += OnLeaderboardDataChanged;
            InitAsync().ForgetByPrintException(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            YG2.onGetLeaderboard -= OnLeaderboardDataChanged;
        }

        private void OnLeaderboardDataChanged(LBData data)
        {
            PopulateLeaderboardFromWorst(data.players);
        }

        private async UniTask InitAsync()
        {
            destroyCancellationToken.ThrowIfCancellationRequested();

            leaderboard = GameObjectQuery.Scene.IncludeInactive()
                .Model<ILeaderboard>(includeComponents: false)
                .Strict();

            leaderboardView = GameObjectQuery.Scene.IncludeInactive()
                .Model<LeaderboardView>()
                .Strict();

            BindLeaderboardToWorst();
        }

        private void BindLeaderboardToWorst()
        {
            leaderboardView.ObserveShow()
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.worstLeaderboard.enabled = true;
                })
                .AddDisposableTo(this);

            leaderboardView.ObserveHide()
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.worstLeaderboard.enabled = false;
                })
                .AddDisposableTo(this);
        }

        private IUserProfile CreateUserProfile(LBPlayerData playerData)
        {
            var profile = new UserProfile(playerData.name, playerData.uniqueID);

            YandexPluginHelper.LoadImageAsync(playerData.photo)
                .ContinueWith(
                img =>
                {
                    profile.Icon = img;
                })
                .ForgetByPrintException();

            profile.AddTo(disposables);

            return profile;
        }

        private ILeaderboardEntry CreateLeaderboardEntry(
            LBPlayerData playerData,
            IUserProfile userProfile
            )
        {
            return new LeaderboardEntry(userProfile).AddScoreRecord("score", playerData.score)
                .AddTo(disposables);
        }

        private void PopulateLeaderboardFromWorst(LBPlayerData[] players)
        {
            leaderboard.Clear();
            disposables.DisposeEachAndClear();

            IUserProfile userProfile;

            ILeaderboardEntry leaderboardEntry;

            foreach (var playerData in players)
            {
                userProfile = CreateUserProfile(playerData);

                leaderboardEntry = CreateLeaderboardEntry(playerData, userProfile);

                leaderboard.Add(leaderboardEntry).AddTo(disposables);
            }
        }
#endif
    }
}
#endif //PLUGIN_YG_2