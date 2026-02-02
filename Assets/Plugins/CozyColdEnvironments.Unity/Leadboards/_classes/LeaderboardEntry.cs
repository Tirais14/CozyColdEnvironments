using CCEnvs.Unity.Profiles;
using ObservableCollections;
using R3;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable enable
namespace CCEnvs.Unity.Leaderboards
{
    public class LeaderboardEntry : ILeaderboardEntry
    {
        public static LeaderboardEntry Empty => new(UserProfile.Empty);

        private readonly ReactiveProperty<float> score = new();

        private readonly Dictionary<string, IDisposable> subbs = new();

        private readonly List<IDisposable> disposables = new();

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        public IUserProfile Profile { get; }
        public ObservableDictionary<string, ReactiveProperty<float>> ScoreValues { get; } = new();
        public float Score => score.Value;

        public LeaderboardEntry(IUserProfile userProfile)
        {
            Profile = userProfile;

            BindScoreValueAdd();
            BindScoreValueRemove();
            BindScoreValuesClear();
        }

        public int CompareTo(ILeaderboardEntry other)
        {
            return Score.CompareTo(other.Score);
        }

        private bool disposed;
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                disposeCancellationTokenSource.Cancel();
                disposeCancellationTokenSource.Dispose();

                OnScoreValuesClear();

                disposables.DisposeEach();
                disposables.Clear();

                score.Dispose();
            }

            disposed = true;
        }

        public Observable<float> ObserveScore() => score;

        protected virtual void OnScoreValueAdd(string key, ReactiveProperty<float> prop)
        {
            var sub = prop.Pairwise()
                .Select(static pair =>
                {
                    return pair.Current - pair.Previous;
                })
                .Subscribe(this,
                static (scoreValue, @this) =>
                {
                    @this.score.Value += scoreValue;
                });

            subbs.Add(key, sub);
        }

        protected virtual void OnScoreValueRemove(string key)
        {
            if (subbs.Remove(key, out var sub))
                sub.Dispose();
        }

        protected virtual void OnScoreValuesClear()
        {
            subbs.ForEach(item => item.Value.Dispose());
            subbs.Clear();
        }

        private void BindScoreValueAdd()
        {
            ScoreValues.ObserveAdd(disposeCancellationTokenSource.Token)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) => @this.OnScoreValueAdd(item.Key, item.Value))
                .AddTo(disposables);
        }

        private void BindScoreValueRemove()
        {
            ScoreValues.ObserveRemove(disposeCancellationTokenSource.Token)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) => @this.OnScoreValueRemove(item.Key))
                .AddTo(disposables);
        }

        private void BindScoreValuesClear()
        {
            ScoreValues.ObserveClear(disposeCancellationTokenSource.Token)
                .Subscribe(this,
                static (_, @this) => @this.OnScoreValuesClear())
                .AddTo(disposables);
        }
    }
}
