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
            BindEntries();
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

        private void BindEntries()
        {
            Entries.ObserveAdd(disposeCancellationTokenSource.Token)
                .Select(ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) =>
                {
                    @this.sortedEntries.Add(entry.Value);

                    var sub = entry.Value.ObserveScore()
                        .Subscribe(@this,
                        static (_, @this) =>
                        {
                            @this.sortedEntries.Sort();
                        });

                    @this.subbs.Add(entry.Key, sub);
                })
                .RegisterTo(disposeCancellationTokenSource.Token);

            Entries.ObserveRemove(disposeCancellationTokenSource.Token)
                .Select(ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) =>
                {
                    @this.sortedEntries.Remove(entry.Value);

                    if (@this.subbs.Remove(entry.Key, out var sub))
                        sub.Dispose();
                })
                .RegisterTo(disposeCancellationTokenSource.Token);

            Entries.ObserveClear(disposeCancellationTokenSource.Token)
                .Merge(Entries.ObserveReset().AsUnitObservable())
                .Subscribe(this,
                static (_, @this) => @this.sortedEntries.Clear())
                .RegisterTo(disposeCancellationTokenSource.Token);
        }
    }
}
