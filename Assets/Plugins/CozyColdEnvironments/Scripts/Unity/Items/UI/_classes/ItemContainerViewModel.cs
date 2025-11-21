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
        private readonly ReactiveProperty<Sprite> itemView = new(initialValue: UCC.Transparent.Value);
        private readonly ReactiveProperty<string> counterText = new(initialValue: string.Empty);

        public IReadOnlyReactiveProperty<Sprite> ItemView => itemView;
        public IReadOnlyReactiveProperty<string> CounterText => counterText;
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
                .SubscribeWithState(itemView,
                    static (x, prop) => prop.Value = x.Map(item => item.Icon)
                        .GetValue(UCC.Transparent.Value))
                .AddTo(disposables);

            itemView.AddTo(disposables);
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
                .SubscribeWithState(counterText, 
                    static (countStr, prop) => prop.Value = countStr)
                .AddTo(disposables);

            counterText.AddTo(disposables);
        }
    }
}
