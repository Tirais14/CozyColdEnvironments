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

        ObservableDictionary<string, ReactiveProperty<float>> ScoreValues { get; }

        float Score { get; }

        Observable<float> ObserveScore();
    }
}
