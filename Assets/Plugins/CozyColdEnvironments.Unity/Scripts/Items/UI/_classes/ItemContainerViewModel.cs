using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using R3;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public class ItemContainerViewModel<T>
        : ViewModel<T>,
        IItemContainerViewModel

        where T : IItemContainer
    {
        private readonly ReactiveProperty<Sprite> iconView = new(UCC.Transparent.Value);
        private readonly ReactiveProperty<string> counterView = new();

        public ReadOnlyReactiveProperty<Sprite> IconView { get; private set; }
        public ReadOnlyReactiveProperty<string> CounterView { get; private set; }
        public Maybe<CompareAction<int>> ShowCounterTextPredicate { get; set; }

        public ItemContainerViewModel(T model, CancellationToken cancellationToken)
            :
            base(model, cancellationToken)
        {
            counterView.AddTo(disposables);

            IconView = iconView.ToReadOnlyReactiveProperty();
            CounterView = counterView.ToReadOnlyReactiveProperty();

            BindItemView();
            BindCounterText();
        }

        private void BindItemView()
        {
            model.ObserveItem()
                .Subscribe(iconView,
                    static (x, prop) => prop.Value = x.Map(item => item.Icon)
                        .GetValue(UCC.Transparent.Value))
                .AddTo(disposables);

            iconView.AddTo(disposables);
        }

        private void BindCounterText()
        {
            model.ObserveItemCount()
                .Subscribe(this,
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
