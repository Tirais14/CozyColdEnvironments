using CCEnvs.Collections;
using CCEnvs.Threading;
using CCEnvs.Unity.Leaderboards;
using CCEnvs.Unity.Profiles;
using CCEnvs.Unity.UI.Leaderboards;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public sealed class SaveObjectRestorer : IDisposable
    {
        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private readonly CompositeDisposable disposables = new();

        private readonly Dictionary<SaveArchive, CompositeDisposable> archiveDisposables = new();
        private readonly Dictionary<SaveCatalog, CompositeDisposable> catalogDisposables = new();
        private readonly Dictionary<SaveGroup, CompositeDisposable> groupDisposables = new();
        private readonly Dictionary<SaveGroup, SaveData> groupSaveDatas = new();

        private readonly SaveLoaderLazyObjectRestorer lazyObjectRestorer;

        public ObservableDictionary<string, SaveArchive> Archives { get; } = new();

        public SaveObjectRestorer()
        {
            lazyObjectRestorer = new SaveLoaderLazyObjectRestorer(this);

            BindArchiveAdd();
            BindArchiveRemove();
        }

        public void TryRestoreObject(
            object obj,
            string key,
            SaveGroup group,
            object? callbackState = null,
            Action<object?, bool>? callback = null
            )
        {
            ThrowIfDisposed();

            if (obj.IsNull())
            {
                this.PrintError($"Argument: {nameof(obj)} is null");

                callback?.Invoke(callbackState, false);

                return;
            }

            if (key.IsNull())
            {
                this.PrintError($"Argument: {nameof(key)} is null");

                callback?.Invoke(callbackState, false);

                return;
            }

            if (group.IsNull())
            {
                this.PrintError($"Argument: {nameof(group)} is null");

                callback?.Invoke(callbackState, false);

                return;
            }

            switch (obj)
            {
                case MonoBehaviour monoBeh:
                    {
                        lazyObjectRestorer.TryEnqueue(
                            monoBeh,
                            key,
                            group,
                            callbackState,
                            callback
                            );
                    }
                    break;
                default:
                    {
                        var isRestored = TryRestoreObjectCore(obj, key, group);

                        try
                        {
                            callback?.Invoke(callbackState, isRestored);
                        }
                        catch (Exception ex)
                        {
                            this.PrintException(ex);
                        }
                    }
                    break;
            }
        }

        public bool TryAddSaveData(SaveData saveData)
        {
            Guard.IsNotNull(saveData, nameof(saveData));

            return groupSaveDatas.TryAdd(saveData.Group, saveData);
        }

        public bool RemoveSaveData(SaveGroup group, [NotNullWhen(true)] out SaveData? saveData)
        {
            Guard.IsNotNull(group, nameof(group));

            return groupSaveDatas.Remove(group, out saveData);
        }

        public bool RemoveSaveData(SaveGroup group)
        {
            Guard.IsNotNull(group, nameof(group));

            return groupSaveDatas.Remove(group);
        }

        public SaveObjectRestorer ClearSaveDatas()
        {
            groupSaveDatas.Clear();

            return this;
        }

        public SaveObjectRestorer MergeSaveDatas(IEnumerable<SaveData> saveDatas)
        {
            ThrowIfDisposed();

            CC.Guard.IsNotNull(saveDatas, nameof(saveDatas));

            if (saveDatas.IsEmpty())
                return this;

            foreach (var saveData in saveDatas)
            {
                if (groupSaveDatas.TryAdd(saveData.Group, saveData))
                    groupSaveDatas[saveData.Group] = saveData;
            }

            return this;
        }

        public SaveObjectRestorer OverrideSaveDatas(IEnumerable<SaveData> saveDatas)
        {
            ThrowIfDisposed();

            CC.Guard.IsNotNull(saveDatas, nameof(saveDatas));

            groupSaveDatas.Clear();

            if (saveDatas.IsEmpty())
                return this;

            foreach (var saveData in saveDatas)
                groupSaveDatas.Add(saveData.Group, saveData);

            return this;
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            disposeCancellationTokenSource.CancelAndDispose();

            disposables.Dispose();

            archiveDisposables.Values.DisposeEach();
            archiveDisposables.Clear();
            archiveDisposables.TrimExcess();

            catalogDisposables.Values.DisposeEach();
            catalogDisposables.Clear();
            catalogDisposables.TrimExcess();

            groupDisposables.Values.DisposeEach();
            groupDisposables.Clear();
            groupDisposables.TrimExcess();

            groupSaveDatas.Clear();
            groupSaveDatas.TrimExcess();

            lazyObjectRestorer.Dispose();

            Archives.Clear();

            disposed = true;
        }

        internal bool TryRestoreObjectCore(object obj, string objKey, SaveGroup saveGroup)
        {
            if (!groupSaveDatas.TryGetValue(saveGroup, out var saveData)
                ||
                !saveData.SaveEntries.TryGetValue(objKey, out var saveUnit))
            {
                TryCallOnSaveRestoringCallback(obj);

                if (!TryRestoreObjectDefault(obj))
                    return false;

                TryCallOnSaveRestoredCallback(obj);

                return true;
            }

            TryCallOnSaveRestoringCallback(obj);

            try
            {
                if (!saveUnit.Snapshot.TryRestore(obj, out _))
                    return false;
            }
            catch (Exception ex)
            {
                this.PrintException(ex);

                return false;
            }

            TryCallOnSaveRestoredCallback(obj);

            return true;
        }

        private bool TryRestoreObjectDefault(object obj)
        {
            if (obj is not ISaveDefaultCallbackReciever defaultSaveCallbackReciever)
                return false;

            try
            {
                defaultSaveCallbackReciever.OnSaveDefault();

                return true;
            }
            catch (Exception ex)
            {
                this.PrintException(ex);

                return false;
            }
        }

        private void TryCallOnSaveRestoringCallback(object obj)
        {
            if (obj is not ISaveRestoringCallbackReciever restoringCallbackReciever)
                return;

            try
            {
                restoringCallbackReciever.OnSaveRestoring();
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        private void TryCallOnSaveRestoredCallback(object obj)
        {
            if (obj is not ISaveRestoredCallbackReciever restoreCallbackReciever)
                return;

            try
            {
                restoreCallbackReciever.OnSaveRestored();
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        private void OnGroupObjectAdd(
            DictionaryAddEvent<string, object> objEv,
            SaveGroup group
            )
        {
            TryRestoreObject(objEv.Value, objEv.Key, group);
        }

        private void BindGroupObjectAdd(SaveGroup group)
        {
            var disposables = groupDisposables.GetOrCreateNew(group);

            group.ObservableObjects.ObserveDictionaryAdd(disposeCancellationTokenSource.Token)
                .Subscribe(group, OnGroupObjectAdd)
                .AddTo(disposables);
        }

        private void OnCatalogGroupAdd(DictionaryAddEvent<string, SaveGroup> groupEv)
        {
            BindGroupObjectAdd(groupEv.Value);
            groupSaveDatas.Add(groupEv.Value, groupEv.Value.SaveData);
        }

        private void BindCatalogGroupAdd(SaveCatalog catalog)
        {
            var disposables = catalogDisposables.GetOrCreateNew(catalog);

            catalog.Groups.ObserveDictionaryAdd(disposeCancellationTokenSource.Token)
                .Subscribe(OnCatalogGroupAdd)
                .AddTo(disposables);
        }

        private void OnArchiveCatalogAdd(DictionaryAddEvent<string, SaveCatalog> catalogEv)
        {
            BindCatalogGroupAdd(catalogEv.Value);

            DictionaryAddEvent<string, SaveGroup> groupAddEv;

            foreach (var group in catalogEv.Value.Groups)
            {
                groupAddEv = new DictionaryAddEvent<string, SaveGroup>(group.Key, group.Value);

                OnCatalogGroupAdd(groupAddEv);
            }
        }

        private void BindArchiveCatalogAdd(SaveArchive archive)
        {
            var disposables = archiveDisposables.GetOrCreateNew(archive);

            archive.Catalogs.ObserveDictionaryAdd(disposeCancellationTokenSource.Token)
                .Subscribe(OnArchiveCatalogAdd)
                .AddTo(disposables);
        }

        private void OnArchiveAdd(DictionaryAddEvent<string, SaveArchive> addEv)
        {
            BindArchiveCatalogAdd(addEv.Value);

            DictionaryAddEvent<string, SaveCatalog> catalogAddEv;

            foreach (var catalog in addEv.Value.Catalogs)
            {
                catalogAddEv = new DictionaryAddEvent<string, SaveCatalog>(catalog.Key, catalog.Value);

                OnArchiveCatalogAdd(catalogAddEv);
            }
        }

        private void BindArchiveAdd()
        {
            Archives.ObserveDictionaryAdd(disposeCancellationTokenSource.Token)
                .Subscribe(OnArchiveAdd)
                .AddTo(disposables);
        }

        private void OnArchiveRemove(DictionaryRemoveEvent<string, SaveArchive> removeEv)
        {
            if (this.archiveDisposables.Remove(removeEv.Value, out var archiveDisposables))
                archiveDisposables.Dispose();

            foreach (var catalog in removeEv.Value.Catalogs.Values)
            {
                if (this.catalogDisposables.Remove(catalog, out var catalogDisposables))
                    catalogDisposables.Dispose();

                foreach (var group in catalog.Groups.Values)
                {
                    if (this.groupDisposables.Remove(group, out var groupDisposables))
                        groupDisposables.Dispose();
                }
            }
        }

        private void BindArchiveRemove()
        {
            Archives.ObserveDictionaryRemove(disposeCancellationTokenSource.Token)
                .Subscribe(OnArchiveRemove)
                .AddTo(disposables);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }


        private void Foo(LeaderboardView lboardView)
        {
            //UserProfile is IDisposable. Already binded to view. Do not call Dispose manually
            var lboard = lboardView.GetModel<Leaderboard>();

            //UserProfile is IDisposable. In real code you must resolve where it is should dispose
            using var playerProfile = new UserProfile("Tirais", "448d3cf7-8133-4326-b7e5-cac543082ebe");

            //Now the special entry will be display a player profile in the SpecialEntry Property
            lboard.SpecialProfile = playerProfile;

            //UserProfile is IDisposable. In real code you must resolve where it is should dispose
            using var entry = new LeaderboardEntry(playerProfile);

            //Thus names will be used in the inspectors LeaderboardEntryView ScoreRecordViews field
            entry.AddScoreRecord("kills").AddScoreRecord("deaths");

            entry.AddScore("kills", 1);
        }
    }
}
