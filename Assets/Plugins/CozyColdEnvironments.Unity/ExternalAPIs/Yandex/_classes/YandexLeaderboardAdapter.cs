#if Leaderboards_yg
using CCEnvs.Patterns.Commands;
using CCEnvs.Reflection;
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
    public sealed class YandexLeaderboardAdapter : CCBehaviour
    {
        private readonly List<IDisposable> disposables = new();

        private readonly CommandScheduler commandScheduler = new(UnityFrameProvider.Update, nameof(YandexLeaderboardAdapter));

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
            commandScheduler.Dispose();
        }

        private void OnLeaderboardDataChanged(LBData data)
        {
            FillLeaderboardFromWorst(data.players);
        }

        private async UniTask InitAsync()
        {
            destroyCancellationToken.ThrowIfCancellationRequested();

            //worstLeaderboard.RectTransform()
            //    .MoveToDevCanvas();

            //await UniTask.WaitForSeconds(1f, cancellationToken: destroyCancellationToken);

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
                    //@this.FillLeaderboardFromWorstAsync().ForgetByPrintException();
                })
                .RegisterDisposableTo(this);

            leaderboardView.ObserveHide()
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.worstLeaderboard.enabled = false;
                })
                .RegisterDisposableTo(this);
        }

        private UserProfile CreateUserProfile(LBPlayerData playerData)
        {
            var profile = new UserProfile(playerData.name)
            {
            };

            profile.AddTo(disposables);

            return profile;
        }

        private LeaderboardEntry CreateLeaderboardEntry(
            LBPlayerData playerData,
            UserProfile userProfile
            )
        {
            var entry = new LeaderboardEntry(userProfile);

            entry.AddTo(disposables);

            entry.AddScoreRecord("score").AddScore("score", playerData.score);

            return entry;
        }

        private void FillLeaderboardFromWorst(LBPlayerData[] players)
        {
            leaderboard.Clear();

            UserProfile userProfile;

            LeaderboardEntry leaderboardEntry;

            //var playerDatas = worstLeaderboard.Reflect()
            //    .Cache()
            //    .IncludeNonPublic()
            //    .WithName("players")
            //    .GetFieldValue<LBPlayerDataYG[]>()
            //    .GetValueUnsafe();

            foreach (var playerData in players)
            {
                userProfile = CreateUserProfile(playerData);

                leaderboardEntry = CreateLeaderboardEntry(playerData, userProfile);

                leaderboard.Add(leaderboardEntry).AddTo(disposables);
            }
        }
    }
}
#endif