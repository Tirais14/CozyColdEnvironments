using CCEnvs.Disposables;
using CCEnvs.UI;
using CCEnvs.Unity.Extensions;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.UI.MVVM;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public class ItemStackViewModel<T>
        :
        AViewModel<T>,
        IItemStackViewModel<T>

        where T : IItemStack, IItemContainerReactive
    {
        private readonly ReactiveProperty<string> counterView = new();
        private readonly ReactiveProperty<Sprite?> iconView = new();

        public IReadOnlyReactiveProperty<string> CounterView => counterView;
        public IReadOnlyReactiveProperty<Sprite?> IconView => iconView;

        public ItemStackViewModel(T model) : base(model)
        {
            model.ItemCountReactive.Subscribe(x => counterView.Value = x.ToString())
                                   .AddTo(this);

            model.ItemReactive.Subscribe(x => iconView.Value = x.IfNotNull(x => x.Icon)
                                                                .IfNull(UCC.ErrorSprite))
                              .AddTo(this);
        }
    }
}
