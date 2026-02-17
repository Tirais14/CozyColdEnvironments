#if PLUGIN_YG_2 && Leaderboards_yg && PLATFORM_WEBGL
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Leaderboards;
using CCEnvs.Unity.Profiles;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Humanizer;
using R3;
using System;
using System.Threading;
using YG;
using YG.Utils.LB;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs.Yandex.Leaderboards
{
    public static class YandexLeaderboardHelper
    {
        public static LBData? LeaderboardData { get; private set; }

        private static bool isLeaderboardPopulating;

        static YandexLeaderboardHelper()
        {
            YG2.onGetLeaderboard += OnLeaderboardDataChanged;
        }

        public static async UniTask<IDisposable> PopulateLeaderboardAsync(
            ILeaderboard lboard,
            string ygLboardName,
            int quantityTop = 3,
            int quantityAround = 3,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (isLeaderboardPopulating)
                return Disposable.Empty;

            CC.Guard.IsNotNull(lboard, nameof(lboard));
            Guard.IsNotNullOrWhiteSpace(ygLboardName, nameof(ygLboardName));

            quantityTop = Math.Clamp(quantityTop, 1, int.MaxValue);
            quantityAround = Math.Clamp(quantityAround, 0, int.MaxValue);

            var lbData = LeaderboardData;

            YG2.GetLeaderboard(ygLboardName, quantityTop, quantityAround);

            isLeaderboardPopulating = true;

            try
            {
                await UniTask.WaitUntil(lbData,
                    static (lbData) =>
                    {
                        return LeaderboardData != lbData;
                    },
                    cancellationToken: cancellationToken
                    )
                    .Timeout(2.Minutes());

                var disposables = new CompositeDisposable();

                PopulateLeaderboardFromWorst(lboard, LeaderboardData!.players, disposables);

                isLeaderboardPopulating = false;

                return disposables;
            }
            catch (Exception ex)
            {
                typeof(YandexLeaderboardHelper).PrintException(ex);

                return Disposable.Empty;
            }
            finally
            {
                isLeaderboardPopulating = false;
            }
        }

        private static void OnLeaderboardDataChanged(LBData data)
        {
            LeaderboardData = data;
        }

        private static IUserProfile CreateUserProfile(
            LBPlayerData playerData,
            CompositeDisposable disposables
            )
        {
            var profile = new UserProfile(playerData.name, playerData.uniqueID).AddTo(disposables);

            YandexPluginHelper.LoadImageAsync(playerData.photo)
                .ContinueWith(
                img =>
                {
                    profile.Icon = img;
                })
                .ForgetByPrintException();

            return profile;
        }

        private static ILeaderboardEntry CreateLeaderboardEntry(
            LBPlayerData playerData,
            IUserProfile userProfile,
            CompositeDisposable disposables
            )
        {
            return new LeaderboardEntry(userProfile).AddScoreRecord("score", playerData.score)
                .AddTo(disposables);
        }

        private static void PopulateLeaderboardFromWorst(
            ILeaderboard lboard,
            LBPlayerData[] players,
            CompositeDisposable disposables
            )
        {
            lboard.Clear();

            IUserProfile userProfile;

            ILeaderboardEntry leaderboardEntry;

            foreach (var playerData in players)
            {
                userProfile = CreateUserProfile(playerData, disposables);

                leaderboardEntry = CreateLeaderboardEntry(playerData, userProfile, disposables);

                lboard.Add(leaderboardEntry).AddTo(disposables);
            }
        }
    }
}
#endif
