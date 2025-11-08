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
        private readonly ReactiveProperty<Sprite> itemIcon = new(initialValue: UCC.TranparentSprite.Value);
        private readonly ReactiveProperty<string> itemCount = new(initialValue: string.Empty);

        public IReadOnlyReactiveProperty<Sprite> ItemIcon => itemIcon;
        public IReadOnlyReactiveProperty<string> ItemCount => itemCount;
        public IReadOnlyReactiveProperty<bool> IsActiveContainer { get; }

        public ItemContainerViewModel(T model, GameObject gameObject)
            :
            base(model, gameObject)
        {
            BindItemIcon();
            BindItemCount();
            IsActiveContainer = model.ObserveIsActiveContainer().ToReactiveProperty();
        }

        public void ActivateContainer()
        {
            model.ActivateContainer();
        }

        public void DeactivateContainer()
        {
            model.DeactivateContainer();
        }

        private void BindItemIcon()
        {
            model.ObserveItem()
                .SubscribeWithState(itemIcon,
                    static (x, prop) => prop.Value = x.Map(item => item.Icon)
                        .Access(UCC.TranparentSprite.Value))
                .AddTo(disposables);

            itemIcon.AddTo(disposables);
        }

        private void BindItemCount()
        {
            model.ObserveItemCount()
                .Select(itemCount => itemCount <= 0 ? string.Empty : itemCount.ToString())
                .SubscribeWithState(itemCount, 
                    static (countStr, prop) => prop.Value = countStr)
                .AddTo(disposables);

            itemCount.AddTo(disposables);
        }
    }
}
