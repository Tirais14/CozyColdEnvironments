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

        private readonly ObservableDictionary<string, ReactiveProperty<float>> scoreValues = new();

        private readonly List<IDisposable> disposables = new();

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private ReactiveCommand<(string name, float initialValue)>? addScoreCmd;
        private ReactiveCommand<string>? removeScoreCmd;

        public IUserProfile Profile { get; }

        public IReadOnlyObservableDictionary<string, ReactiveProperty<float>> ScoreValues => scoreValues;

        public float Score => score.Value;

        public LeaderboardEntry(IUserProfile userProfile)
        {
            Profile = userProfile;

            BindScoreValueAdd();
            BindScoreValueRemove();
            BindScoreValuesClear();
        }

        public IDisposable AddScore(string name, float initialValue = 0f)
        {
            if (ScoreValues.ContainsKey(name))
                throw new ArgumentException($"Score: {name} already exists");

            var prop = new ReactiveProperty<float>(initialValue);

            scoreValues.Add(name, prop);

            addScoreCmd?.Execute((name, initialValue));

            return Disposable.Create((@this: this, name),
                static (args) =>
                {
                    args.@this.RemoveScore(args.name);
                });
        }

        public bool RemoveScore(string name)
        {
            if (!scoreValues.Remove(name, out var prop))
                return false;

            prop.Dispose();

            removeScoreCmd?.Execute(name);

            return true;
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

                addScoreCmd?.Dispose();
                removeScoreCmd?.Dispose();
            }

            disposed = true;
        }

        public override string ToString()
        {
            return $"({nameof(Profile)}: {Profile}; {nameof(Score)}: {Score})";
        }

        public Observable<float> ObserveScore() => score;

        public Observable<(string name, float initialValue)> ObserveAddScore()
        {
            addScoreCmd ??= new ReactiveCommand<(string name, float initialValue)>();

            return addScoreCmd;
        }

        public Observable<string> ObserveRemoveScore()
        {
            removeScoreCmd ??= new ReactiveCommand<string>();

            return removeScoreCmd;
        }

        protected virtual void OnScoreValueAdd(string key, ReactiveProperty<float> prop)
        {
            var sub = prop.Pairwise()
                .Select(
                static pair =>
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
