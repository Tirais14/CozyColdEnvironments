using CCEnvs.Disposables;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using R3;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
#pragma warning disable IDE0044
#pragma warning disable S1125
namespace CCEnvs.Unity.Storages.UI
{
    [RequireComponent(typeof(Image))]
    public abstract class ItemContainerView<TViewModel>
        :
        View<TViewModel>

        where TViewModel : IItemContainerViewModel
    {
        [SerializeField]
        [GetByChildren(IsOptional = true)]
        protected TextMeshProUGUI counterMesh;

        private IDisposable? iconBinding;
        private IDisposable? counterBinding;

        protected override void InitViewModel(TViewModel vm)
        {
            BindItemIcon(vm);
            BindCounter(vm);
        }

        protected override void OnSetViewModel(TViewModel? vm)
        {
            CCDisposable.Dispose(ref iconBinding);
            CCDisposable.Dispose(ref counterBinding);
        }

        private void BindItemIcon(TViewModel vm)
        {
            if (image == null)
                return;

            iconBinding = vm.Icon.Subscribe(OnIconChanged);
        }

        private void OnIconChanged(Sprite icon)
        {
            CC.Guard.IsNotNull(image, nameof(image));

            image.sprite = icon;
        }

        private void BindCounter(TViewModel vm)
        {
            if (counterMesh == null)
                return;

            counterBinding = vm.CounterView.Subscribe(OnCounterChanged);
        }

        private void OnCounterChanged(string counterView)
        {
            CC.Guard.IsNotNull(counterMesh, nameof(counterMesh));

            counterMesh.text = counterView;
        }
    }
    public class ItemContainerView : ItemContainerView<ItemContainerViewModel<IItemContainer>>
    {
        [SerializeField]
        protected CompareAction<int> ShowCounterTextPredicate = new(1, CompareTypes.Equals | CompareTypes.Bigger);

        protected override ItemContainerViewModel<IItemContainer> CreateViewModel()
        {
            var cnt = new ItemContainer();
            return new ItemContainerViewModel<IItemContainer>(cnt)
            {
                ShowCounterTextPredicate = ShowCounterTextPredicate
            };
        }
    }
}
