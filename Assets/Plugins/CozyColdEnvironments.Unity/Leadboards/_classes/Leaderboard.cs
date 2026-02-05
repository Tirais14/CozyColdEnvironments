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
        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private readonly Dictionary<Identifier, IDisposable> subbs = new();

        private readonly ObservableDictionary<Identifier, ILeaderboardEntry> entries = new();

        private readonly ObservableDictionary<Identifier, int> entryPositions = new();

        private readonly ObservableList<ILeaderboardEntry> sortedEntries = new();

        private readonly ReactiveProperty<Maybe<IUserProfile>> specialProfile = new();

        public IReadOnlyObservableDictionary<Identifier, ILeaderboardEntry> Entries => entries;

        public IReadOnlyObservableList<ILeaderboardEntry> SortedEntries => sortedEntries;

        public IReadOnlyObservableDictionary<Identifier, int> EntryPositions => entryPositions;

        public Maybe<IUserProfile> SpecialProfile {
            get => specialProfile.Value;
            set => specialProfile.Value = value; 
        }

        public IComparer<ILeaderboardEntry> Comparer { get; set; } = null!;

        public int Count => Entries.Count;

        bool ICollection<ILeaderboardEntry>.IsReadOnly => false;

        public Leaderboard(bool reversedEntryComparer = true)
        {
            SetComparer(reversedEntryComparer);
            BindEntryAdd();
            BindEntryRemove();
            BindEntriesClear();
            BindSortedEntryMove();
            BindSortedEntryAdd();
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
            return specialProfile;
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

        private void SetComparer(bool reversedEntryComparer)
        {
            if (reversedEntryComparer)
            {
                Comparer = Comparer<ILeaderboardEntry>.Create(
                    static (left, right) =>
                    {
                        return left.CompareTo(right) * -1;
                    });
            }
            else
                Comparer = Comparer<ILeaderboardEntry>.Default;
        }

        private void SortEntries()
        {
            if (Comparer is not null)
                sortedEntries.Sort(Comparer);
            else
                sortedEntries.Sort();
        }

        private void OnEntryAdd(KeyValuePair<Identifier, ILeaderboardEntry> entry)
        {
            sortedEntries.Add(entry.Value);
            entryPositions.Add(entry.Key, -1000);

            var sub = entry.Value.ObserveScore()
                .ChunkFrame(1)
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.SortEntries();
                });

            subbs.Add(entry.Key, sub);
        }

        private void OnEntryRemove(KeyValuePair<Identifier, ILeaderboardEntry> entry)
        {
            sortedEntries.Remove(entry.Value);
            entryPositions.Remove(entry.Key);

            if (subbs.Remove(entry.Key, out var sub))
                sub.Dispose();
        }

        private void OnEntriesClear()
        {
            sortedEntries.Clear();
            entryPositions.Clear();
        }

        private void BindEntryAdd()
        {
            entries.ObserveAdd(disposeCancellationTokenSource.Token)
                .Select(static ev => ev.Value)
                .ChunkFrame(1)
                .Subscribe(this,
                static (entries, @this) =>
                {
                    for (int i = 0; i < entries.Length; i++)
                        @this.OnEntryAdd(entries[i]);
                })
                .RegisterTo(disposeCancellationTokenSource.Token);
        }

        private void BindEntryRemove()
        {
            entries.ObserveRemove(disposeCancellationTokenSource.Token)
                .Select(static ev => ev.Value)
                .ChunkFrame(1)
                .Subscribe(this,
                static (entries, @this) =>
                {
                    for (int i = 0; i < entries.Length; i++)
                        @this.OnEntryRemove(entries[i]);
                })
                .RegisterTo(disposeCancellationTokenSource.Token);
        }

        private void BindEntriesClear()
        {
            entries.ObserveClear(disposeCancellationTokenSource.Token)
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.OnEntriesClear();
                })
                .RegisterTo(disposeCancellationTokenSource.Token);
        }

        private void OnSortedEntryMove(CollectionMoveEvent<ILeaderboardEntry> entry)
        {
            entryPositions[entry.Value.Profile.ID] = entry.NewIndex;
            entry.Value.Position = entry.NewIndex;
        }

        private void OnSortedEntryAdd(CollectionAddEvent<ILeaderboardEntry> entry)
        {
            OnSortedEntryMove(new CollectionMoveEvent<ILeaderboardEntry>(-1, entry.Index + 1, entry.Value));
        }

        private void BindSortedEntryMove()
        {
            sortedEntries.ObserveMove(disposeCancellationTokenSource.Token)
                .ChunkFrame(1)
                .Subscribe(this,
                static (entries, @this) =>
                {
                    foreach (var entry in entries)
                        @this.OnSortedEntryMove(entry);
                })
                .RegisterTo(disposeCancellationTokenSource.Token);
        }

        private void BindSortedEntryAdd()
        {
            sortedEntries.ObserveAdd(disposeCancellationTokenSource.Token)
                .ChunkFrame(1)
                .Subscribe(this,
                static (events, @this) =>
                {
                    for (int i = 0; i < events.Length; i++)
                        @this.OnSortedEntryAdd(events[i]);
                })
                .RegisterTo(disposeCancellationTokenSource.Token);
        }
    }
}
