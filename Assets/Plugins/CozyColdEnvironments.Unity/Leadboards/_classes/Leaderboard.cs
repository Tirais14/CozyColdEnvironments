using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Unity.Profiles;
using ObservableCollections;
using R3;
using SuperLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#nullable enable
namespace CCEnvs.Unity.Leaderboards
{
    public sealed class Leaderboard
        : 
        ILeaderboard
    {
        private readonly Dictionary<Identifier, IDisposable> subbs = new();

        private readonly ObservableDictionary<Identifier, ILeaderboardEntry> entries = new();

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private readonly ObservableList<ILeaderboardEntry> sortedEntries = new();

        private readonly ReactiveProperty<Maybe<IUserProfile>> activeProfile = new();

        public IReadOnlyObservableDictionary<Identifier, ILeaderboardEntry> Entries => entries;

        public IReadOnlyObservableList<ILeaderboardEntry> SortedEntries => sortedEntries;

        public Maybe<IUserProfile> SpecialProfile {
            get => activeProfile.Value;
            set => activeProfile.Value = value; 
        }

        public int Count => Entries.Count;

        bool ICollection<ILeaderboardEntry>.IsReadOnly => false;

        public Leaderboard()
        {
            BindEntryAdd();
            BindEntryRemove();
            BindEntriesClear();
        }

        public bool TryGetScore(Identifier userProfileID, out float score)
        {
            if (!entries.TryGetValue(userProfileID, out var entry))
            {
                score = float.MinValue;
                return false;
            }

            score = entry.Score;
            return true;
        }

        public IDisposable Add(ILeaderboardEntry entry)
        {
            CC.Guard.IsNotNull(entry, nameof(entry));

            entries.Add(entry.Profile.ID, entry);

            return entry;
        }

        public void Clear()
        {
            entries.Clear();
        }

        public bool Contains(ILeaderboardEntry entry)
        {
            CC.Guard.IsNotNull(entry, nameof(entry));

            return entries.ContainsKey(entry.Profile.ID);
        }

        public void CopyTo(ILeaderboardEntry[] array, int arrayIndex)
        {
            GetEnumerator().AsEnumerable().CopyTo(array, arrayIndex);
        }

        public bool Remove(ILeaderboardEntry entry)
        {
            CC.Guard.IsNotNull(entry, nameof(entry));

            return entries.Remove(entry.Profile.ID);
        }

        public Observable<Maybe<IUserProfile>> ObserveSpecialProfile()
        {
            return activeProfile;
        }

        public IEnumerator<ILeaderboardEntry> GetEnumerator()
        {
            return entries.Select(entry => entry.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            disposeCancellationTokenSource.Cancel();
            disposeCancellationTokenSource.Dispose();

            entries.ForEach(entry => entry.Value.Dispose());
            entries.Clear();

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
            entries.ObserveAdd(disposeCancellationTokenSource.Token)
                .Select(ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) => @this.OnEntryAdd(entry))
                .RegisterTo(disposeCancellationTokenSource.Token);
        }

        private void BindEntryRemove()
        {
            entries.ObserveRemove(disposeCancellationTokenSource.Token)
                .Select(ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) => @this.OnEntryRemove(entry))
                .RegisterTo(disposeCancellationTokenSource.Token);
        }

        private void BindEntriesClear()
        {
            entries.ObserveClear(disposeCancellationTokenSource.Token)
                .Subscribe(this,
                static (_, @this) => @this.OnEntriesClear())
                .RegisterTo(disposeCancellationTokenSource.Token);
        }
    }
}
