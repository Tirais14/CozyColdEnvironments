using CCEnvs.Collections;
using CCEnvs.Snapshots;
using CCEnvs.Threading;
using CCEnvs.Unity.Async;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public class SaveLoader
    {
        private readonly ObservableDictionary<string, SaveArchive> archives = new();

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private readonly CompositeDisposable disposables = new();

        private readonly Dictionary<SaveArchive, CompositeDisposable> archiveDisposables = new();
        private readonly Dictionary<SaveGroupCatalog, CompositeDisposable> catalogDisposables = new();
        private readonly Dictionary<SaveGroup, CompositeDisposable> groupDisposables = new();
        private readonly Dictionary<SaveGroup, SaveData> groupSaveDatas = new();

        private readonly SaveLoaderLazyObjectRestorer lazyObjectRestorer;

        public IReadOnlyObservableDictionary<string, SaveArchive> Archives => archives;

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
            Guard.IsNotNull(callback, nameof(callback));

            if (obj.IsNull())
            {
                callback?.Invoke(callbackState, false);
                return;
            }

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

            if (!saveUnit.Snapshot.TryRestore(obj, out _))
                return false;

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
            catch (System.Exception ex)
            {
                this.PrintException(ex);

                return false;
            }
        }

        private bool TryCallOnSaveRestoringCallback(object obj)
        {
            if (obj is not ISaveRestoringCallbackReciever restoringCallbackReciever)
                return false; ;

            try
            {
                restoringCallbackReciever.OnSaveRestoring();

                return true;
            }
            catch (System.Exception ex)
            {
                this.PrintException(ex);

                return false;
            }
        }

        private bool TryCallOnSaveRestoredCallback(object obj)
        {
            if (obj is not ISaveRestoredCallbackReciever restoreCallbackReciever)
                return false;

            try
            {
                restoreCallbackReciever.OnSaveRestored();

                return true;
            }
            catch (System.Exception ex)
            {
                this.PrintException(ex);

                return false;
            }
        }

        private async UniTask<bool> TryRestoreMonoBehaviourAsync(
            MonoBehaviour mono, 
            string key, 
            SaveGroup group,
            CancellationToken cancellationToken
            )
        {
            

            if (mono.didStart)
                return TryRestoreObjectCore(mono, key, group);

            //TODO: Create queue with Mono-s and separate loop, which indicates is Mono.didStart

            var waitingSucces = await UniTask.WaitUntil(
                mono,
                static mono =>
                {
                    return mono.didStart;
                },
                timing: PlayerLoopTiming.EarlyUpdate,
                cancellationToken: cancellationToken
                )
                .SuppressCancellationThrow();

            if (!waitingSucces)
                return false;

            return TryRestoreObjectCore(mono, key, group);
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

        private void BindCatalogGroupAdd(SaveGroupCatalog catalog)
        {
            var disposables = catalogDisposables.GetOrCreateNew(catalog);

            catalog.Groups.ObserveDictionaryAdd(disposeCancellationTokenSource.Token)
                .Subscribe(OnCatalogGroupAdd)
                .AddTo(disposables);
        }

        private void OnArchiveCatalogAdd(DictionaryAddEvent<string, SaveGroupCatalog> catalogEv)
        {
            BindCatalogGroupAdd(catalogEv.Value);
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
        }

        private void BindArchiveAdd()
        {
            archives.ObserveDictionaryAdd(disposeCancellationTokenSource.Token)
                .Subscribe(OnArchiveAdd)
                .AddTo(disposables);
        }

        private void OnArchiveRemove(DictionaryRemoveEvent<string, SaveArchive> removeEv)
        {

        }

        private void BindArchiveRemove()
        {
            archives.ObserveDictionaryRemove(disposeCancellationTokenSource.Token)
                .Subscribe(OnArchiveRemove)
                .AddTo(disposables);
        }
    }
}
