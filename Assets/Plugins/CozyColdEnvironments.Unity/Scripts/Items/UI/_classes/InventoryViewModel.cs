using System.Collections.Generic;
using System.Threading;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using ObservableCollections;
using R3;

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

        public InventoryViewModel(TModel model, CancellationToken cancellationToken)
            :
            base(model, cancellationToken)
        {
            InstallBindings();
        }

        public Observable<DictionaryAddEvent<int, IItemContainer>> ObserveAdd()
        {
            return model.ObserveAddContainer();
        }

        public Observable<DictionaryRemoveEvent<int, IItemContainer>> ObserveRemove()
        {
            return model.ObserveRemoveContainer();
        }

        public Observable<DictionaryReplaceEvent<int, IItemContainer>> ObserveReplace()
        {
            return model.ObserveReplaceContainer();
        }

        public Observable<CollectionResetEvent<KeyValuePair<int, IItemContainer>>> ObserveReset()
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
