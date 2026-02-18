using CCEnvs.Unity.Profiles;
using ObservableCollections;
using R3;
using System;
using System.Threading;

#nullable enable
namespace CCEnvs.Unity.Leaderboards
{
    public interface ILeaderboardEntry
        :
        IComparable<ILeaderboardEntry>,
        IDisposable
    {
        IUserProfile Profile { get; }

        IReadOnlyObservableDictionary<string, float> ScoreRecords { get; }

        float Score { get; }

        int? Position { get; set; }

        ILeaderboardEntry AddScoreRecord(string name, float initialValue = 0f);

        ILeaderboardEntry AddScore(string name, float value);

        ILeaderboardEntry AddScore(string name, float value, out float result);

        ILeaderboardEntry SubtractScore(string name, float value);

        ILeaderboardEntry SubtractScore(string name, float value, out float result);

        ILeaderboardEntry SetScore(string name, float value);

        bool RemoveScoreRecord(string name);

        Observable<float> ObserveScore();

        Observable<(string name, float initialValue)> ObserveAddScore();

        Observable<string> ObserveRemoveScore();

        Observable<int?> ObservePosition();

        Observable<float> ObserveScoreRecord(
            string recordName,
            CancellationToken cancellationToken = default
            );
    }
}
