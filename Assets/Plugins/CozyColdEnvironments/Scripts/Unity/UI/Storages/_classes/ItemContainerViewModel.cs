using CCEnvs.Disposables;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.GameSystems.Storages;
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
        private readonly ReactiveProperty<string> itemCountView = new();

        public IReadOnlyReactiveProperty<Sprite?> ItemIconView => itemIconView;
        public IReadOnlyReactiveProperty<string> ItemCountView => itemCountView;

        public ItemContainerViewModel(T model, GameObject gameObject)
            :
            base(model, gameObject)
        {
            model.Item.Subscribe(x => x.Match(
                some: (item) => itemIconView.Value = item.Icon,
                none: () => itemIconView.Value = null))
                .AddTo(this);

            model.ItemCount.Subscribe(count => itemCountView.Value = count.ToString())
                           .AddTo(this);
        }
    }
}
