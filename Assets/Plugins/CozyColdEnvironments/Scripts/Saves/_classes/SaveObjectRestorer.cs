using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Threading;
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
namespace CCEnvs.Saves
{
    public sealed class SaveObjectRestorer : IDisposable
    {
        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private readonly CompositeDisposable disposables = new();

        private readonly Dictionary<string, CompositeDisposable> archiveDisposables = new();
        private readonly Dictionary<string, CompositeDisposable> catalogDisposables = new();
        private readonly Dictionary<string, CompositeDisposable> groupDisposables = new();
        private readonly Dictionary<string, SaveData> groupSaveDatas = new();

        private readonly SaveLoaderLazyObjectRestorer lazyObjectRestorer;

        public ObservableDictionary<string, SaveArchive> Archives { get; } = new();

        public SaveObjectRestorer()
        {
            lazyObjectRestorer = new SaveLoaderLazyObjectRestorer(this);

            BindArchiveAdd();
            BindArchiveRemove();
            BindArchivesClear();
        }

        public void TryRestoreObject(
            object obj,
            string key,
            string groupName,
            object? callbackState = null,
            Action<object?, bool>? callback = null
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

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
#if UNITY_2017_1_OR_NEWER
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
#endif
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

            return groupSaveDatas.TryAdd(saveData.GroupName, saveData);
        }

        public bool RemoveSaveData(SaveGroup group, [NotNullWhen(true)] out SaveData? saveData)
        {
            Guard.IsNotNull(group, nameof(group));

            return groupSaveDatas.Remove(group.Name, out saveData);
        }

        public bool RemoveSaveData(SaveGroup group)
        {
            Guard.IsNotNull(group, nameof(group));

            return groupSaveDatas.Remove(group.Name);
        }

        public SaveObjectRestorer ClearSaveDatas()
        {
            groupSaveDatas.Clear();

            return this;
        }

        public SaveObjectRestorer ClearArchives()
        {
            Archives.Clear();

            return this;
        }

        public SaveObjectRestorer MergeSaveDatas(IEnumerable<SaveData> saveDatas)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            CC.Guard.IsNotNull(saveDatas, nameof(saveDatas));

            if (saveDatas.IsEmpty())
                return this;

            foreach (var saveData in saveDatas)
            {
                if (groupSaveDatas.TryAdd(saveData.GroupName, saveData))
                    groupSaveDatas[saveData.GroupName] = saveData;
            }

            return this;
        }

        public SaveObjectRestorer OverrideSaveDatas(IEnumerable<SaveData> saveDatas)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            CC.Guard.IsNotNull(saveDatas, nameof(saveDatas));

            groupSaveDatas.Clear();

            if (saveDatas.IsEmpty())
                return this;

            foreach (var saveData in saveDatas)
                groupSaveDatas.Add(saveData.GroupName, saveData);

