#if Leaderboards_yg
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

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    [RequireComponent(typeof(LeaderboardYG))]
    public sealed class YandexLeaderboardAdapter : CCBehaviour
    {
        private readonly List<IDisposable> disposables = new();

        [GetBySelf]
        private LeaderboardYG worstLeaderboard = null!;

        private ILeaderboard leaderboard = null!;

        private LeaderboardView leaderboardView = null!;

        protected override void Start()
        {
            base.Start();
            InitAsync().ForgetByPrintException(this);
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
                    @this.worstLeaderboard.gameObject.SetActive(true);
                    @this.FillLeaderboardFromWorstAsync().ForgetByPrintException();
                })
                .RegisterDisposableTo(this);

            leaderboardView.ObserveHide()
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.worstLeaderboard.gameObject.SetActive(false);
                })
                .RegisterDisposableTo(this);
        }

        private UserProfile CreateUserProfile(LBPlayerDataYG.Data playerData)
        {
            var profile = new UserProfile(playerData.name)
            {
                Icon = playerData.photoSprite
            };

            profile.AddTo(disposables);

            return profile;
        }

        private LeaderboardEntry CreateLeaderboardEntry(
            LBPlayerDataYG.Data playerData,
            UserProfile userProfile
            )
        {
            var entry = new LeaderboardEntry(userProfile);

            entry.AddTo(disposables);

            var score = int.Parse(playerData.score);

            var scoreRX = new ReactiveProperty<float>(score);

            scoreRX.AddTo(disposables);

            entry.ScoreValues.Add("score", scoreRX);

            return entry;
        }

        private async UniTask FillLeaderboardFromWorstAsync()
        {
            destroyCancellationToken.ThrowIfCancellationRequested();

            await UniTask.NextFrame(cancellationToken: destroyCancellationToken);

            leaderboard.Clear();

            UserProfile userProfile;

            LeaderboardEntry leaderboardEntry;

            var playerDatas = worstLeaderboard.Reflect()
                .Cache()
                .IncludeNonPublic()
                .WithName("players")
                .GetFieldValue<LBPlayerDataYG[]>()
                .GetValueUnsafe();

            foreach (var playerData in playerDatas)
            {
                userProfile = CreateUserProfile(playerData.data);

                leaderboardEntry = CreateLeaderboardEntry(playerData.data, userProfile);

                leaderboard.Add(leaderboardEntry).AddTo(disposables);
            }
        }
    }
}
#endif