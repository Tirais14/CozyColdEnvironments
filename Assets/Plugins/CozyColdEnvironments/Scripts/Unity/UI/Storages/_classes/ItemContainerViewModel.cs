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
        private readonly ReactiveProperty<Sprite> itemIcon = new();
        private readonly ReactiveProperty<string> itemCount = new();

        public IReadOnlyReactiveProperty<Sprite> ItemIcon => itemIcon;
        public IReadOnlyReactiveProperty<string> ItemCount => itemCount;
        public IReadOnlyReactiveProperty<bool> IsActiveContainer => model.IsActiveContainer;

        public ItemContainerViewModel(T model, GameObject gameObject)
            :
            base(model, gameObject)
        {
            BindItemIcon();
            BindItemCount();
        }

        private void BindItemIcon()
        {
            model.Item.SubscribeWithState(itemIcon, static (x, prop) => prop.Value = x.Map(item => item.Icon).Access(UCC.TranparentSprite.Value))
                      .AddTo(disposables);
        }

        private void BindItemCount()
        {
            model.ItemCount.Select(itemCount => itemCount <= 0 ? string.Empty : itemCount.ToString())
                           .SubscribeWithState(itemCount, static (countStr, prop) => prop.Value = countStr)
                           .AddTo(disposables);
        }
    }
}
