using CCEnvs.Disposables;
using CCEnvs.FuncLanguage;
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
        private readonly ReactiveProperty<Sprite?> itemIconView = new();
        private readonly ReactiveProperty<bool> itemIconVisible = new();
        private readonly ReactiveProperty<string> itemCountView = new();
        private readonly ReactiveProperty<bool> itemCountEnabled = new();

        public IReadOnlyReactiveProperty<Sprite?> ItemIconView => itemIconView;
        public IReadOnlyReactiveProperty<bool> ItemIconVisible => itemIconVisible;
        public IReadOnlyReactiveProperty<string> ItemCountView => itemCountView;
        public IReadOnlyReactiveProperty<bool> ItemCountVisible => itemCountEnabled;

        public ItemContainerViewModel(T model, GameObject gameObject)
            :
            base(model, gameObject)
        {
            BindItemIcon();
            BindItemCount();
        }

        private void BindItemIcon()
        {
            model.Item.Subscribe(x => itemIconView.Value = x.Map(item => item.Icon).Access())
                      .AddTo(this);

            model.Item.Select(x => x.IsSome)
                      .Subscribe(state => itemIconVisible.Value = state)
                      .AddTo(this);
        }

        private void BindItemCount()
        {
            model.ItemCount.Subscribe(x => itemCountView.Value = x.ToString())
                           .AddTo(this);

            model.ItemCount.Select(x => x > 0)
                           .Subscribe(state => itemCountEnabled.Value = state)
                           .AddTo(this);
        }
    }
}
