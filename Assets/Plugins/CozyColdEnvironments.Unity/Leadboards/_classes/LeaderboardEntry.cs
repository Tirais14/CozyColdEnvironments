using CCEnvs.Unity.Profiles;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Leaderboards
{
    public class LeaderboardEntry : ILeaderboardEntry
    {
        private readonly Dictionary<string, IDisposable> subbs = new();
        private readonly List<IDisposable> disposables = new();
        private readonly ReactiveProperty<float> score = new();

        public IUserProfile Profile { get; }
        public ObservableDictionary<string, ReactiveProperty<float>> ScoreValues { get; } = new();
        public float Score => score.Value;

        public LeaderboardEntry(IUserProfile userProfile)
        {
            Profile = userProfile;

            BindCollection();
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
                foreach (var item in ScoreValues)
                    item.Value.Dispose();

                ScoreValues.Clear();

                score.Dispose();
            }

            disposed = true;
        }

        public Observable<float> ObserveScore() => score;

        private void OnAdd(string key, ReactiveProperty<float> prop)
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

        private void OnRemove(string key)
        {
            if (subbs.Remove(key, out var sub))
                sub.Dispose();
        }

        private void BindCollection()
        {
            ScoreValues.ObserveAdd()
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) => @this.OnAdd(item.Key, item.Value))
                .AddTo(disposables);

            ScoreValues.ObserveRemove()
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) => @this.OnRemove(item.Key))
                .AddTo(disposables);
        }
    }
}
