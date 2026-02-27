using CCEnvs.Attributes.Serialization;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Snapshots;
using CCEnvs.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    [TypeSerializationDescriptor("Saves.SaveGroup", "617e5bef-3872-4fae-b0d4-8d42f0893231")]
    public sealed class SaveGroup
        :
        IEquatable<SaveGroup?>,
        IEnumerable<KeyValuePair<string, object>>
    {
        private readonly ObservableDictionary<string, object> observableObjects = new();

        private int? hashCode;

        [JsonProperty("saveData")]
        private SaveData? _saveData;

        [JsonIgnore]
        public IReadOnlyObservableDictionary<string, object> ObservableObjects => observableObjects;

        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("catalog")]
        public SaveCatalog Catalog { get; init; }

        [JsonIgnore]
        public SaveData SaveData {
            get
            {
                _saveData ??= new SaveData(this);

                return _saveData;
            }
        }

        [JsonIgnore]
        public bool IsSaveLoadedFromFile { get; private set; }

        public SaveGroup(
            SaveCatalog catalog,
            string? name = null
            )
        {
            Guard.IsNotNull(catalog, nameof(catalog));

            Name = name ?? string.Empty;
            Catalog = catalog;
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

        public SaveGroup RegisterObject(object obj, string? key = null)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            if (key is null && !TryResolveKey(obj, out key))
                key = string.Empty;

            observableObjects.Add(key, obj);

            return this;
        }

        public bool UnregisterObject(string key)
        {
            Guard.IsNotNull(key, nameof(key));

            return observableObjects.Remove(key);
        }

        public bool UnregisterObject(object obj)
        {
            if (!TryResolveKey(obj, out var key))
                key = string.Empty;

            return UnregisterObject(key);
        }

        public bool IsObjectRegistered(string? key)
        {
            if (key is null)
                return false;

            return observableObjects.ContainsKey(key);
        }

        public bool IsObjectRegistered(object obj)
        {
            if (!TryResolveKey(obj, out var key))
                key = string.Empty;

            return IsObjectRegistered(key);
        }

        public bool TryResolveKey(
            object obj, 
            [NotNullWhen(true)] out string? key
            )
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            key = obj switch
            {
                GameObject go => ResolveGameObjectKey(go),
                Component cmp => ResolveComponentKey(cmp),
                _ => null
            };

            return key is not null;
        }

        public ISnapshot GetObjectSnapshot(string? key)
        {
            key ??= string.Empty;

            var obj = observableObjects[key];

            var objType = obj.GetType();

            var converter = SaveSystem.ResolveConverter(objType);

            return converter(obj);
        }

        public PooledArray<SaveUnit> CreateSaveUnitsPooled()
        {
            var saveUnits = new PooledArray<SaveUnit>(observableObjects.Count);

            SaveUnit saveUnit;

            ISnapshot snapshot;

            Func<object, ISnapshot> objConverter;

            int i = 0;

            foreach (var (key, obj) in observableObjects)
            {
                objConverter = SaveSystem.ResolveConverter(obj.GetType());

                snapshot = objConverter(obj);

                saveUnit = new SaveUnit(key, snapshot);

                saveUnits[i++] = saveUnit;
            }

            return saveUnits;
        }

        public SaveGroup CaptureAndWriteSaveData(
            WriteSaveDataMode writeSaveDataMode = WriteSaveDataMode.Override
            )
        {
            using var saveUnits = CreateSaveUnitsPooled();

            SaveData.Write(saveUnits, writeSaveDataMode);

            return this;
        }

        public string GetFullPath()
        {
            using var sb = StringBuilderPool.Shared.Get();

            using var pathParts = PooledArray<string>.FromRange(
                Catalog.Archive.Path,
                Catalog.Path,
                Name
                );

            sb.Value.AppendJoin('/', pathParts.Value);

            return sb.Value.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SaveGroup Clear()
        {
            observableObjects.Clear();

            return this;
        }

        public async UniTask<SaveData?> GetSaveDataFromFileAsync(
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            await UniTaskHelper.TrySwitchToThreadPool();

            string cmdName = InvokableNameFactory.Create(
                this,
                nameof(GetSaveDataFromFileAsync)
                );

            var result = new ValueReference<SaveData?>();

            await Command.Builder.SetName(cmdName)
                .WithState((@this: this, configureAwait, result))
                .Asyncronously()
                .SetExecuteAction(
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
            if (!force && IsSaveLoadedFromFile)
                return;

            await UniTaskHelper.TrySwitchToThreadPool();

            string cmdName = InvokableNameFactory.Create(
                this,
                nameof(LoadSaveDataFromFileAsync)
                );

            await Command.Builder.SetName(cmdName)
                .WithState((@this: this, configureAwait, writeSaveDataMode))
                .Asyncronously()
                .SetExecuteAction(
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
            bool configureAwait = true,
            bool forceGet = false,
            CancellationToken cancellationToken = default
            )
        {
            if (forceGet || IsSaveLoadedFromFile)
                return SaveData;

            await LoadSaveDataFromFileAsync(
                writeSaveDataMode,
                configureAwait,
                forceGet,
                cancellationToken
                );

            return SaveData;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ResolveGameObjectKey(GameObject go)
        {
            return go.GetExtraInfo().ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ResolveComponentKey(Component cmp)
        {
            return cmp.GetExtraInfo().ToString();
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
            var loadedSaveData = await GetSaveDataFromFileAsyncCore(
                configureAwait: false,
                cancellationToken
                );

            await UniTaskHelper.TrySwitchToThreadPool();

            try
            {
                if (loadedSaveData is null)
                {
                    SaveData.Write(Array.Empty<SaveUnit>(), writeSaveDataMode);

                    IsSaveLoadedFromFile = true;

                    return;
                }

                SaveData.Write(loadedSaveData.SaveUnits.Values, writeSaveDataMode);

                IsSaveLoadedFromFile = true;
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

        private async UniTask<SaveData> GetOrLoadSaveDataFromFileAsyncCore(
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            CancellationToken cancellationToken = default,
            bool forceGet = false
            )
        {
            await LoadSaveDataFromFileAsyncCore(
                writeSaveDataMode,
                configureAwait,
                cancellationToken
                );

            return SaveData;
        }
    }
}
