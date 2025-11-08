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
        /// <summary>
        /// To prevent endless loop in some actions
        /// </summary>
        private readonly ReactiveProperty<bool> toModel = new(initialValue: false);

        private readonly ReactiveProperty<Sprite> itemIcon = new(initialValue: UCC.TranparentSprite.Value);
        private readonly ReactiveProperty<string> itemCount = new(initialValue: string.Empty);
        private readonly ReactiveCommand<bool> isActiveContainer;

        public IReadOnlyReactiveProperty<Sprite> ItemIcon => itemIcon;
        public IReadOnlyReactiveProperty<string> ItemCount => itemCount;
        public IReactiveCommand<bool> IsActiveContainer => isActiveContainer;

        public ItemContainerViewModel(T model, GameObject gameObject)
            :
            base(model, gameObject)
        {
            BindItemIcon();
            BindItemCount();

            isActiveContainer = new ReactiveCommand<bool>(toModel,
                initialValue: false);

            BindIsActiveContainer();
        }

        public void ActivateContainer()
        {
            toModel.Value = true;
            model.ActivateContainer();
            toModel.Value = false;
        }

        private void BindItemIcon()
        {
            model.Item.SubscribeWithState(itemIcon,
                      static (x, prop) => prop.Value = x.Map(item => item.Icon)
                        .Access(UCC.TranparentSprite.Value))
                      .AddTo(disposables);
        }

        private void BindItemCount()
        {
            model.ItemCount.Select(itemCount => itemCount <= 0 ? string.Empty : itemCount.ToString())
                           .SubscribeWithState(itemCount, 
                           static (countStr, prop) => prop.Value = countStr)
                           .AddTo(disposables);
        }

        private void BindIsActiveContainer()
        {
            model.IsActiveContainer.SubscribeWithState(isActiveContainer,
                                   static (state, prop) => prop.Execute(state))
                                   .AddTo(disposables);
        }
    }
}
