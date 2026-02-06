using CCEnvs.Collections;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Leaderboards;
using CCEnvs.Unity.Profiles;
using CCEnvs.Unity.UI.Leaderboards;
using Cysharp.Threading.Tasks;
using Humanizer;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
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

        private readonly Dictionary<string, ImageLoadYG> profileIconLoaders = new();

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
                .RegisterDisposableTo(this);

            leaderboardView.ObserveHide()
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.worstLeaderboard.enabled = false;
                })
                .RegisterDisposableTo(this);
        }

        private Sprite? LoadProfileIcon(string iconUrl, UserProfile profile)
        {
            if (iconUrl is null)
                return null;

            if (!profileIconLoaders.TryGetValue(iconUrl, out var loader))
            {
                var loaderGO = new GameObject(iconUrl);

                loader = loaderGO.AddComponent<ImageLoadYG>();

                loader.spriteImage = loaderGO.AddComponent<Image>();

                loader.spriteImage.color = loader.spriteImage.color.WithAlpha(0f);

                loader.urlImage = iconUrl;

                profileIconLoaders.Add(iconUrl, loader);

                loader.Load();
            }

            if (loader.spriteImage.sprite != null)
                return loader.spriteImage.sprite;

            Observable.EveryValueChanged(loader,
                static loader =>
                {
                    return loader.spriteImage.sprite;
                })
                .Timeout(5.Minutes())
                .Subscribe(profile,
                static (sprite, profile) =>
                {
                    profile.Icon = sprite;
                })
                .AddTo(disposables);

            return loader.spriteImage.sprite;
        }

        private IUserProfile CreateUserProfile(LBPlayerData playerData)
        {
            var profile = new UserProfile(playerData.name, playerData.uniqueID);

            profile.Icon = LoadProfileIcon(playerData.photo, profile);

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