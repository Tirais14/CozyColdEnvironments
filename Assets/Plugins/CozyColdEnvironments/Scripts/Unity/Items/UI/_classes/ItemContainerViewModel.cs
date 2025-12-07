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
        private readonly ReactiveProperty<string> counterView = new();

        public IReadOnlyReactiveProperty<Sprite> IconView => iconView;
        public IReadOnlyReactiveProperty<string> CounterView => counterView;
        public Maybe<CompareAction<int>> ShowCounterTextPredicate { get; set; }

        public ItemContainerViewModel(T model)
            :
            base(model)
        {
            counterView.AddTo(disposables);
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
                .Select(static pair => pair.Current)
                .SubscribeWithState(this, 
                static (itemCount, @this) =>
                {
                    if (@this.ShowCounterTextPredicate.TryGetValue(out var predicate))
                    {
                        if (predicate.Invoke(itemCount))
                            @this.counterView.Value = itemCount.ToString();
                        else
                            @this.counterView.Value = string.Empty;

                        return;
                    }
                    else if (itemCount > 0)
                        @this.counterView.Value = itemCount.ToString();
                    else
                        @this.counterView.Value = string.Empty;

                })
                .AddTo(disposables);
        }
    }
}
