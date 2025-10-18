using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.UI.MVVM;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public class ItemContainerViewModel<T>
        : AViewModel<T>,
        IItemContainerViewModel<T>

        where T : IItemContainer
    {
        private readonly ReactiveProperty<Sprite> itemIconView = new();
        private readonly ReactiveProperty<int> itemCountView = new();

        public IReadOnlyReactiveProperty<Sprite> ItemIconView => itemIconView;
        public IReadOnlyReactiveProperty<int> ItemCountView => itemCountView;

        public ItemContainerViewModel(T model) : base(model)
        {
        }
    }
}
