using UniRx;
using UnityEngine;
using CCEnvs.Disposables;
using CCEnvs.GameSystems.ItemStorageSystem;
using CCEnvs.UI.MVVM;
using CCEnvs.Unity.Extensions;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public class ItemStackViewModel<T> : AViewModel<T>,
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
                                                                .IfNull(CC.ErrorSprite))
                              .AddTo(this);
        }
    }
}
