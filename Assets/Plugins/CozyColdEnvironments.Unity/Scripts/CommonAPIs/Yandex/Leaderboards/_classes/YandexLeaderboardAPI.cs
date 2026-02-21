#if PLUGIN_YG_2 && PLATFORM_WEBGL
using CCEnvs.Collections;
using CCEnvs.Dependencies;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Threading;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Leaderboards;
using CCEnvs.Unity.Profiles;
using CCEnvs.Unity.Saves;
using CCEnvs.Unity.UI.Leaderboards;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Humanizer;
using R3;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using YG;
using YG.Utils.LB;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs.Yandex
{
    public sealed class YandexLeaderboardAPI : ILeaderboardAPI
    {
        private readonly List<IDisposable> lboardPopulationDisposable = new();
        private readonly List<IDisposable> disposables = new(); 

        private readonly LeaderboardView lboardView;

        private readonly string lboardname;

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private readonly IPlayerAPI playerAPI;

        private ILeaderboard? lboard;

        public YandexLeaderboardAPI(
            LeaderboardView lboardView, 
            string lboardname,
            IPlayerAPI playerAPI
            )
        {
            CC.Guard.IsNotNull(lboardView, nameof(lboardView));
            Guard.IsNotNullOrWhiteSpace(lboardname, nameof(lboardname));
            CC.Guard.IsNotNull(playerAPI, nameof(playerAPI));

            this.lboardView = lboardView;
            this.lboardname = lboardname;
            this.playerAPI = playerAPI;

            YG2.onGetLeaderboard += OnLeaderboardDataChanged;

            BindLeaderboardView();
            BindSavingSystem();

            CCDependecyContainer.Bind<ILeaderboardAPI>(this, lboardname);
            CCDependecyContainer.Bind(this, lboardname);
        }

        private void OnLeaderboardViewInited()
        {
            lboard = lboardView.GetModel<ILeaderboard>();

            if (playerAPI.IsAuthorized)
                lboard.SpecialProfile = playerAPI.PlayerPofile;
            else
                BindPlayerAPI();
        }

        private void BindLeaderboardView()
        {
            lboardView.ObserveIsInited()
                .Where(static x => x)
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.OnLeaderboardViewInited();
                })
                .AddTo(disposables);

            lboardView.ObserveShow()
                .Where(this,
                static (_, @this) =>
                {
                    return @this.lboard.IsNotNull();
                })
                .ThrottleFirst(1.5.Seconds()) //Delay duration taken from docs
                .Subscribe(this,
                static (_, @this) =>
                {
                    YG2.GetLeaderboard(@this.lboardname);
                })
                .AddTo(disposables);
        }

        private void BindPlayerAPI()
        {
            playerAPI.ObserveIsAuthorised()
                .Where(this,
                static (_, @this) =>
                {
                    return @this.lboard.IsNotNull();
                })
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.lboard!.SpecialProfile = @this.playerAPI.PlayerPofile;
                })
                .AddTo(disposables);
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            CCDependecyContainer.Unbind<ILeaderboardAPI>(lboardname);
            CCDependecyContainer.Unbind(GetType(), lboardname);

            disposeCancellationTokenSource.CancelAndDispose();

            YG2.onGetLeaderboard -= OnLeaderboardDataChanged;

            lboardPopulationDisposable.DisposeEachAndClear();
            disposables.DisposeEachAndClear();

            disposed = true;
        }

        private void OnLeaderboardDataChanged(LBData? data)
        {
#if CC_DEBUG_ENABLED
            this.PrintLog($"Recieved {nameof(LBData)}: {(data is not null ? data.players.Select(static player => (player.name, player.score)).ElementsToString() : "null")}");
#endif

            if (data is null
                ||
                data.technoName != lboardname)
            {
                return;
            }

            if (data.currentPlayer is null)
            {
                YG2.SetLeaderboard(
                    lboardname, 
                    lboard.Maybe()
                        .Map(static lboard => lboard.SpecialEntry)
                        .Map(static specialEntry => Mathf.RoundToInt(specialEntry.Score))
                        .GetValue()
                        );

                return;
            }
#if UNITY_EDITOR
            else
            {
                var currentPlayerRecord = data.players.Index()
                    .FirstOrDefault(
                    static (player) =>
                    {
                        return player.Item.name == YG2.player.name
                               &&
                               player.Item.uniqueID == YG2.player.id;
                    });

                if (currentPlayerRecord.Item is not null
                    &&
                    lboard.Maybe().Map(lboard => lboard.SpecialEntry).TryGetValue(out var specialEntry))
                {
                    currentPlayerRecord.Item.score = Mathf.RoundToInt(specialEntry.Score);

                    data.players[currentPlayerRecord.Index] = currentPlayerRecord.Item;
                }


            }
#endif

            this.PrintLog($"Player entry: (name: {YG2.player.name}; score: {data.currentPlayer!.score})");

            PopulateLeaderboardFromWorst(data.players);
        }

        private IUserProfile CreateUserProfile(
            LBPlayerData playerData
            )
        {
            var profile = new UserProfile(playerData.name, playerData.uniqueID).AddTo(lboardPopulationDisposable);

            YandexPluginHelper.LoadImageAsync(playerData.photo)
                .ContinueWith(img => profile.Icon = img)
                .ForgetByPrintException();

            return profile;
        }

        private ILeaderboardEntry CreateLeaderboardEntry(
            LBPlayerData playerData,
            IUserProfile userProfile
            )
        {
            return new LeaderboardEntry(userProfile).AddScoreRecord("score", playerData.score)
                .AddTo(lboardPopulationDisposable);
        }

        private void PopulateLeaderboardFromWorst(
            LBPlayerData[] players
            )
        {
            if (lboard.IsNull())
                return;

            lboardPopulationDisposable.DisposeEachAndClear();
            lboard.Clear();

            IUserProfile userProfile;

            ILeaderboardEntry leaderboardEntry;

            foreach (var playerData in players)
            {
                userProfile = CreateUserProfile(playerData);

                leaderboardEntry = CreateLeaderboardEntry(playerData, userProfile);

                lboard.Add(leaderboardEntry).AddTo(lboardPopulationDisposable);
            }
        }

        private void BindSavingSystem()
        {
            SavingSystem.Self.ObserveSaving()
                .Where(static x => x)
                .ThrottleLast(1.5.Seconds())
                .Subscribe(this,
                static (_, @this) =>
                {
                    var score = @this.lboard.Maybe()
                        .Map(static lboard => lboard.SpecialEntry)
                        .Map(static entry => Mathf.RoundToInt(entry.Score))
                        .GetValue();

                    YG2.SetLeaderboard(@this.lboardname, score);
                })
                .AddTo(disposables);
        }
    }
}
#endif
