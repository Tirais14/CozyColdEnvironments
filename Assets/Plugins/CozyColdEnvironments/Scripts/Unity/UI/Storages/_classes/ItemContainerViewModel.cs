using CCEnvs.Disposables;
using CCEnvs.Unity.Storages;
using CCEnvs.Unity.UI.MVVM;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public class ItemContainerViewModel<T>
        : ViewModel<T>,
        IItemContainerViewModel<T>

        where T : IItemContainer
    {
        private readonly ReactiveProperty<Sprite?> itemIcon = new();
        private readonly ReactiveProperty<bool> itemIconVisible = new();
        private readonly ReactiveProperty<string> itemCount = new();
        private readonly ReactiveProperty<bool> itemCountEnabled = new();

        public IReadOnlyReactiveProperty<Sprite?> ItemIcon => itemIcon;
        public IReadOnlyReactiveProperty<bool> ItemIconVisible => itemIconVisible;
        public IReadOnlyReactiveProperty<string> ItemCount => itemCount;
        public IReadOnlyReactiveProperty<bool> ItemCountVisible => itemCountEnabled;
        public IReadOnlyReactiveProperty<bool> IsActiveContainer => model.IsActiveContainer;

        public ItemContainerViewModel(T model, GameObject gameObject)
            :
            base(model, gameObject)
        {
            //gameObject.FindFor
            BindItemIcon();
            BindItemCount();
        }

        private void BindItemIcon()
        {
            model.Item.SubscribeWithState(itemIcon, static (x, prop) => prop.Value = x.Map(item => item.Icon).Access())
                      .AddTo(disposables);

            model.Item.Select(x => x.IsSome)
                      .SubscribeWithState(itemIconVisible, static (state, prop) => prop.Value = state)
                      .AddTo(disposables);
        }

        private void BindItemCount()
        {
            model.ItemCount.SubscribeWithState(itemCount, static (x, prop) => prop.Value = x.ToString())
                           .AddTo(disposables);

            model.ItemCount.Select(x => x > 0)
                           .SubscribeWithState(itemCountEnabled, static (state, prop) => prop.Value = state)
                           .AddTo(disposables);
        }
    }
}
