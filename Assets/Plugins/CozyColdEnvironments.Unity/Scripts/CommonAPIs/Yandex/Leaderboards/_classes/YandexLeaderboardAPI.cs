#if PLUGIN_YG_2 && PLATFORM_WEBGL
using CCEnvs.Attributes;
using CCEnvs.Collections;
using CCEnvs.Dependencies;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Threading;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Leaderboards;
using CCEnvs.Unity.Profiles;
using CCEnvs.Unity.UI.Leaderboards;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Humanizer;
using R3;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;
using YG.Utils.LB;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs.Yandex
{
    public sealed class YandexLeaderboardAPI : ILeaderboardAPI
    {
        [field: OnInstallResetable]
        public static YandexLeaderboardAPI? Instance { get; private set; }

        private readonly List<IDisposable> lboardPopulationDisposable = new();
        private readonly List<IDisposable> disposables = new(); 

        private readonly LeaderboardView lboardView;

        private readonly string lboardname;

        private readonly DisposeCancellationTokenSource disposeCancellationTokenSource = new();

        private readonly IPlayerAPI playerAPI;

        private ILeaderboard? lboard;

        private IDisposable? specialEntryScoreSubb;

        public YandexLeaderboardAPI(
            LeaderboardView lboardView, 
            string lboardname,
            IPlayerAPI playerAPI
            )
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexLeaderboardAPI));

            CC.Guard.IsNotNull(lboardView, nameof(lboardView));
            Guard.IsNotNullOrWhiteSpace(lboardname, nameof(lboardname));
            CC.Guard.IsNotNull(playerAPI, nameof(playerAPI));

            this.lboardView = lboardView;
            this.lboardname = lboardname;
            this.playerAPI = playerAPI;

            YG2.onGetLeaderboard += OnLeaderboardDataChanged;

            BindLeaderboardViewAsync().ForgetByPrintException();
            BindLeaderboardSpecialProfileAsync().ForgetByPrintException();

            Instance = this;

            BuiltInDependecyContainer.Bind<ILeaderboardAPI>(this);
            BuiltInDependecyContainer.Bind(this);
        }

        private async UniTask BindLeaderboardViewAsync()
        {
            await UniTask.WaitUntil(lboardView,
                lboardView => lboardView.StartPassed,
                cancellationToken: disposeCancellationTokenSource.Token
                );

            lboardView.ObserveShow()
                .ThrottleFirst(1.5.Seconds()) //Delay duration taken from docs
                .Subscribe(this,
                static (_, @this) =>
                {
                    YG2.GetLeaderboard(@this.lboardname);
                })
                .AddTo(disposables);
        }

        private void OnLeaderboardSpecialEntryChanged(ILeaderboardEntry? specialEntry)
        {
            this.PrintLog($"Now Player leaderboard entry is {specialEntry}");

            specialEntryScoreSubb?.Dispose();

            if (specialEntry.IsNull())
                return;

            specialEntryScoreSubb = specialEntry.ObserveScore()
                .ThrottleLast(1.5.Seconds()) //Delay duration taken from docs
                .Subscribe(this,
                static (score, @this) =>
                {
                    YG2.SetLeaderboard(@this.lboardname, Mathf.RoundToInt(score));
                });
        }

        private async UniTask BindLeaderboardSpecialProfileAsync()
        {
            disposeCancellationTokenSource.Token.ThrowIfCancellationRequested();

            this.PrintLog($"{nameof(LeaderboardView)} loading");

            await UniTask.WaitUntil(
                lboardView,
                lboardView => lboardView.StartPassed && lboardView.model.IsNotDefault(),
                cancellationToken: disposeCancellationTokenSource.Token
                );

            this.PrintLog($"{nameof(LeaderboardView)} loaded");

            lboard = lboardView.GetModel<ILeaderboard>();

            YG2.GetLeaderboard(lboardname);

            this.PrintLog($"Setted player profile: {playerAPI.PlayerPofile}");

            lboard.SpecialProfile = playerAPI.PlayerPofile;

            lboard.ObserveSpecialEntry()
                .Subscribe(OnLeaderboardSpecialEntryChanged)
                .AddTo(disposables);

            if (!playerAPI.IsAuthorized)
            {
                playerAPI.ObserveIsAuthorised()
                    .Where(this,
                    static(_, @this) =>
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

            OnLeaderboardSpecialEntryChanged(lboard.SpecialEntry);
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            disposeCancellationTokenSource.Dispose();

            YG2.onGetLeaderboard -= OnLeaderboardDataChanged;

            lboardPopulationDisposable.DisposeEachAndClear();
            disposables.DisposeEachAndClear();

            specialEntryScoreSubb?.Dispose();

            if (lboard.IsNotNull() && lboard.SpecialEntry.IsNotNull())
                YG2.SetLeaderboard(lboardname, Mathf.RoundToInt(lboard.SpecialEntry.Score));

            disposed = true;
        }

        private void OnLeaderboardDataChanged(LBData? data)
        {
            this.PrintLog($"Recieved {nameof(LBData)}: {(data is not null ? data.players.Select(static player => (player.name, player.score)).ElementsToString() : "null")}");

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
                        .Map(lboard => lboard.SpecialEntry)
                        .Map(specialEntry => Mathf.RoundToInt(specialEntry.Score))
                        .GetValue()
                        );

                YG2.GetLeaderboard(lboardname);

                return;
            }

            this.PrintLog($"Player entry: (name: {YG2.player.name}; score: {data.currentPlayer!.score})");

            //var currentPlayerData = new LBPlayerData()
            //{
            //    name = YG2.player.name,
            //    photo = YG2.player.photo,
            //    uniqueID = YG2.player.id,
            //    rank = data.currentPlayer is not null ? data.currentPlayer.rank : 0,
            //    score = data.currentPlayer is not null ? data.currentPlayer.score : 0,
            //};

            //var players = data.players.Prepend(currentPlayerData)
            //    .DistinctBy(player => (player.name, player.uniqueID))
            //    .ToArray();

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
    }
}
#endif
