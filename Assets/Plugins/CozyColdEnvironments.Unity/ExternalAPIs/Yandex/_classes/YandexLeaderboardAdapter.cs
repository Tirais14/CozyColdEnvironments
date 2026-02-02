#if Leaderboards_yg
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
using UnityEngine.UI;
using YG;
using YG.Example;

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

            worstLeaderboard.RectTransform()
                .MoveToDevCanvas()
                .RegisterDisposableTo(this);

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
                    @this.worstLeaderboard.UpdateLB();

                    @this.FillLeaderboardFromWorstAsync().ForgetByPrintException();
                })
                .RegisterDisposableTo(this);

            //leaderboardView.ObserveHide()
            //    .Subscribe(this,
            //    static (_, @this) =>
            //    {
            //    })
            //    .RegisterDisposableTo(this);
        }

        private UserProfile CreateUserProfile(GetPlayerData playerData)
        {
            var userName = playerData.Q()
                .FromChildrens()
                .IncludeInactive()
                .WithName("Name")
                .GameObject()
                .Strict()
                .Q()
                .FromChildrens()
                .IncludeInactive()
                .Component<Text>()
                .Strict()
                .text;

            return new UserProfile(userName, playerData.gameObject.GetInstanceID())
            {
                Icon = playerData.imageLoad.spriteImage.sprite
            };
        }

        private LeaderboardEntry CreateLeaderboardEntry(
            GetPlayerData playerData,
            UserProfile userProfile
            )
        {
            var scoreView = playerData.Q()
                .FromChildrens()
                .IncludeInactive()
                .WithName("Score")
                .GameObject()
                .Strict()
                .Q()
                .FromChildrens()
                .IncludeInactive()
                .Component<Text>()
                .Strict()
                .text;

            var score = int.Parse(scoreView);

            var entry = new LeaderboardEntry(userProfile);

            var scoreRX = new ReactiveProperty<float>(score);

            scoreRX.AddTo(disposables);

            entry.ScoreValues.Add("score", scoreRX);

            return entry;
        }

        private async UniTask FillLeaderboardFromWorstAsync()
        {
            await UniTask.NextFrame();

            leaderboard.Clear();

            UserProfile userProfile;
            LeaderboardEntry leaderboardEntry;

            foreach (var playerData in worstLeaderboard.gameObject.Q()
                .FromChildrens()
                .Components<GetPlayerData>())
            {
                userProfile = CreateUserProfile(playerData);
                leaderboardEntry = CreateLeaderboardEntry(playerData, userProfile);

                leaderboard.Add(leaderboardEntry).AddTo(disposables);
            }
        }
    }
}
#endif