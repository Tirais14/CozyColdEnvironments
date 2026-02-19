using CCEnvs.Collections;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using System.Collections.Generic;
using System.Threading;

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

        public IReadOnlyObservableDictionary<string, SaveArchive> Archives => archives;

        public SaveLoader()
        {
            BindArchiveAdd();
            BindArchiveRemove();
        }

        public async UniTask<bool> TryRestoreObjectAsync(object obj, string key)
        {
            if (obj.IsNull())
                return false;

            //TODO:

            return true;
        }

        private void OnGroupObjectAdd(DictionaryAddEvent<string, object> objEv)
        {
            TryRestoreObjectAsync(objEv.Value, objEv.Key);
        }

        private void BindGroupObjectAdd(SaveGroup group)
        {
            var disposables = groupDisposables.GetOrCreateNew(group);

            group.ObservableObjects.ObserveDictionaryAdd(disposeCancellationTokenSource.Token)
                .Subscribe(OnGroupObjectAdd)
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
