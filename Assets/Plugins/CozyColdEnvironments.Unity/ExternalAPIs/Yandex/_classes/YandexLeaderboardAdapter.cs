using CCEnvs.Collections;
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
using UnityEngine.UI;
using YG;
using YG.Utils.LB;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    [RequireComponent(typeof(LeaderboardYG))]
    public sealed class YandexLeaderboardAdapter : CCBehaviour
    {
#if Leaderboards_yg
        private readonly List<IDisposable> disposables = new();

        private readonly Dictionary<string, ImageLoadYG> profileIconLoaders = new();

        private readonly Dictionary<string, Action> onProfileIconLoadedActions = new();

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

        private void LoadProfileIcon(string iconUrl, UserProfile profile)
        {
            if (!profileIconLoaders.TryGetValue(iconUrl, out var loader))
            {
                var laodedGO = new GameObject(iconUrl);

                loader = laodedGO.AddComponent<ImageLoadYG>();
                
                loader.spriteImage = laodedGO.AddComponent<Image>();

                loader.spriteImage.color = loader.spriteImage.color.WithAlpha(0f);

                loader.urlImage = iconUrl;

                profileIconLoaders.Add(iconUrl, loader);
                onProfileIconLoadedActions.Add(iconUrl, onLoaded);
            }

            loader.Load();

            void onLoaded()
            {
                profile.Icon = loader.spriteImage.sprite;
                onProfileIconLoadedActions.Remove(iconUrl);
            }
        }

        private UserProfile CreateUserProfile(LBPlayerData playerData)
        {
            var profile = new UserProfile(playerData.name, playerData.uniqueID);

            LoadProfileIcon(playerData.photo, profile);

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

        private void PopulateLeaderboardFromWorst(LBPlayerData[] players)
        {
            leaderboard.Clear();
            disposables.DisposeEachAndClear();

            UserProfile userProfile;

            LeaderboardEntry leaderboardEntry;

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