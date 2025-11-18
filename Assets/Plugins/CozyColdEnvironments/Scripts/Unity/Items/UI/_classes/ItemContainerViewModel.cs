using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI.MVVM;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
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
        public Maybe<CompareAction<int>> ShowItemCounterPredicate { get; set; }

        public ItemContainerViewModel(T model, GameObject gameObject)
            :
            base(model, gameObject)
        {
            BindItemIcon();
            BindItemCount();
            IsActiveContainer = model.ObserveActiveState().ToReactiveProperty();
        }

        public void SetActiveState(Maybe<bool> state = default)
        {
            state.Match(
                some: state =>
                {
                    if (state)
                        model.ActivateContainer();
                    else
                        model.DeactivateContainer();
                },
                none: () => model.SwitchContainerActiveState()
                );
        }
        private void BindItemIcon()
        {
            model.ObserveItem()
                .SubscribeWithState(itemIcon,
                    static (x, prop) => prop.Value = x.Map(item => item.Icon)
                        .GetValue(UCC.TranparentSprite.Value))
                .AddTo(disposables);

            itemIcon.AddTo(disposables);
        }

        private void BindItemCount()
        {
            model.ObserveItemCount()
                .Select(pair => pair.Current)
                .Select(itemCount => ShowItemCounterPredicate.BiMap(
                            some: predicate =>
                            {
                                if (predicate.Invoke(itemCount))
                                    return itemCount.ToString();
                                else
                                    return string.Empty;
                            },
                            none: () => itemCount > 1 ? itemCount.ToString() : string.Empty
                            ).GetValueUnsafe())
                .SubscribeWithState(itemCount, 
                    static (countStr, prop) => prop.Value = countStr)
                .AddTo(disposables);

            itemCount.AddTo(disposables);
        }
    }
}
