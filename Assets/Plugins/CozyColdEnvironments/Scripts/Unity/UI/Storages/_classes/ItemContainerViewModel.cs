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
        private readonly ReactiveProperty<Maybe<Sprite>> itemIconView = new();
        private readonly ReactiveProperty<int> itemCountView = new();

        public IReadOnlyReactiveProperty<Maybe<Sprite>> ItemIconView => itemIconView;
        public IReadOnlyReactiveProperty<int> ItemCountView => itemCountView;

        public ItemContainerViewModel(T model, GameObject gameObject)
            :
            base(model, gameObject)
        {
            model.Item.Subscribe(x => itemIconView.Value = x.Map(x => x.Icon).Access())
                      .AddTo(this);

            model.ItemCount.Subscribe(count => itemCountView.Value = count)
                           .AddTo(this);
        }
    }
}
