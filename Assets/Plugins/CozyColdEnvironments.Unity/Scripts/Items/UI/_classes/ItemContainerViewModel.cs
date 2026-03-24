using CCEnvs.Disposables;
using CCEnvs.FuncLanguage;
using CCEnvs.Rx;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using R3;
using System;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public class ItemContainerViewModel<T>
        :
        ViewModel<T>,
        IItemContainerViewModel

        where T : IItemContainer
    {
        private readonly ReactiveProperty<Sprite> iconView = new(UCC.Transparent.Value);
        private readonly ReactiveProperty<string> counterView = new();

        private IDisposable? iconBinding;
        private IDisposable? counterBinding;

        public ReadOnlyReactiveProperty<Sprite> Icon => iconView;
        public ReadOnlyReactiveProperty<string> CounterView => counterView;
        public CompareAction<int>? ShowCounterTextPredicate { get; set; }

        public ItemContainerViewModel(T model, CancellationToken cancellationToken)
            :
            base(model, cancellationToken)
        {
        }

        ~ItemContainerViewModel() => Dispose();

        protected override void OnSetModel(T? model)
        {
            CCDisposable.Dispose(ref iconBinding);
            CCDisposable.Dispose(ref counterBinding);

            iconView.Value = UCC.Transparent.Value;
            counterView.Value = string.Empty;
        }

        protected override void InitModel(T model)
        {
            BindItem(model);
            BindCounterText(model);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                OnSetModel(default);
            }
        }

        private void BindItem(T model)
        {
            iconBinding = model.ObserveItem()
                .Unmaybe()
                .Subscribe(OnItemChanged);
        }

        private void OnItemChanged(IItem? item)
        {
            if (item.IsNull())
                iconView.Value = UCC.Transparent.Value;

            iconView.Value = item.Maybe()
                .Map(static item => item.Icon)
                .GetValue(UCC.Transparent.Value);
        }

        private void BindCounterText(T model)
        {
            counterBinding = model.ObserveItemCount()
                .Subscribe(OnCounterText);
        }

        private void OnCounterText(int itemCount)
        {
            if (ShowCounterTextPredicate.HasValue)
            {
                if (ShowCounterTextPredicate.Value.Invoke(itemCount))
                    counterView.Value = itemCount.ToString();
                else
                    counterView.Value = string.Empty;

                return;
            }
            else if (itemCount > 0)
                counterView.Value = itemCount.ToString();
            else
                counterView.Value = string.Empty;
        }
    }
}
