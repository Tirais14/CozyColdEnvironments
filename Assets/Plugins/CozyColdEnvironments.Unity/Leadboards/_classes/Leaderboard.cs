using CCEnvs.Linq;
using ObservableCollections;
using R3;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable enable
namespace CCEnvs.Unity.Leaderboards
{
    public sealed class Leaderboard
        : 
        ILeaderboard
    {
        private readonly Dictionary<Identifier, IDisposable> subbs = new();

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private readonly List<ILeaderboardEntry> sortedEntries = new();

        public ObservableDictionary<Identifier, ILeaderboardEntry> Entries { get; } = new();
        public IList<ILeaderboardEntry> SortedEntries => sortedEntries;

        public Leaderboard()
        {
            BindEntryAdd();
            BindEntryRemove();
            BindEntriesClear();
        }

        public float GetScore(Identifier userProfileID)
        {
            if (!Entries.TryGetValue(userProfileID, out var entry))
                throw new ArgumentException($"Cannot find user profile with id: {userProfileID}");

            return entry.Score;
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            disposeCancellationTokenSource.Cancel();
            disposeCancellationTokenSource.Dispose();

            Entries.ForEach(entry => entry.Value.Dispose());
            Entries.Clear();

            subbs.Values.DisposeEach();
            subbs.Clear();

            disposed = true;
        }

        private void OnEntryAdd(KeyValuePair<Identifier, ILeaderboardEntry> entry)
        {
            sortedEntries.Add(entry.Value);

            var sub = entry.Value.ObserveScore()
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.sortedEntries.Sort();
                });

            subbs.Add(entry.Key, sub);
        }

        private void OnEntryRemove(KeyValuePair<Identifier, ILeaderboardEntry> entry)
        {
            sortedEntries.Remove(entry.Value);

            if (subbs.Remove(entry.Key, out var sub))
                sub.Dispose();
        }

        private void OnEntriesClear()
        {
            sortedEntries.Clear();
        }

        private void BindEntryAdd()
        {
            Entries.ObserveAdd(disposeCancellationTokenSource.Token)
                .Select(ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) => @this.OnEntryAdd(entry))
                .RegisterTo(disposeCancellationTokenSource.Token);
        }

        private void BindEntryRemove()
        {
            Entries.ObserveRemove(disposeCancellationTokenSource.Token)
                .Select(ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) => @this.OnEntryRemove(entry))
                .RegisterTo(disposeCancellationTokenSource.Token);
        }

        private void BindEntriesClear()
        {
            Entries.ObserveClear(disposeCancellationTokenSource.Token)
                .Subscribe(this,
                static (_, @this) => @this.OnEntriesClear())
                .RegisterTo(disposeCancellationTokenSource.Token);
        }
    }
}
