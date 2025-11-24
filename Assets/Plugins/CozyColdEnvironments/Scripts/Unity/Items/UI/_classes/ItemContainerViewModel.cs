using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public class ItemContainerViewModel<T>
        : ViewModel<T>,
        IItemContainerViewModel

        where T : IItemContainer
    {
        private readonly ReactiveProperty<Sprite> iconView = new(initialValue: UCC.Transparent.Value);
        private readonly ReactiveProperty<string> counterView = new(initialValue: string.Empty);

        public IReadOnlyReactiveProperty<Sprite> IconView => iconView;
        public IReadOnlyReactiveProperty<string> CounterView => counterView;
        public Maybe<CompareAction<int>> ShowCounterTextPredicate { get; set; }

        public ItemContainerViewModel(T model)
            :
            base(model)
        {
            BindItemView();
            BindCounterText();
        }

        private void BindItemView()
        {
            model.ObserveItem()
                .SubscribeWithState(iconView,
                    static (x, prop) => prop.Value = x.Map(item => item.Icon)
                        .GetValue(UCC.Transparent.Value))
                .AddTo(disposables);

            iconView.AddTo(disposables);
        }

        private void BindCounterText()
        {
            model.ObserveItemCount()
                .Select(pair => pair.Current)
                .Select(itemCount => ShowCounterTextPredicate.BiMap(
                            some: predicate =>
                            {
                                if (predicate.Invoke(itemCount))
                                    return itemCount.ToString();
                                else
                                    return string.Empty;
                            },
                            none: () => itemCount > 1 ? itemCount.ToString() : string.Empty
                            ).GetValueUnsafe())
                .SubscribeWithState(counterView, 
                    static (countStr, prop) => prop.Value = countStr)
                .AddTo(disposables);

            counterView.AddTo(disposables);
        }
    }
}
