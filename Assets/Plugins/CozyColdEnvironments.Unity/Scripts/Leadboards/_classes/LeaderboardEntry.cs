using CCEnvs.Unity.Profiles;
using CommunityToolkit.Diagnostics;
using ObservableCollections;
using R3;
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
        private readonly ReactiveProperty<int?> position = new();

        private readonly ObservableDictionary<string, float> scoreRecords = new();

        private readonly List<IDisposable> disposables = new();

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private ReactiveCommand<(string name, float initialValue)>? addScoreCmd;
        private ReactiveCommand<string>? removeScoreCmd;

        public IUserProfile Profile { get; }

        public IReadOnlyObservableDictionary<string, float> ScoreRecords => scoreRecords;

        public float Score => score.Value;

        public int? Position {
            get => position.Value;
            set => position.Value = value;
        }

        public LeaderboardEntry(IUserProfile userProfile)
        {
            Profile = userProfile;

            BindScoreRecordReplaced();
        }

        public ILeaderboardEntry AddScoreRecord(string name, float initialValue = 0f)
        {
            if (ScoreRecords.ContainsKey(name))
                throw new ArgumentException($"Score: {name} already exists");

            scoreRecords.Add(name, initialValue);

            OnScoreRecordReplaced(new DictionaryReplaceEvent<string, float>(name, default, initialValue));

            addScoreCmd?.Execute((name, initialValue));

            return this;
        }

        public bool RemoveScoreRecord(string name)
        {
            return scoreRecords.Remove(name);
        }

        public ILeaderboardEntry AddScore(string name, float value)
        {
            Guard.IsNotNull(name, nameof(name));

            scoreRecords[name] += value;

            return this;
        }

        public ILeaderboardEntry AddScore(string name, float value, out float result)
        {
            Guard.IsNotNull(name, nameof(name));

            scoreRecords[name] += value;

            result = scoreRecords[name];

            return this;
        }

        public ILeaderboardEntry SubtractScore(string name, float value)
        {
            Guard.IsNotNull(name, nameof(name));

            scoreRecords[name] -= value;

            return this;
        }

        public ILeaderboardEntry SubtractScore(string name, float value, out float result)
        {
            Guard.IsNotNull(name, nameof(name));

            scoreRecords[name] -= value;

            result = scoreRecords[name];

            return this;
        }

        public ILeaderboardEntry SetScore(string name, float value)
        {
            Guard.IsNotNull(name, nameof(name));

            scoreRecords[name] = value;

            return this;
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

        public Observable<int?> ObservePosition()
        {
            return position;
        }

        protected virtual void OnScoreRecordReplaced(DictionaryReplaceEvent<string, float> item)
        {
            var delta = item.NewValue - item.OldValue;

            score.Value += delta;
        }

        private void BindScoreRecordReplaced()
        {
            scoreRecords.ObserveDictionaryReplace(disposeCancellationTokenSource.Token)
                .Subscribe(this,
                static (item, @this) => @this.OnScoreRecordReplaced(item))
                .AddTo(disposables);
        }
    }
}
