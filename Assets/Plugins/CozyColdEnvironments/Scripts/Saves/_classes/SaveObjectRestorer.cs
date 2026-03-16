using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Threading;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Humanizer;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

        private CancellationToken disposeToken => disposeCancellationTokenSource.Token;

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

            if (groupName.IsNull())
            {
                this.PrintError($"Argument: {nameof(groupName)} is null");

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
                            groupName,
                            callbackState,
                            callback
                            );
                    }
                    break;
#endif
                default:
                    {
                        var isRestored = TryRestoreObjectCore(obj, key, groupName);

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
            TryRestoreObject(objEv.Value, objEv.Key, group.Name);
        }

        private void BindGroupObjectAdd(SaveGroup group)
        {
            var disposables = groupDisposables.GetOrCreateNew(group.Name);

            group.ObservableObjects.ObserveDictionaryAdd(disposeToken)
                .Subscribe(group, OnGroupObjectAdd)
                .AddTo(disposables);
        }

        private void OnGroupLoaded(SaveGroup group)
        {
            foreach (var (key, obj) in group)
                TryRestoreObject(obj, key, group.Name);
        }

        private void BindGroupLoaded(SaveGroup group)
        {
            var disposables = groupDisposables.GetOrCreateNew(group.Name);

            group.Loader.ObserveLoadSaveData()
                .Subscribe((@this: this, group),
                static (saveData, args) =>
                {
                    args.@this.OnGroupLoaded(args.group);
                })
                .AddTo(disposables);
        }

        private void OnCatalogGroupAdd(DictionaryAddEvent<string, SaveGroup> addEv)
        {
            var group = addEv.Value;

            BindGroupObjectAdd(group);
            groupSaveDatas.Add(group.Name, group.SaveData);
            BindGroupLoaded(group);
        }

        private void OnCatalogIncrementalGroupAdd(DictionaryAddEvent<string, SaveGroupIncremental> addEv)
        {
            OnCatalogGroupAdd(new DictionaryAddEvent<string, SaveGroup>(addEv.Key, addEv.Value));
        }

        private void BindCatalogGroupAdd(SaveCatalog catalog)
        {
            var disposables = catalogDisposables.GetOrCreateNew(catalog.Path);

            catalog.Groups.ObserveDictionaryAdd(disposeToken)
                .Subscribe(OnCatalogGroupAdd)
                .AddTo(disposables);

            catalog.IncrementalGroups.ObserveDictionaryAdd(disposeToken)
                .Subscribe(OnCatalogIncrementalGroupAdd)
                .AddTo(disposables);
        }

        private void OnArchiveCatalogAdd(DictionaryAddEvent<string, SaveCatalog> addEv)
        {
            var catalog = addEv.Value;

            BindCatalogGroupAdd(catalog);

            DictionaryAddEvent<string, SaveGroup> groupAddEv;

            foreach (var group in catalog)
            {
                groupAddEv = new DictionaryAddEvent<string, SaveGroup>(group.Name, group);

                OnCatalogGroupAdd(groupAddEv);
            }
        }

        private void BindArchiveCatalogAdd(SaveArchive archive)
        {
            var disposables = archiveDisposables.GetOrCreateNew(archive.Path);

            archive.Catalogs.ObserveDictionaryAdd(disposeToken)
                .Subscribe(OnArchiveCatalogAdd)
                .AddTo(disposables);
        }

        private void OnArchiveAdd(DictionaryAddEvent<string, SaveArchive> addEv)
        {
            var archive = addEv.Value;

            BindArchiveCatalogAdd(archive);

            DictionaryAddEvent<string, SaveCatalog> catalogAddEv;

            foreach (var catalog in archive)
            {
                catalogAddEv = new DictionaryAddEvent<string, SaveCatalog>(catalog.Path, catalog);

                OnArchiveCatalogAdd(catalogAddEv);
            }
        }

        private void BindArchiveAdd()
        {
            Archives.ObserveDictionaryAdd(disposeToken)
                .Subscribe(OnArchiveAdd)
                .AddTo(disposables);
        }

        private void OnCatalogGroupRemove(DictionaryRemoveEvent<string, SaveGroup> removeEv)
        {
            var group = removeEv.Value;

            if (groupDisposables.Remove(group.Name, out var disposables))
                disposables.Dispose();

            groupSaveDatas.Remove(group.Name);
        }

        private void OnCatalogIncrementalGroupRemove(DictionaryRemoveEvent<string, SaveGroupIncremental> removeEv)
        {
            OnCatalogGroupRemove(new DictionaryRemoveEvent<string, SaveGroup>(removeEv.Key, removeEv.Value));
        }

        private void BindCatalogGroupRemove(SaveCatalog catalog)
        {
            var disposables = catalogDisposables.GetOrCreateNew(catalog.Path);

            catalog.Groups.ObserveDictionaryRemove(disposeToken)
                .Subscribe(OnCatalogGroupRemove)
                .AddTo(disposables);
        }

        private void OnArchiveCatalogRemove(DictionaryRemoveEvent<string, SaveCatalog> removeEv)
        {
            var catalog = removeEv.Value;

            if (catalogDisposables.Remove(catalog.Path, out var disposables))
                disposables.Dispose();

            foreach (var group in catalog)
                OnCatalogGroupRemove(new DictionaryRemoveEvent<string, SaveGroup>(group.Name, group));
        }

        private void BindArchiveCatalogRemove(SaveArchive archive)
        {
            var disposables = archiveDisposables.GetOrCreateNew(archive.Path);

            archive.Catalogs.ObserveDictionaryRemove(disposeToken)
                .Subscribe(OnArchiveCatalogRemove)
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

        private void OnArchiveRemove(DictionaryRemoveEvent<string, SaveArchive> removeEv)
        {
            if (this.archiveDisposables.Remove(removeEv.Value.Path, out var archiveDisposables))
                archiveDisposables.Dispose();

            var archive = removeEv.Value;

            foreach (var catalog in archive)
                OnArchiveCatalogRemove(new DictionaryRemoveEvent<string, SaveCatalog>(catalog.Path, catalog));
        }

        private void BindArchiveRemove()
        {
            Archives.ObserveDictionaryRemove(disposeToken)
                .Subscribe(OnArchiveRemove)
                .AddTo(disposables);
        }

        private void OnArchiveReplace(DictionaryReplaceEvent<string, SaveArchive> replaceEv)
        {
            var removeEv = new DictionaryRemoveEvent<string, SaveArchive>(replaceEv.Key, replaceEv.OldValue);
            OnArchiveRemove(removeEv);

            var addEv = new DictionaryAddEvent<string, SaveArchive>(replaceEv.Key, replaceEv.NewValue);
            OnArchiveAdd(addEv);
        }

        private void BindArchivesClear()
        {
            Archives.ObserveClear(disposeToken)
                .Subscribe(OnArchivesClear)
                .AddTo(disposables);
        }

        private void BindArchiveReplace()
        {
            Archives.ObserveDictionaryReplace(disposeToken)
                .Subscribe(OnArchiveReplace)
                .AddTo(disposables);
        }
    }
}
