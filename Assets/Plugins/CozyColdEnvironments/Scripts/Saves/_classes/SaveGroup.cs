using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Snapshots;
using CCEnvs.Threading;
using CCEnvs.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Humanizer;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Saves
{
    [SerializationDescriptor("SaveGroup", "617e5bef-3872-4fae-b0d4-8d42f0893231")]
    public class SaveGroup
        :
        IEquatable<SaveGroup?>,
        IEnumerable<KeyValuePair<string, object>>,
        IDisposable
    {
        protected readonly ObservableDictionary<string, object> observableObjects = new();

        protected int? hashCode;

        protected SaveData? _saveData;

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        public IReadOnlyObservableDictionary<string, object> ObservableObjects => observableObjects;

        public string Name { get; init; }

        public SaveCatalog Catalog { get; init; }

        public SaveData SaveData {
            get
            {
                _saveData ??= new SaveData(Name);

                return _saveData;
            }
        }

        public bool IsDataLoadedFromFile { get; private set; }

        public object SyncRoot { get; } = new();

        protected CancellationToken DisposeCancellationToken {
            get => disposeCancellationTokenSource.Token;
        }

        public SaveGroup(
            SaveCatalog catalog,
            string? name = null
            )
        {
            Guard.IsNotNull(catalog, nameof(catalog));

            Name = name ?? string.Empty;
            Catalog = catalog;
        }

        public static SaveGroupIncremental ConvertToIncremental(SaveGroup group)
        {
            Guard.IsNotNull(group, nameof(group));

            if (group is SaveGroupIncremental incGroup)
                return incGroup;

            incGroup = new SaveGroupIncremental(group.Catalog, group.Name);

            try
            {
                lock (group.SyncRoot)
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

            var group = new SaveGroup(incGroup.Catalog, incGroup.Name);

            try
            {
                lock (incGroup.SyncRoot)
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

        public static bool operator ==(SaveGroup? left, SaveGroup? right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left is null || right is null)
                return false;

            return left.Name == right.Name
                   &&
                   left.Catalog == right.Catalog
                   &&
                   left._saveData == right._saveData
                   &&
                   left.disposed == right.disposed;
        }

        public static bool operator !=(SaveGroup? left, SaveGroup? right)
        {
            return !(left == right);
        }

        public SaveGroup RegisterObject(
            object obj,
            string? key,
            out string resolvedKey
            )
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            if (key is null && !SaveGroupObjectKey.TryResolve(obj, out key))
                key = string.Empty;

            resolvedKey = key;

            observableObjects.Add(key, obj);

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
            if (key is null || CCDisposable.IsDisposed(disposed))
                return false;

            return observableObjects.ContainsKey(key);
        }

        public bool IsObjectRegistered(object obj)
        {
            if (!SaveGroupObjectKey.TryResolve(obj, out var key))
                key = string.Empty;

            return IsObjectRegistered(key);
        }

        public SaveGroup CaptureAndWriteSaveData(
            WriteSaveDataMode writeSaveDataMode = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            using var saveUnits = CreateAndProcessSaveEntriesPooled();

            SaveData.Write(saveUnits.Value, writeSaveDataMode);

            return this;
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

        public async UniTask<SaveData?> GetSaveDataFromFileAsync(
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

            await UniTaskHelper.TrySwitchToThreadPool();

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(GetSaveDataFromFileAsync),
                expirationTimeRelativeToNow: 2.Minutes()
                );

            var result = new ValueReference<SaveData?>();

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, configureAwait, result))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result = await args.@this.GetSaveDataFromFileAsyncCore(
                        args.configureAwait,
                        cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(SaveSystem.CommandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);

            await UniTaskHelper.TrySwitchToMainThread(configureAwait);

            return result;
        }

        public async UniTask LoadSaveDataFromFileAsync(
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            bool force = false,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

            if (!force && IsDataLoadedFromFile)
                return;

            await UniTaskHelper.TrySwitchToThreadPool();

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadSaveDataFromFileAsync),
                expirationTimeRelativeToNow: 2.Minutes()
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, configureAwait, writeSaveDataMode))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadSaveDataFromFileAsyncCore(
                        args.writeSaveDataMode,
                        args.configureAwait,
                        cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(SaveSystem.CommandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);

            await UniTaskHelper.TrySwitchToMainThread(configureAwait);
        }

        public async UniTask<SaveData> GetOrLoadSaveDataFromFileAsync(
            WriteSaveDataMode writeSaveDataMode = default,
            bool forceGet = false,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

            if (forceGet || IsDataLoadedFromFile)
                return SaveData;

            await LoadSaveDataFromFileAsync(
                writeSaveDataMode,
                configureAwait,
                forceGet,
                cancellationToken
                );

            return SaveData;
        }

        public async UniTask WriteSaveDataToFileAsync(
            string fileExtension = SaveWrite.DEFAULT_SAVE_EXTENSION,
            bool compressed = true,
            bool backuped = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

            await UniTaskHelper.TrySwitchToThreadPool();

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(WriteSaveDataToFileAsync),
                expirationTimeRelativeToNow: TimeSpan.Zero
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, fileExtension, compressed, backuped, configureAwait))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.WriteSaveDataToFileAsyncCore(
                        fileExtension: args.fileExtension,
                        compressed: args.compressed,
                        backuped: args.backuped,
                        configureAwait: args.configureAwait,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(SaveSystem.CommandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);

            await UniTaskHelper.TrySwitchToMainThread(configureAwait);
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

        public bool Equals(SaveGroup? other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is SaveGroup typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(Name, Catalog, _saveData, disposed);

            return hashCode.Value;
        }

        private int disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (disposing)
            {
                disposeCancellationTokenSource.CancelAndDispose();
                observableObjects.Clear();
            }
        }

        /// <summary>
        /// !May change SaveData snapshot states!
        /// </summary>
        protected virtual PooledObject<List<SaveEntry>> CreateAndProcessSaveEntriesPooled()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            var saveUnits = ListPool<SaveEntry>.Shared.Get();

            lock (SyncRoot)
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

            saveEntry = new SaveEntry(, key, snapshot);

            return true;
        }

        private async UniTask<SaveData?> GetSaveDataFromFileAsyncCore(
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            cancellationToken.ThrowIfCancellationRequested();

            await UniTaskHelper.TrySwitchToThreadPool();

            var filePath = GetFullPath();

            try
            {
                var loadedSaveData = await SaveLoad.DataFromFileAsync(
                    filePath,
                    configureAwait: false,
                    cancellationToken
                    );

                return loadedSaveData;
            }
            catch (Exception ex)
            {
                this.PrintException(ex);

                return null;
            }
            finally
            {
                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
            }
        }

        private async UniTask LoadSaveDataFromFileAsyncCore(
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            cancellationToken.ThrowIfCancellationRequested();

            var loadedSaveData = await GetSaveDataFromFileAsyncCore(
                configureAwait: false,
                cancellationToken
                );

            await UniTaskHelper.TrySwitchToThreadPool();

            try
            {
                if (loadedSaveData is null)
                {
                    SaveData.Write(Array.Empty<SaveEntry>(), writeSaveDataMode);

                    IsDataLoadedFromFile = true;

                    return;
                }

                SaveData.Write(loadedSaveData.SaveEntries.Values, writeSaveDataMode);

                IsDataLoadedFromFile = true;
            }
            catch (Exception ex)
            {
                this.PrintException(ex);

                return;
            }
            finally
            {
                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
            }
        }

        private async UniTask WriteSaveDataToFileAsyncCore(
            string fileExtension = SaveWrite.DEFAULT_SAVE_EXTENSION,
            bool compressed = true,
            bool backuped = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            cancellationToken.ThrowIfCancellationRequested();

            await UniTaskHelper.TrySwitchToThreadPool();

            string serializedEntries = JsonConvert.SerializeObject(SaveData);

            var path = GetFullPath();

            var parameters = new WriteSaveDataToFileParameters(
                path,
                fileExtension
                )
            {
                Compressed = compressed,
                Backuped = backuped,
                FileContent = serializedEntries
            };

            await SaveWrite.ToFileAsync(
                parameters,
                configureAwait: false,
                cancellationToken: cancellationToken
                );

            await UniTaskHelper.TrySwitchToMainThread(configureAwait);
        }
    }
}
