using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Serialization;
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
    [Serializable, JsonObject, PolymorphSerializable]
    [SerializationDescriptor("SaveGroup", "617e5bef-3872-4fae-b0d4-8d42f0893231")]
    public class SaveGroup
        :
        IEquatable<SaveGroup?>,
        IEnumerable<KeyValuePair<string, object>>,
        IDisposable
    {
        [JsonIgnore]
        protected readonly ObservableDictionary<string, object> observableObjects = new();

        [JsonIgnore]
        protected int? hashCode;

        [JsonProperty("saveData")]
        protected SaveData? _saveData;

        [JsonIgnore]
        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        [JsonIgnore]
        public IReadOnlyObservableDictionary<string, object> ObservableObjects => observableObjects;

        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("catalog")]
        public SaveCatalog Catalog { get; init; }

        [JsonProperty("incrementalWriting")]
        public bool IncrementalWriting { get; set; }

        [JsonIgnore]
        public SaveData SaveData {
            get
            {
                _saveData ??= new SaveData(this);

                return _saveData;
            }
        }

        [JsonIgnore]
        public bool IsDataLoadedFromFile { get; private set; }

        [JsonIgnore]
        protected CancellationToken DisposeCancellationToken {
            get => disposeCancellationTokenSource.Token;
        }

        [JsonIgnore]
        public object SyncRoot { get; } = new();

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

            lock (group.SyncRoot)
            {
                foreach (var (key, obj) in group.observableObjects)
                    incGroup.RegisterObject(obj, key);

                incGroup.SaveData.Merge(group.SaveData.SaveEntries.Values);
            }

            group.Dispose();

            return incGroup;
        }

        public static SaveGroup ConvertToNonIncremental(SaveGroupIncremental incGroup)
        {
            Guard.IsNotNull(incGroup, nameof(incGroup));

            var group = new SaveGroup(incGroup.Catalog, incGroup.Name);

            lock (incGroup.SyncRoot)
            {
                foreach (var (key, obj) in incGroup.observableObjects)
                    incGroup.RegisterObject(obj, key);

                group.SaveData.Merge(incGroup.SaveData.SaveEntries.Values);
            }

            incGroup.Dispose();

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
                   left._saveData == right._saveData;
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

            lock (SyncRoot)
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
            ThrowIfDisposed();

            Guard.IsNotNull(key, nameof(key));

            lock (SyncRoot)
                return observableObjects.Remove(key);
        }

        public bool UnregisterObject(object obj)
        {
            ThrowIfDisposed();

            if (!SaveGroupObjectKey.TryResolve(obj, out var key))
                key = string.Empty;

            return UnregisterObject(key);
        }

        public bool IsObjectRegistered(string? key)
        {
            if (key is null)
                return false;

            lock (SyncRoot)
                return observableObjects.ContainsKey(key);
        }

        public bool IsObjectRegistered(object obj)
        {
            if (!SaveGroupObjectKey.TryResolve(obj, out var key))
                key = string.Empty;

            return IsObjectRegistered(key);
        }

        public SaveGroup CaptureAndWriteSaveData(
            WriteSaveDataMode writeSaveDataMode = WriteSaveDataMode.Override
            )
        {
            ThrowIfDisposed();

            using var saveUnits = CreateAndProcessSaveEntriesPooled();

            SaveData.Write(saveUnits.Value, writeSaveDataMode);

            return this;
        }

        public string GetFullPath()
        {
            ThrowIfDisposed();

            using var sb = StringBuilderPool.Shared.Get();

            using var pathParts = PooledArray<string>.FromRange(
                Catalog.Archive.Path,
                Catalog.Path,
                Name
                );

            foreach (var pathPart in pathParts)
            {
                sb.Value.Append(pathPart);
                sb.Value.Append('/');
            }
            
            return sb.Value.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SaveGroup Clear()
        {
            ThrowIfDisposed();

            lock (SyncRoot)
                observableObjects.Clear();

            return this;
        }

        public async UniTask<SaveData?> GetSaveDataFromFileAsync(
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            ThrowIfDisposed();
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
            ThrowIfDisposed();
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
            ThrowIfDisposed();
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
            bool compressionEnabled = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            await UniTaskHelper.TrySwitchToThreadPool();

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(WriteSaveDataToFileAsync),
                expirationTimeRelativeToNow: TimeSpan.Zero
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, compressionEnabled, configureAwait))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.WriteSaveDataToFileAsyncCore(
                        args.compressionEnabled,
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
            hashCode ??= HashCode.Combine(Name, Catalog, _saveData);

            return hashCode.Value;
        }

        private bool disposed;
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                disposeCancellationTokenSource.CancelAndDispose();

                lock (SyncRoot)
                    observableObjects.Clear();
            }

            disposed = true;
        }

        /// <summary>
        /// !May change SaveData snapshot states!
        /// </summary>
        protected virtual PooledObject<List<SaveEntry>> CreateAndProcessSaveEntriesPooled()
        {
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

            saveEntry = new SaveEntry(key, snapshot);

            return true;
        }

        private void ThrowIfDisposed()
        {
            if (!disposed)
                return;

            throw new ObjectDisposedException(GetType().Name);
        }

        private async UniTask<SaveData?> GetSaveDataFromFileAsyncCore(
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            await UniTaskHelper.TrySwitchToThreadPool();

            var filePath = GetFullPath();

            try
            {
                var loadedSaveData = await SaveLoad.SaveDataFromFileAsync(
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
            bool compressionEnabeld = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            await UniTaskHelper.TrySwitchToThreadPool();

            string saveDataSerialized;

            try
            {
                saveDataSerialized = SerializeSaveDataAsyncCore();
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

            try
            {
                var path = GetFullPath();

                await SaveWrite.ToFileAsync(
                    path,
                    saveDataSerialized,
                    compressionEnabled: compressionEnabeld,
                    configureAwait: false,
                    cancellationToken: cancellationToken
                    );
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

        private string SerializeSaveDataAsyncCore()
        {
            return JsonConvert.SerializeObject(SaveData, CC.SerializerSettings) ?? string.Empty;
        }
    }
}
