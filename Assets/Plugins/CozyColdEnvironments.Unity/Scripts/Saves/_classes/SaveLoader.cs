using CCEnvs.Collections;
using CCEnvs.Snapshots;
using CCEnvs.Threading;
using CCEnvs.Unity.Async;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
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
        private readonly Dictionary<(SaveGroup saveGroup, string objectKey), CancellationTokenSource> objectRestoreTaskTokens = new();
        private readonly Dictionary<SaveGroup, SaveData> groupSaveDatas = new();

        public IReadOnlyObservableDictionary<string, SaveArchive> Archives => archives;

        public SaveLoader()
        {
            BindArchiveAdd();
            BindArchiveRemove();
        }

        public async UniTask<bool> TryRestoreObjectAsync(
            object obj,
            string key,
            SaveGroup group,
            CancellationToken cancellationToken = default
            )
        {
            if (obj.IsNull())
                return false;

            if (objectRestoreTaskTokens.Remove((group, key), out var previousTaskCancellationTokenSource))
                previousTaskCancellationTokenSource.CancelAndDispose();

            var cancellationTokenSource = disposeCancellationTokenSource.Token.TryLinkTokens(cancellationToken, out cancellationToken);

            if (cancellationTokenSource is not null)
                objectRestoreTaskTokens.Add((group, key), cancellationTokenSource);

            try
            {
                return obj switch
                {
                    MonoBehaviour mono => await TryRestoreMonoBehaviourAsync(mono, key, group, cancellationToken),
                    _ => TryRestoreObjectCore(obj, key, group),
                };
            }
            finally
            {
                cancellationTokenSource?.CancelAndDispose();
            }
        }

        private bool TryRestoreObjectCore(object obj, string objKey, SaveGroup saveGroup)
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
            CancellationToken cancellationToken)
        {
            

            if (mono.didStart)
                return TryRestoreObjectCore(mono, key, group);

            await UniTask.SwitchToThreadPool();

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

            await UniTask.SwitchToMainThread();

            if (!waitingSucces)
                return false;

            return TryRestoreObjectCore(mono, key, group);
        }

        private void OnGroupObjectAdd(
            DictionaryAddEvent<string, object> objEv,
            SaveGroup group
            )
        {
            TryRestoreObjectAsync(objEv.Value, objEv.Key, group).ForgetByPrintException();
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
