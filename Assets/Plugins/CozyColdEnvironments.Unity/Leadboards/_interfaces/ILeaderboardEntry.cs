using CCEnvs.Unity.Profiles;
using ObservableCollections;
using R3;
using System;

#nullable enable
namespace CCEnvs.Unity.Leaderboards
{
    public interface ILeaderboardEntry
        :
        IComparable<ILeaderboardEntry>,
        IDisposable
    {
        IUserProfile Profile { get; }

        IReadOnlyObservableDictionary<string, ReactiveProperty<float>> ScoreValues { get; }

        float Score { get; }

        IDisposable AddScore(string name, float initialValue = 0f);

        bool RemoveScore(string name);

        Observable<float> ObserveScore();

        Observable<(string name, float initialValue)> ObserveAddScore();

        Observable<string> ObserveRemoveScore();
    }
}
