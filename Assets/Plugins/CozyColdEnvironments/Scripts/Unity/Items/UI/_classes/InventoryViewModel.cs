using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using System;
using System.Collections.Generic;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public class InventoryViewModel<TModel> : ViewModel<TModel>, IInventoryViewModel

        where TModel : IInventory
    {
        private readonly ReactiveCommand<KeyValuePair<int, IItemContainer>> add = new();
        private readonly ReactiveCommand<int> remove = new();
        private readonly ReactiveCommand<KeyValuePair<int, IItemContainer>> replace = new();

        public IReactiveCommand<KeyValuePair<int, IItemContainer>> Add => add;
        public IReactiveCommand<int> Remove => remove;
        public IReactiveCommand<KeyValuePair<int, IItemContainer>> Replace => replace;

        public InventoryViewModel(TModel model) 
            :
            base(model)
        {
            InstallBindings();
        }

        public IObservable<DictionaryAddEvent<int, IItemContainer>> ObserveAdd()
        {
            return model.ObserveAdd();
        }

        public IObservable<DictionaryRemoveEvent<int, IItemContainer>> ObserveRemove()
        {
            return model.ObserveRemove();
        }

        public IObservable<DictionaryReplaceEvent<int, IItemContainer>> ObserveReplace()
        {
            return model.ObserveReplace();
        }

        public IObservable<Unit> ObserveReset()
        {
            return model.ObserveReset();
        }

        private void InstallBindings()
        {
            add.SubscribeWithState(this,
                    static (cnt, @this) => @this.model.Add(cnt.Key, cnt.Value))
                .AddTo(disposables);

            remove.SubscribeWithState(this,
                    static (id, @this) => @this.model.Remove(id))
                .AddTo(disposables);

            replace.SubscribeWithState(this,
                    static (cnt, @this) => @this.model.As<IDictionary<int, IItemContainer>>()[cnt.Key] = cnt.Value)
                .AddTo(disposables);
        }
    }
}