            return this;
        }

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            disposeCancellationTokenSource.CancelAndDispose();

            try
            {
                Archives.Clear();
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            lazyObjectRestorer.Dispose();

            disposables.Dispose();
        }

        internal bool TryRestoreObjectCore(object obj, string objKey, string groupName)
        {
            if (!groupSaveDatas.TryGetValue(groupName, out var saveData)
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
            var disposables = groupDisposables.GetOrCreateNew(group.Name);

            group.ObservableObjects.ObserveDictionaryAdd(disposeCancellationTokenSource.Token)
                .Subscribe(group, OnGroupObjectAdd)
                .AddTo(disposables);
        }

        private void OnSaveDataLoaded(SaveGroup group)
        {
            lock (group.ObservableObjects.SyncRoot)
            {
                foreach (var (key, obj) in group.ObservableObjects)
                    TryRestoreObject(obj, key, group.Name);
            }
        }

        private void BindSaveDataLoaded(SaveGroup group)
        {
            group.ObserveLoadSaveData()
                .Subscribe((@this: this, group),
                static (saveData, args) =>
                {
                    args.@this.OnSaveDataLoaded(args.group);
                })
                .AddTo(disposables);
        }

        private void OnCatalogGroupAdd(DictionaryAddEvent<string, SaveGroup> addEv)
        {
            var group = addEv.Value;

            BindGroupObjectAdd(group);
            groupSaveDatas.Add(group.Name, group.SaveData);
            BindSaveDataLoaded(group);
        }

        private void BindCatalogGroupAdd(SaveCatalog catalog)
        {
            var disposables = catalogDisposables.GetOrCreateNew(catalog.Path);

            catalog.Groups.ObserveDictionaryAdd(disposeCancellationTokenSource.Token)
                .Subscribe(OnCatalogGroupAdd)
                .AddTo(disposables);
        }

        private void OnArchiveCatalogAdd(DictionaryAddEvent<string, SaveCatalog> addEv)
        {
            var catalog = addEv.Value;

            BindCatalogGroupAdd(catalog);

            DictionaryAddEvent<string, SaveGroup> groupAddEv;

            foreach (var (groupKey, group) in catalog.Groups)
            {
                groupAddEv = new DictionaryAddEvent<string, SaveGroup>(groupKey, group);

                OnCatalogGroupAdd(groupAddEv);
            }
        }

        private void BindArchiveCatalogAdd(SaveArchive archive)
        {
            var disposables = archiveDisposables.GetOrCreateNew(archive.Path);

            archive.Catalogs.ObserveDictionaryAdd(disposeCancellationTokenSource.Token)
                .Subscribe(OnArchiveCatalogAdd)
                .AddTo(disposables);
        }

        private void OnArchiveAdd(DictionaryAddEvent<string, SaveArchive> addEv)
        {
            var archive = addEv.Value;

            BindArchiveCatalogAdd(archive);

            DictionaryAddEvent<string, SaveCatalog> catalogAddEv;

            foreach (var (ctlgKey, ctlg) in archive.Catalogs)
            {
                catalogAddEv = new DictionaryAddEvent<string, SaveCatalog>(ctlgKey, ctlg);

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
            if (this.archiveDisposables.Remove(removeEv.Value.Path, out var archiveDisposables))
                archiveDisposables.Dispose();

            var archive = removeEv.Value;

            lock (archive.Catalogs.SyncRoot)
            {
                foreach (var catalog in archive.Catalogs.Values)
                {
                    if (this.catalogDisposables.Remove(catalog.Path, out var catalogDisposables))
                        catalogDisposables.Dispose();

                    lock (catalog.Groups.SyncRoot)
                    {
                        foreach (var group in catalog.Groups.Values)
                        {
                            if (this.groupDisposables.Remove(group.Name, out var groupDisposables))
                                groupDisposables.Dispose();
                        }
                    }
                }
            }
        }

        private void BindArchiveRemove()
        {
            Archives.ObserveDictionaryRemove(disposeCancellationTokenSource.Token)
                .Subscribe(OnArchiveRemove)
                .AddTo(disposables);
        }

        private void OnArchivesClear(Unit _)
        {
            archiveDisposables.Values.DisposeEach();
            archiveDisposables.Clear();

            catalogDisposables.Values.DisposeEach();
            catalogDisposables.Clear();
            catalogDisposables.TrimExcess();

            groupDisposables.Values.DisposeEach();
            groupDisposables.Clear();
            groupDisposables.TrimExcess();

            groupSaveDatas.Clear();
            groupSaveDatas.TrimExcess();
        }

        private void BindArchivesClear()
        {
            Archives.ObserveClear(disposeCancellationTokenSource.Token)
                .Subscribe(OnArchivesClear)
                .AddTo(disposables);
        }

        private void OnArchiveReplace(DictionaryReplaceEvent<string, SaveArchive> replaceEv)
        {
            var removeEv = new DictionaryRemoveEvent<string, SaveArchive>(replaceEv.Key, replaceEv.OldValue);
            OnArchiveRemove(removeEv);

            var addEv = new DictionaryAddEvent<string, SaveArchive>(replaceEv.Key, replaceEv.NewValue);
            OnArchiveAdd(addEv);
        }

        private void BindArchiveReplace()
        {
            Archives.ObserveDictionaryReplace(disposeCancellationTokenSource.Token)
                .Subscribe(OnArchiveReplace)
                .AddTo(disposables);
        }
    }
}
