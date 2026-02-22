using CCEnvs.Collections;
using CCEnvs.Threading;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public sealed class SaveLoader : IDisposable
    {
        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private readonly CompositeDisposable disposables = new();

        private readonly Dictionary<SaveArchive, CompositeDisposable> archiveDisposables = new(SaveArchive.PathEqualityComparer.Instance);
        private readonly Dictionary<SaveCatalog, CompositeDisposable> catalogDisposables = new(SaveCatalog.PathArchiveEqualityComparer.Instance);
        private readonly Dictionary<SaveGroup, CompositeDisposable> groupDisposables = new(SaveGroup.NameCatalogEqualityComparer.Instance);
        private readonly Dictionary<SaveGroup, SaveData> groupSaveDatas = new(SaveGroup.NameCatalogEqualityComparer.Instance);

        private readonly SaveLoaderLazyObjectRestorer lazyObjectRestorer;

        public ObservableDictionary<string, SaveArchive> Archives { get; } = new();

        public SaveLoader()
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

            CC.Guard.IsNotNull(obj, nameof(obj));
            Guard.IsNotNull(key, nameof(key));
            Guard.IsNotNull(group, nameof(group));

            switch (obj)
            {
                case MonoBehaviour monoBeh:
                    {
                        lazyObjectRestorer.Enqueue(
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

        public void OverrideSaveDatas(IEnumerable<(SaveGroup Group, SaveData Value)> saveDatas)
        {
            CC.Guard.IsNotNull(saveDatas, nameof(saveDatas));

            if (saveDatas.IsEmpty())
                return;

            foreach (var (group, saveData) in saveDatas)
            {
                if (!groupSaveDatas.TryAdd(group, saveData))
                    groupSaveDatas[group] = saveData;    
            }
        }

        public (SaveGroup Group, SaveData SaveData)[] DeserializeSaveData(string serialized)
        {
            CC.Guard.IsNotNull(serialized, nameof(serialized));

            if (serialized.IsNullOrWhiteSpace())
                return new arr<(SaveGroup Group, SaveData SaveData)>();

            JsonConvert.DeserializeObject<(SaveGroup Group, SaveData SaveData)[]>(serialized);
        }

        public void LoadSaveDatas()
        {

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

            lazyObjectRestorer.Dispose();

            Archives.Clear();

            disposed = true;
        }

        internal bool TryRestoreObjectCore(object obj, string objKey, SaveGroup saveGroup)
        {
            if (!groupSaveDatas.TryGetValue(saveGroup, out var saveData)
                ||
                !saveData.SaveUnits.TryGetValue(objKey, out var saveUnit))
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
    }
}
