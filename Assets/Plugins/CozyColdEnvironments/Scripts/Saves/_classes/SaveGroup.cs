using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Snapshots;
using CCEnvs.Threading;
using CommunityToolkit.Diagnostics;
using ObservableCollections;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable enable
namespace CCEnvs.Saves
{
    public class SaveGroup
        :
        IEnumerable<KeyValuePair<string, object>>,
        IDisposable
    {
        internal readonly CommandScheduler commandScheduler = CommandScheduler.CreateDefaultRegistered(nameof(SaveGroup));

        protected readonly ObservableDictionary<string, object> observableObjects = new();

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private bool objectWasRegistered;

        public IReadOnlyObservableDictionary<string, object> ObservableObjects => observableObjects;

        public string Name { get; }

        public SaveCatalog Catalog { get; }

        public SaveData SaveData { get; }

        public SaveGroupLoader Loader { get; }

        public SaveGroupSerializer Serializer { get; }

        public SaveGroupWriter Writer { get; }

        public RedirectionMode Redirection { get; }

        public bool LoadOnFirstObjectRegistered { get; }

        protected CancellationToken DisposeCancellationToken {
            get => disposeCancellationTokenSource.Token;
        }

        public SaveGroup(
            SaveCatalog catalog,
            string? name = null,
            long saveDataVersion = 0L,
            RedirectionMode redirectionMode = default,
            bool loadOnFirstObjectRegistered = true
            )
        {
            Guard.IsNotNull(catalog, nameof(catalog));

            Name = name ?? string.Empty;
            Catalog = catalog;
            SaveData = new SaveData(Name, saveDataVersion);
            Redirection = redirectionMode;
            Loader = new SaveGroupLoader(this);
            Serializer = new SaveGroupSerializer(this);
            Writer = new SaveGroupWriter(this);
            LoadOnFirstObjectRegistered = loadOnFirstObjectRegistered;
        }

        ~SaveGroup() => Dispose();

        public static SaveGroupIncremental ConvertToIncremental(SaveGroup group)
        {
            Guard.IsNotNull(group, nameof(group));

            if (group is SaveGroupIncremental incGroup)
                return incGroup;

            incGroup = new SaveGroupIncremental(
                group.Catalog,
                group.Name,
                saveDataVersion: group.SaveData.Version,
                redirectionMode: group.Redirection,
                loadOnFirstObjectRegistered: group.LoadOnFirstObjectRegistered
                );

            try
            {
                lock (group.observableObjects.SyncRoot)
                {
                    foreach (var (key, obj) in group.observableObjects)
                        incGroup.RegisterObject(obj, key);

                    incGroup.SaveData.Merge(group.SaveData.SaveEntries.Values);
                }

                group.Dispose();
            }
            catch (Exception)
            {
                incGroup.Dispose();
                throw;
            }

            return incGroup;
        }

        public static SaveGroup ConvertToNonIncremental(SaveGroupIncremental incGroup)
        {
            Guard.IsNotNull(incGroup, nameof(incGroup));

            var group = new SaveGroup(
                incGroup.Catalog, 
                incGroup.Name,
                saveDataVersion: incGroup.SaveData.Version,
                redirectionMode: incGroup.Redirection,
                loadOnFirstObjectRegistered: incGroup.LoadOnFirstObjectRegistered
                );

            try
            {
                lock (incGroup.observableObjects.SyncRoot)
                {
                    foreach (var (key, obj) in incGroup.observableObjects)
                        incGroup.RegisterObject(obj, key);

                    group.SaveData.Merge(incGroup.SaveData.SaveEntries.Values);
                }

                incGroup.Dispose();
            }
            catch (Exception)
            {
                group.Dispose();
                throw;
            }

            return group;
        }

        public SaveGroup RegisterObject(
            object obj,
            string? key,
            out string resolvedKey
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            CC.Guard.IsNotNull(obj, nameof(obj));

            if (key is null && !SaveGroupObjectKey.TryResolve(obj, out key))
                key = string.Empty;

            resolvedKey = key;

            observableObjects.Add(key, obj);

            if (!objectWasRegistered
                &&
                !Loader.IsDataLoaded
                &&
                LoadOnFirstObjectRegistered)
            {
                Loader.LoadSaveDataFromFileAsync(cancellationToken: DisposeCancellationToken).Forget(this);
            }

            objectWasRegistered = true;

            return this;
        }

        public SaveGroup RegisterObject(object obj, string? key = null)
        {
            return RegisterObject(obj, key, out _);
        }

        public SaveGroupRegistration RegisterObjectHandled(object obj, string? key = null)
        {
            RegisterObject(obj, key, out var resolvedKey);

            return new SaveGroupRegistration(this, resolvedKey);
        }

        public bool UnregisterObject(string key)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            Guard.IsNotNull(key, nameof(key));

            return observableObjects.Remove(key);
        }

        public bool UnregisterObject(object obj)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (!SaveGroupObjectKey.TryResolve(obj, out var key))
                key = string.Empty;

            return UnregisterObject(key);
        }

        public bool IsObjectRegistered(string? key)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (key is null || CCDisposable.IsDisposed(disposed))
                return false;

            return observableObjects.ContainsKey(key);
        }

        public bool IsObjectRegistered(object obj)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (!SaveGroupObjectKey.TryResolve(obj, out var key))
                key = string.Empty;

            return IsObjectRegistered(key);
        }

        public string GetFullPath()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            var arch = Catalog.Archive;

            return Path.Join(arch.Path, Catalog.Path, Name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SaveGroup Clear()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            observableObjects.Clear();

            return this;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return observableObjects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            return $"({nameof(Name)}: {Name}; {nameof(Catalog)}: {Catalog})";
        }

        private int disposed;
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (disposing)
            {
                disposeCancellationTokenSource.CancelAndDispose();
                observableObjects.Clear();
                Loader.Dispose();
                Writer.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// !May change SaveData snapshot states!
        /// </summary>
        internal virtual PooledObject<List<SaveEntry>> CreateAndProcessSaveEntriesPooled()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            var saveUnits = ListPool<SaveEntry>.Shared.Get();

            lock (observableObjects.SyncRoot)
            {
                int observableObjectCount = observableObjects.Count;

                saveUnits.Value.TryIncreaseCapacity(observableObjectCount);

                foreach (var (key, obj) in observableObjects)
                {
                    if (!TryCreateAndProcessSaveEntry(key, obj, out var saveEntry))
                        continue;

                    saveUnits.Value.Add(saveEntry);
                }
            }

            return saveUnits;
        }

        protected bool TryCreateAndProcessSaveEntry(
            string key,
            object obj,
            out SaveEntry saveEntry
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            ISnapshot snapshot;

            SnapshotFactory snapshotFactory;

            if (SaveData.SaveEntries.TryGetValue(key, out saveEntry))
            {
                snapshot = saveEntry.Snapshot;
                snapshot = snapshot.CaptureFrom(obj);
            }
            else
            {
                snapshotFactory = SaveSystem.ResolveConverter(obj.GetType());

                try
                {
                    snapshot = snapshotFactory(obj);
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                    return false;
                }

                if (snapshot.IsNull())
                {
                    this.PrintError($"Snapshot factory of type: {obj.GetType()} returned null");
                    return false;
                }
            }

            saveEntry = new SaveEntry(SaveData.Version, key, snapshot);

            return true;
        }
    }
}
