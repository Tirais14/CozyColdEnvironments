using CCEnvs.Collections;
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
        private readonly List<IDisposable> disposables = new();

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private readonly Dictionary<Identifier, IDisposable> subbs = new();

        private readonly ObservableDictionary<Identifier, ILeaderboardEntry> entries = new();
        private readonly ObservableDictionary<Identifier, int> entryPositions = new();

        private readonly ObservableList<ILeaderboardEntry> sortedEntries = new();

        private readonly ReactiveProperty<IUserProfile?> specialProfile = new();
        private readonly ReactiveProperty<ILeaderboardEntry?> specialEntry = new();

        public IReadOnlyObservableDictionary<Identifier, ILeaderboardEntry> Entries => entries;

        public IReadOnlyObservableList<ILeaderboardEntry> SortedEntries => sortedEntries;

        public IReadOnlyObservableDictionary<Identifier, int> EntryPositions => entryPositions;

        public IUserProfile? SpecialProfile {
            get => specialProfile.Value;
            set => specialProfile.Value = value; 
        }

        public ILeaderboardEntry? SpecialEntry => specialEntry.Value;   

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
            BindSpecialProfile();
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

        public Observable<IUserProfile?> ObserveSpecialProfile()
        {
            return specialProfile;
        }

        public Observable<ILeaderboardEntry?> ObserveSpecialEntry()
        {
            return specialEntry;
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

            disposables.DisposeEachAndClear();

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

        private void ResolveSpecialEntry()
        {
            if (SpecialProfile.IsNull()
                ||
                !Entries.TryGetValue(SpecialProfile.ID, out var specialEntry))
            {
                this.specialEntry.Value = null;
                return;
            }

            this.specialEntry.Value = specialEntry;
        }

        private void OnEntryAdd(KeyValuePair<Identifier, ILeaderboardEntry> entry)
        {
            sortedEntries.Add(entry.Value);
            entryPositions.Add(entry.Key, -1000);

            ResolveSpecialEntry();

            var sub = entry.Value.ObserveScore()
                .ThrottleLastFrame(1)
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

            ResolveSpecialEntry();

            if (subbs.Remove(entry.Key, out var sub))
                sub.Dispose();
        }

        private void OnEntriesClear()
        {
            sortedEntries.Clear();
            entryPositions.Clear();
            specialEntry.Value = null;

            subbs.Values.DisposeEach();
            subbs.Clear();
        }

        private void BindEntryAdd()
        {
            entries.ObserveAdd(disposeCancellationTokenSource.Token)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) =>
                {
                    @this.OnEntryAdd(entry);
                })
                .AddTo(disposables);
        }

        private void BindEntryRemove()
        {
            entries.ObserveRemove(disposeCancellationTokenSource.Token)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) =>
                {
                    @this.OnEntryRemove(entry);
                })
                .AddTo(disposables);
        }

        private void BindEntriesClear()
        {
            entries.ObserveClear(disposeCancellationTokenSource.Token)
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.OnEntriesClear();
                })
                .AddTo(disposables);
        }

        private void OnSortedEntriesSort()
        {
            entryPositions.Clear();

            ILeaderboardEntry entry;

            int pos;

            for (int i = 0; i < sortedEntries.Count; i++)
            {
                entry = sortedEntries[i];

                pos = i + 1;

                entryPositions.Add(entry.Profile.ID, pos);
                entry.Position = pos;
            }
        }

        private void OnSortedEntryAdd(CollectionAddEvent<ILeaderboardEntry> entry)
        {
            //OnSortedEntryMove(entry.Value, entry.Index);
        }

        private void BindSortedEntryMove()
        {
            sortedEntries.ObserveSort(disposeCancellationTokenSource.Token)
                .Subscribe(this,
                static (entry, @this) =>
                {
                    @this.OnSortedEntriesSort();
                })
                .AddTo(disposables);
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
                .AddTo(disposables);
        }

        private void BindSpecialProfile()
        {
            specialProfile.Subscribe(this,
                static (profile, @this) =>
                {
                    if (profile.IsNull() || !@this.Entries.TryGetValue(profile.ID, out var entry))
                    {
                        @this.specialEntry.Value = null;
                        return;
                    }

                    @this.specialEntry.Value = entry;
                })
                .AddTo(disposables);
        }
    }
}
