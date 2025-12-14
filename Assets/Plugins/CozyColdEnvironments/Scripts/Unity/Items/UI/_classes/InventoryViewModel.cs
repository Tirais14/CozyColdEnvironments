using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using System;
using System.Collections.Generic;
using R3;
using ObservableCollections;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public class InventoryViewModel<TModel> : ViewModel<TModel>, IInventoryViewModel

        where TModel : IInventory
    {
        private readonly ReactiveCommand<KeyValuePair<int, IItemContainer>> add = new();
        private readonly ReactiveCommand<int> remove = new();
        private readonly ReactiveCommand<KeyValuePair<int, IItemContainer>> replace = new();

        public ReactiveCommand<KeyValuePair<int, IItemContainer>> Add => add;
        public ReactiveCommand<int> Remove => remove;
        public ReactiveCommand<KeyValuePair<int, IItemContainer>> Replace => replace;

        public InventoryViewModel(TModel model) 
            :
            base(model)
        {
            InstallBindings();
        }

        public Observable<DictionaryAddEvent<int, IItemContainer>> ObserveAddContainer()
        {
            return model.ObserveAddContainer();
        }

        public Observable<DictionaryRemoveEvent<int, IItemContainer>> ObserveRemoveContainer()
        {
            return model.ObserveRemoveContainer();
        }

        public Observable<DictionaryReplaceEvent<int, IItemContainer>> ObserveReplaceContainer()
        {
            return model.ObserveReplaceContainer();
        }

        public Observable<CollectionResetEvent<KeyValuePair<int, IItemContainer>>> ObserveResetContainer()
        {
            return model.ObserveReset();
        }

        private void InstallBindings()
        {
            add.Subscribe(this,
                    static (cnt, @this) => @this.model.AddContainer(cnt.Key, cnt.Value))
                .AddTo(disposables);

            remove.Subscribe(this,
                    static (id, @this) => @this.model.RemoveContainer(id))
                .AddTo(disposables);

            replace.Subscribe(this,
                    static (cnt, @this) => @this.model.To<IDictionary<int, IItemContainer>>()[cnt.Key] = cnt.Value)
                .AddTo(disposables);
        }
    }
}
